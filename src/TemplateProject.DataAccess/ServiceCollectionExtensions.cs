using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TemplateProject.DataAccess.Connection;
using TemplateProject.DataAccess.Contracts;
using TemplateProject.DataAccess.Interceptors;
using TemplateProject.DataAccess.Repositories;

namespace TemplateProject.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddDbContext<DatabaseContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(DatabaseContext)))
                    .AddInterceptors(new AuditableEntitySaveChangesInterceptor(sp));

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                if (environment.IsDevelopment())
                {
                    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                    options.EnableSensitiveDataLogging();
                }
            }
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokensRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}