using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TemplateProject.Api.Configuration.Swagger;

internal sealed class AuthOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var attributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
			.Union(context.MethodInfo.GetCustomAttributes(true))
			.ToArray();

		var allowAnonymous = attributes
			.OfType<AllowAnonymousAttribute>()
			.Any();

		var hasAuthorizeAttribute = attributes
			.OfType<AuthorizeAttribute>()
			.Any();
		
		if (allowAnonymous || !hasAuthorizeAttribute)
		{
			return;
		}
		
		var securityRequirement = new OpenApiSecurityRequirement();
		securityRequirement.Add(
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"},
			}, Array.Empty<string>());

		operation.Security = new[] {securityRequirement};

		operation.Responses.Add(
			((int)HttpStatusCode.Unauthorized).ToString(),
			GetEmptyJsonResponse(nameof(HttpStatusCode.Unauthorized))
		);

		operation.Responses.Add(
			((int)HttpStatusCode.Forbidden).ToString(),
			GetEmptyJsonResponse(nameof(HttpStatusCode.Forbidden))
		);
	}

	private static OpenApiResponse GetEmptyJsonResponse(string description)
	{
		return new OpenApiResponse
		{
			Content = new Dictionary<string, OpenApiMediaType>
			{
				{
					"application/json",
					new OpenApiMediaType {Schema = new OpenApiSchema {Default = new OpenApiString("{}")}}
				}
			},
			Description = description
		};
	}
}
