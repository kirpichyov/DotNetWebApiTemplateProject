using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TemplateProject.Api.Configuration;
using TemplateProject.Core.Constants;
using TemplateProject.DataAccess.Connection;
using TemplateProject.DataAccess.Interceptors;
using Testcontainers.PostgreSql;

namespace TemplateProject.xUnit.IntegrationTests.Factory;

public class IntegrationTestsWebApplicationFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("templateproject-int-tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly TestsDbMode _dbMode;
    private readonly string _realDbConnectionString;
    
    public IntegrationTestsWebApplicationFactory(TestsDbMode dbMode, string realDbConnectionString)
    {
        _dbMode = dbMode;
        _realDbConnectionString = realDbConnectionString;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentNames.IntegrationTests);
        
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(IntegrationTestsAppSettings.Settings);
        });
        
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
            
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<DatabaseContext>((sp, options) =>
            {
                var connectionString = _dbMode is TestsDbMode.RealDb
                    ? _realDbConnectionString
                    : _dbContainer.GetConnectionString();
                
                options.UseNpgsql(connectionString)
                    .AddInterceptors(new AuditableEntitySaveChangesInterceptor(sp));

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));
                options.EnableSensitiveDataLogging();
            });
        });
    }

    public async Task InitializeAsync()
    {
        if (_dbMode is TestsDbMode.TestContainers)
        {
            await _dbContainer.StartAsync();
        }
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        
        if (_dbMode is TestsDbMode.TestContainers)
        {
            await _dbContainer.StopAsync();
        }
    }
}