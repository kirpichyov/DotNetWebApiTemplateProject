﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TemplateProject.Api.Configuration.Swagger;

public sealed class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Configure each API discovered for Swagger Documentation.
    /// </summary>
    public void Configure(SwaggerGenOptions options)
    {
        // Add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    /// <summary>
    /// Configure Swagger Options. Inherited from the Interface.
    /// </summary>
    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    /// <summary>
    /// Create information about the version of the API.
    /// </summary>
    /// <returns>Information about the API.</returns>
    private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
    {
        var info = new OpenApiInfo()
        {
            Title = "TemplateProject API",
            Version = desc.ApiVersion.ToString()
        };

        if (desc.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}