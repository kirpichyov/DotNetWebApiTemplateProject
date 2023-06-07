using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TemplateProject.Api.Configuration;
using TemplateProject.Api.Extensions;

namespace TemplateProject.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var basicConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        return Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider((_, options) =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .AddLogging(basicConfiguration)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}