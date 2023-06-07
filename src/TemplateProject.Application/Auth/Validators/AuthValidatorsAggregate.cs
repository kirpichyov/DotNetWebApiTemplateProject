using FluentValidation;
using TemplateProject.Application.Auth.Contracts;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.Application.Auth.Validators;

public sealed class AuthValidatorsAggregate : IAuthValidatorsAggregate
{
    public AuthValidatorsAggregate(
        IValidator<SignInRequest> signInValidator,
        IValidator<RefreshTokenRequest> refreshTokenValidator,
        IValidator<UserRegisterRequest> userRegisterValidator)
    {
        SignInValidator = signInValidator;
        RefreshTokenValidator = refreshTokenValidator;
        UserRegisterValidator = userRegisterValidator;
    }

    public IValidator<SignInRequest> SignInValidator { get; }
    public IValidator<RefreshTokenRequest> RefreshTokenValidator { get; }
    public IValidator<UserRegisterRequest> UserRegisterValidator { get; }
}