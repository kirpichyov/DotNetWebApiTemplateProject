using System;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentAssertions.Execution;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TemplateProject.Application.Models.Auth;
using TemplateProject.DataAccess.Connection;
using TemplateProject.IntegrationTests.Testing;
using TemplateProject.Tests.Common;

namespace TemplateProject.IntegrationTests;

[TestFixture]
public sealed class AuthTests
{
    private readonly DataGenerator _dataGenerator = new();
    private readonly Faker _faker = new();
    
    [Test]
    public async Task SignUp_RequestIsValid_ShouldReturn201()
    {
        // Arrange
        var client = SystemUnderTest.GetAnonymousClient();
        var request = _dataGenerator.GivenUserRegisterRequest();

        // Act
        var response = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status201Created);
        await AssertUserSaved(request);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("1234567")]
    [TestCase("qwertyu")]
    public async Task SignUp_PasswordIsNotMeetRequirements_ShouldReturn400(string password)
    {
        // Arrange
        var client = SystemUnderTest.GetAnonymousClient();
        var request = _dataGenerator.GivenUserRegisterRequest();

        request = request with
        {
            Password = password,
        };

        // Act
        var response = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Test]
    public async Task SignIn_RequestIsValid_ShouldReturn200()
    {
        // Arrange
        var client = SystemUnderTest.GetAnonymousClient();
        var signUpRequest = _dataGenerator.GivenUserRegisterRequest();

        // Act
        var signUpResponse = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(signUpRequest);
        
        var signInRequest = new SignInRequest()
        {
            Email = signUpRequest.Email,
            Password = signUpRequest.Password,
        };

        var signInResponse = await client.Request(Endpoints.V1.Auth.SignIn())
            .PostJsonAsync(signInRequest);

        // Assert
        using var scope = new AssertionScope();
        signUpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);
        signInResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

        var authResponseModel = await signInResponse.GetJsonAsync<AuthResponse>();
        authResponseModel.Jwt.Should().NotBeNull();
        authResponseModel.Jwt.AccessToken.Should().NotBeNull();
        authResponseModel.Jwt.ExpiresAtUtc.Should().NotBeNull();
        authResponseModel.RefreshToken.Should().NotBeNull();
        authResponseModel.RefreshToken.Token.Should().NotBeNull();
    }

    [Test]
    public async Task SignIn_EmailIsNotValid_ShouldReturn400()
    {
        // Arrange
        var client = SystemUnderTest.GetAnonymousClient();
        var signUpRequest = _dataGenerator.GivenUserRegisterRequest();

        // Act
        var signUpResponse = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(signUpRequest);
        
        var signInRequest = new SignInRequest()
        {
            Email = new Person().Email,
            Password = signUpRequest.Password,
        };

        var signInResponse = await client.Request(Endpoints.V1.Auth.SignIn())
            .PostJsonAsync(signInRequest);

        // Assert
        using var scope = new AssertionScope();
        signUpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);
        signInResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task SignIn_PasswordIsNotValid_ShouldReturn400()
    {
        // Arrange
        var client = SystemUnderTest.GetAnonymousClient();
        var signUpRequest = _dataGenerator.GivenUserRegisterRequest();

        // Act
        var signUpResponse = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(signUpRequest);
        
        var signInRequest = new SignInRequest()
        {
            Email = signUpRequest.Email,
            Password = signUpRequest.Password + _faker.Random.String2(length: 1),
        };

        var signInResponse = await client.Request(Endpoints.V1.Auth.SignIn())
            .PostJsonAsync(signInRequest);

        // Assert
        using var scope = new AssertionScope();
        signUpResponse.StatusCode.Should().Be(StatusCodes.Status201Created);
        signInResponse.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Test]
    public async Task RefreshToken_RequestIsValid_ShouldReturn200()
    {
        // Arrange
        var client = await SystemUnderTest.GetAuthorizedClient();

        var request = new RefreshTokenRequest
        {
            RefreshToken = Guid.Parse(client.AuthInfo.RefreshToken.Token),
            AccessToken = client.AuthInfo.Jwt.AccessToken,
        };

        // Act
        var response = await client.FlurlClient.Request(Endpoints.V1.Auth.Refresh())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status200OK);

        var authResponseModel = await response.GetJsonAsync<AuthResponse>();
        authResponseModel.Jwt.Should().NotBeNull();
        authResponseModel.Jwt.AccessToken.Should().NotBeNull();
        authResponseModel.Jwt.ExpiresAtUtc.Should().NotBeNull();
        authResponseModel.RefreshToken.Should().NotBeNull();
        authResponseModel.RefreshToken.Token.Should().NotBeNull();
    }
    
    [Test]
    public async Task RefreshToken_AccessTokenIsNotValid_ShouldReturn400()
    {
        // Arrange
        var client = await SystemUnderTest.GetAuthorizedClient();

        var request = new RefreshTokenRequest
        {
            RefreshToken = Guid.Parse(client.AuthInfo.RefreshToken.Token),
            AccessToken = _faker.Random.String2(length: 32),
        };

        // Act
        var response = await client.FlurlClient.Request(Endpoints.V1.Auth.Refresh())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Test]
    public async Task RefreshToken_AccessTokenIsForOtherRefreshToken_ShouldReturn400()
    {
        // Arrange
        var client = await SystemUnderTest.GetAuthorizedClient();
        var otherClient = await SystemUnderTest.GetAuthorizedClient();

        var request = new RefreshTokenRequest
        {
            RefreshToken = Guid.Parse(client.AuthInfo.RefreshToken.Token),
            AccessToken = otherClient.AuthInfo.Jwt.AccessToken,
        };

        // Act
        var response = await client.FlurlClient.Request(Endpoints.V1.Auth.Refresh())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Test]
    public async Task RefreshToken_RefreshTokenIsNotValid_ShouldReturn400()
    {
        // Arrange
        var client = await SystemUnderTest.GetAuthorizedClient();

        var request = new RefreshTokenRequest
        {
            RefreshToken = Guid.NewGuid(),
            AccessToken = client.AuthInfo.Jwt.AccessToken,
        };

        // Act
        var response = await client.FlurlClient.Request(Endpoints.V1.Auth.Refresh())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Test]
    public async Task RefreshToken_RefreshTokenIsForOtherJwt_ShouldReturn400()
    {
        // Arrange
        var client = await SystemUnderTest.GetAuthorizedClient();
        var otherClient = await SystemUnderTest.GetAuthorizedClient();

        var request = new RefreshTokenRequest
        {
            RefreshToken = Guid.Parse(otherClient.AuthInfo.RefreshToken.Token),
            AccessToken = client.AuthInfo.Jwt.AccessToken,
        };

        // Act
        var response = await client.FlurlClient.Request(Endpoints.V1.Auth.Refresh())
            .PostJsonAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    private static async Task AssertUserSaved(UserRegisterRequest request)
    {
        var databaseContext = SystemUnderTest.GetService<DatabaseContext>();
        var user = await databaseContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Email == request.Email);
        user.Should().NotBeNull();
    }
}