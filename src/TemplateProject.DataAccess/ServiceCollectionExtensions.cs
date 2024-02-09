using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TemplateProject.Core.Options;
using TemplateProject.DataAccess.Connection;
using TemplateProject.DataAccess.Contracts;
using TemplateProject.DataAccess.Repositories;

namespace TemplateProject.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(DatabaseContext)));

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                if (environment.IsDevelopment())
                {
                    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                    options.EnableSensitiveDataLogging();
                }
            }
        );
        
        services.Configure<MongoDatabaseOptions>(configuration.GetSection(nameof(MongoDatabaseOptions)));
        var mongoDatabaseOptions = configuration.GetSection(nameof(MongoDatabaseOptions)).Get<MongoDatabaseOptions>();

        var mongoDatabase = new MongoClient(mongoDatabaseOptions.ConnectionString)
            .GetDatabase(mongoDatabaseOptions.DatabaseName);

        services.AddScoped(_ => MongoDbContext.Create(mongoDatabase));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokensRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}