using System;
using System.Threading.Tasks;
using Kirpichyov.FriendlyJwt;
using Kirpichyov.FriendlyJwt.Contracts;
using Kirpichyov.FriendlyJwt.RefreshTokenUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TemplateProject.Application.Contracts;
using TemplateProject.Application.Models.Auth;
using TemplateProject.Core.Contracts;
using TemplateProject.Core.Contracts.Repositories;
using TemplateProject.Core.Exceptions;
using TemplateProject.Core.Models.Entities;
using TemplateProject.Core.Options;

namespace TemplateProject.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashingProvider _hashingProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AuthOptions _authOptions;
    private readonly IJwtTokenVerifier _jwtTokenVerifier;
    private readonly ILogger<AuthService> _logger;
    private readonly IFluentValidatorFactory _fluentValidatorFactory;

    public AuthService(
        IUnitOfWork unitOfWork,
        IHashingProvider hashingProvider,
        IDateTimeProvider dateTimeProvider,
        IOptions<AuthOptions> authOptions,
        IJwtTokenVerifier jwtTokenVerifier,
        ILogger<AuthService> logger,
        IFluentValidatorFactory fluentValidatorFactory)
    {
        _unitOfWork = unitOfWork;
        _hashingProvider = hashingProvider;
        _dateTimeProvider = dateTimeProvider;
        _jwtTokenVerifier = jwtTokenVerifier;
        _logger = logger;
        _fluentValidatorFactory = fluentValidatorFactory;
        _authOptions = authOptions.Value;
    }

    public async Task<UserCreatedResponse> CreateUser(UserRegisterRequest request)
    {
        _fluentValidatorFactory.ValidateAndThrow(request);
        
        var emailExists = await _unitOfWork.Users.IsEmailExists(request.Email);
        if (emailExists)
        {
            throw new ValidationFailedException(nameof(request.Email), "Is already in use.");
        }

        var passwordHash = _hashingProvider.GetHash(request.Password);
        var user = new User(request.Firstname, request.Lastname, request.Email, passwordHash);
        
        _unitOfWork.Users.Add(user);
        await _unitOfWork.CommitAsync();

        return new UserCreatedResponse()
        {
            Id = user.Id,
            Email = user.Email,
        };
    }

    public async Task<AuthResponse> GenerateJwtSession(SignInRequest request)
    {
        _fluentValidatorFactory.ValidateAndThrow(request);
        
        var user = await _unitOfWork.Users.TryGet(request.Email, withTracking: true);
        if (user is null || !_hashingProvider.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationFailedException("Credentials are invalid.");
        }
        
        var response = await _unitOfWork.CommitTransactionAsync(() => CreateAuthResponse(user));
        return response;
    }

    public async Task<AuthResponse> RefreshJwtSession(RefreshTokenRequest request)
    {
        _fluentValidatorFactory.ValidateAndThrow(request);
        
        var refreshToken = await _unitOfWork.RefreshTokens.FindById(request.RefreshToken, useTracking: true);
        var jwtVerificationResult = EnsureRefreshRequestIsValid(request, refreshToken);

        var userId = Guid.Parse(jwtVerificationResult.UserId);
        var user = await _unitOfWork.Users.TryGet(userId, withTracking: true);

        try
        {
            var authResponse = await _unitOfWork.CommitTransactionAsync(() =>
            {
                _unitOfWork.RefreshTokens.Remove(refreshToken);
                return CreateAuthResponse(user);
            });
            
            return authResponse;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            _logger.LogError(dbUpdateConcurrencyException, "Failed to refresh token");
            throw new ConflictException("Token was already refreshed concurrently");
        }
    }

    private JwtVerificationResult EnsureRefreshRequestIsValid(RefreshTokenRequest request, RefreshToken refreshToken)
    {
        var jwtVerificationResult = _jwtTokenVerifier.Verify(request.AccessToken);

        if (!jwtVerificationResult.IsValid)
        {
            throw new ValidationFailedException(nameof(request.AccessToken), "Access token is invalid.");
        }
        
        var accessTokenId = Guid.Parse(jwtVerificationResult.TokenId);
        
        if (refreshToken is null || refreshToken.AccessTokenId != accessTokenId)
        {
            throw new ValidationFailedException(nameof(request.RefreshToken), "Refresh token is not found.");
        }

        if (refreshToken.IsInvalidated)
        {
            throw new ValidationFailedException(nameof(request.RefreshToken), "Refresh token is invalidated.");
        }

        if (IsRefreshTokenExpired(refreshToken, _authOptions.RefreshTokenLifetime))
        {
            throw new ValidationFailedException(nameof(request.RefreshToken), "Refresh token is expired.");
        }

        return jwtVerificationResult;
    }

    private GeneratedTokenInfo GenerateJwt(User user)
    {
        var lifeTime = TimeSpan.FromSeconds(_authOptions.AccessTokenLifetime);

        return new JwtTokenBuilder(lifeTime, _authOptions.Secret)
            .WithAudience(_authOptions.Audience)
            .WithIssuer(_authOptions.Issuer)
            .WithUserIdPayloadData(user.Id.ToString())
            .WithUserEmailPayloadData(user.Email)
            .Build();
    }

    private RefreshToken CreateRefreshToken(Guid jwtId, User user)
    {
        var createdAtUtc = _dateTimeProvider.UtcNow;
        var refreshToken = new RefreshToken(jwtId, createdAtUtc, user);
        _unitOfWork.RefreshTokens.Add(refreshToken);

        return refreshToken;
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var jwtInfo = GenerateJwt(user);
        var tokenId = Guid.Parse(jwtInfo.TokenId);
        var refreshToken = CreateRefreshToken(tokenId, user);

        return new AuthResponse()
        {
            Jwt = new JwtResponse()
            {
                AccessToken = jwtInfo.Token,
                ExpiresAtUtc = jwtInfo.ExpiresOn,
            },
            RefreshToken = new RefreshTokenResponse()
            {
                Token = refreshToken.Id.ToString(),
                ExpiresAtUtc = refreshToken.CreatedAtUtc.AddSeconds(_authOptions.RefreshTokenLifetime),
            },
            UserId = user.Id,
        };
    }
    
    private bool IsRefreshTokenExpired(RefreshToken refreshToken, long lifeTime)
    {
        return _dateTimeProvider.UtcNow >= refreshToken.CreatedAtUtc.AddSeconds(lifeTime);
    }
}