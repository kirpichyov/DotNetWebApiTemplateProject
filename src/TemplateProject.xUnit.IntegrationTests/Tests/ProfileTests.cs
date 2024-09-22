using System.Net;
using FluentAssertions;
using RestSharp;
using TemplateProject.Application.Models.Profile;
using TemplateProject.Tests.Common;
using TemplateProject.xUnit.IntegrationTests.Endpoints;
using TemplateProject.xUnit.IntegrationTests.Fixture;
using Xunit.Abstractions;

namespace TemplateProject.xUnit.IntegrationTests.Tests;

public sealed class ProfileTests : IClassFixture<ProfileTestsFixture>
{
    private readonly ProfileTestsFixture _systemUnderTest;
    private readonly ITestOutputHelper _output;
    private readonly RestEndpoints _endpoints;
    private readonly DataGenerator _dataGenerator = new();

    public ProfileTests(
        ProfileTestsFixture systemUnderTest,
        ITestOutputHelper output)
    {
        _systemUnderTest = systemUnderTest;
        _output = output;
        _endpoints = new RestEndpoints();
    }

    [Fact]
    public async Task GetCurrentProfile_NewUserSignedIn_ShouldReturnOkWithExpectedData()
    {
        // Arrange
        var registerUserRequestModel = _dataGenerator.GivenUserRegisterRequest();
        var newUser = await _systemUnderTest.CreateAndGetContextForUserAsync(registerUserRequestModel);
        
        // Act
        var getProfileRequest = _endpoints.InternalApi.Profile_GetCurrent();
        
        var response = await newUser.RestClient
            .ExecuteAsync<CurrentUserProfileResponse>(getProfileRequest);
        
        // Assert
        _output.WriteLine($"Response: {response.Content}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Data.Should().NotBeNull();
        response.Data?.Id.Should().Be(newUser.User.Id);
        response.Data?.Email.Should().Be(newUser.User.Email);
    }
}