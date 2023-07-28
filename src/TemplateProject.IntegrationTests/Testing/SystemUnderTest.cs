using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using TemplateProject.Api.Configuration;
using TemplateProject.Application.Models.Auth;
using TemplateProject.DataAccess.Connection;
using TemplateProject.Tests.Common;
using Testcontainers.PostgreSql;

namespace TemplateProject.IntegrationTests.Testing;

public static class SystemUnderTest
{
    private static TestServer _testServer;
    private static PostgreSqlContainer _postgreSqlContainer;
    private static DataGenerator _dataGenerator = new();
    
    private enum DatabaseMode
    {
        RealDatabase,
        Docker
    }

    public static async Task StartAsync()
    {
        var testsConfiguration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.integration-tests.json", false)
            .Build();

        var apiConfigurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true);

        var webHostBuilder = new WebHostBuilder()
            .UseEnvironment("IntegrationTests")
            .UseStartup<Startup>();

        var databaseModeRaw = testsConfiguration["DatabaseMode"];
        var databaseMode = Enum.Parse<DatabaseMode>(databaseModeRaw, ignoreCase: true);

        const string databaseConnectionsStringOptionsKey = "ConnectionStrings:DatabaseContext";
        
        if (databaseMode is DatabaseMode.Docker)
        {
            await StartDockerDb();

            apiConfigurationBuilder
                .AddInMemoryCollection(new KeyValuePair<string, string>[]
                {
                    new(databaseConnectionsStringOptionsKey, _postgreSqlContainer.GetConnectionString()),
                });
        }
        else
        {
            var testDatabaseConnectionString = testsConfiguration["TestDatabaseConnectionString"];
            
            apiConfigurationBuilder
                .AddInMemoryCollection(new KeyValuePair<string, string>[]
                {
                    new(databaseConnectionsStringOptionsKey, testDatabaseConnectionString),
                });
        }

        webHostBuilder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddConfiguration(apiConfigurationBuilder.Build());
        });

        _testServer = new TestServer(webHostBuilder);
        
        await _testServer.Host.StartAsync();
        await ApplyMigrations();
    }
    
    public static IFlurlClient GetAnonymousClient()
    {
        var httpClient = _testServer.CreateClient();
        var flurlClient = new FlurlClient(httpClient);
        
        flurlClient.AllowAnyHttpStatus();
        flurlClient.BaseUrl = httpClient.BaseAddress + "api/";

        return flurlClient;
    }

    public static async Task<AuthorizedClient> GetAuthorizedClient()
    {
        var signUpRequest = _dataGenerator.GivenUserRegisterRequest();

        var client = GetAnonymousClient();

        var signUpResponse = await client.Request(Endpoints.V1.Auth.SignUp())
            .PostJsonAsync(signUpRequest);

        if (signUpResponse.StatusCode != StatusCodes.Status201Created)
        {
            throw new InvalidOperationException("Unable to setup user. Sign up failed.");
        }

        var signInRequest = new SignInRequest()
        {
            Email = signUpRequest.Email,
            Password = signUpRequest.Password,
        };

        var signInResponse = await client.Request(Endpoints.V1.Auth.SignIn())
            .PostJsonAsync(signInRequest);
        
        if (signInResponse.StatusCode != StatusCodes.Status200OK)
        {
            throw new InvalidOperationException("Unable to setup user. Sign in failed.");
        }

        var authResponseModel = await signInResponse.GetJsonAsync<AuthResponse>();
        client.Headers.Add(HeaderNames.Authorization, $"Bearer {authResponseModel.Jwt.AccessToken}");

        return new AuthorizedClient(client, signUpRequest, authResponseModel);
    }

    public static TService GetService<TService>()
    {
        return _testServer.Services.GetRequiredService<TService>();
    } 

    private static async Task StartDockerDb()
    {
        var postgreSqlBuilder = new PostgreSqlBuilder()
            .WithDatabase("integration-tests-db")
            .WithPassword("P@ssvv0rdF0rTests")
            .WithCleanUp(true);

        _postgreSqlContainer = postgreSqlBuilder.Build();
        await _postgreSqlContainer.StartAsync();
    }

    private static async Task ApplyMigrations()
    {
        var databaseContext = _testServer.Services.GetRequiredService<DatabaseContext>();
        await databaseContext.Database.EnsureCreatedAsync();
    }

    public static async Task ShutdownAsync()
    {
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
        await _testServer.Host.StopAsync();
        _testServer.Dispose();
    }
}