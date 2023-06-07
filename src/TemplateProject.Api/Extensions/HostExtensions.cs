using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TemplateProject.Core.Options;

namespace TemplateProject.Api.Extensions;

internal static class HostExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        var loggingOptions = configuration.GetSection("Logging").Get<LoggingOptions>();

        hostBuilder.UseSerilog((_, logger) =>
        {
            logger
                .Enrich.FromLogContext()
                .WriteTo.Console(loggingOptions.ConsoleLogLevel);
        });

        hostBuilder.ConfigureWebHost(builder => builder.Configure(app => app.UseSerilogRequestLogging()));

        return hostBuilder;
    }
}