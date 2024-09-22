using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using TemplateProject.Application.Models.Auth;
using TemplateProject.DataAccess.Connection;
using TemplateProject.xUnit.IntegrationTests.Endpoints;
using TemplateProject.xUnit.IntegrationTests.Extensions;
using TemplateProject.xUnit.IntegrationTests.Factory;

namespace TemplateProject.xUnit.IntegrationTests.Fixture;

public class SystemUnderTestFixture : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    private readonly IntegrationTestsWebApplicationFactory _factory;
    protected readonly Faker Faker = new();
    
    public RestEndpoints Endpoints { get; } = new();
    
    private readonly TestsDbMode _dbMode;

    protected SystemUnderTestFixture()
    {
        var testsConfig = new ConfigurationBuilder()
            .AddJsonFile("integration-tests-config.json")
            .AddJsonFile("integration-tests-config.local.json", optional: true)
            .Build();
        
        _dbMode = testsConfig.GetSection("DbMode").Get<TestsDbMode>();
        var realDbConnectionString = testsConfig.GetConnectionString("RealDb");
        
        _factory = new IntegrationTestsWebApplicationFactory(_dbMode, realDbConnectionString);
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
        
        using var scope = CreateScope();
        
        if (_dbMode is TestsDbMode.TestContainers)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        await OnInitializeAsync();
    }

    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await OnDisposeAsync();
    }
    
    protected virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask;
    }

    public IServiceScope CreateScope()
    {
        return _factory.Services.CreateScope();
    }
    
    public HttpClient GetHttpClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    public async Task<UserWithRestClient> GetContextForUserAsync(
        string email,
        string password)
    {
        var anonymousClient = BuildRestClient();

        var signInRequest = new SignInRequest()
        {
            Password = password,
            Email = email,
        };

        var endpoints = new RestEndpoints();
        
        var signInResponse = await anonymousClient.ExecuteAsync<AuthResponse>(
            endpoints.InternalApi.Auth_SignIn(signInRequest));

        signInResponse.ThrowOnFailStatusCode();

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == signInResponse.Data!.UserId);

        var restClient = BuildRestClient(signInResponse.Data!.Jwt.AccessToken);
        
        return new UserWithRestClient()
        {
            User = user,
            RestClient = restClient,
        };
    }
    
    public async Task<UserWithRestClient> CreateAndGetContextForUserAsync(
        UserRegisterRequest requestModel)
    {
        var endpoints = new RestEndpoints();

        var anonymousClient = BuildRestClient();
        
        var registerResponse = await anonymousClient.ExecuteAsync<AuthResponse>(
            endpoints.InternalApi.Auth_Register(requestModel));
        
        registerResponse.ThrowOnFailStatusCode();

        return await GetContextForUserAsync(requestModel.Email, requestModel.Password);
    }
    
    private RestClient BuildRestClient(string token = null)
    {
        IAuthenticator authenticator = null;

        if (!string.IsNullOrEmpty(token))
        {
            authenticator = new JwtAuthenticator(token);
        }

        var httpClient = GetHttpClient();
        var restClient = new RestClient(httpClient, new RestClientOptions()
        {
            Authenticator = authenticator,
        }, configureSerialization: cfg =>
        {
            cfg.UseSystemTextJson(JsonSerializerOptions);
        });

        return restClient;
    }
}