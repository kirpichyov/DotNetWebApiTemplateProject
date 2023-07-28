using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Kirpichyov.FriendlyJwt.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TemplateProject.Api.Configuration.Converters;
using TemplateProject.Api.Configuration.Middleware.Filters;
using TemplateProject.Api.Configuration.Swagger;
using TemplateProject.Application;
using TemplateProject.Application.Validators.Auth;
using TemplateProject.Core.Options;
using TemplateProject.DataAccess;

namespace TemplateProject.Api.Configuration;

public class Startup
{
	private const string MainCorsPolicy = "MainPolicy";

	private readonly IConfiguration _configuration;
	private readonly IWebHostEnvironment _environment;

	public Startup(IConfiguration configuration, IWebHostEnvironment environment)
	{
		_configuration = configuration;
		_environment = environment;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddHttpContextAccessor();

		services.AddFriendlyJwt();
		services.Configure<AuthOptions>(_configuration.GetSection(nameof(AuthOptions)));

		services.AddDataAccessServices(_configuration, _environment);
		services.AddApplicationServices(_configuration);
		services.AddBroker();
		services.AddJobs(_configuration);

		services.AddRouting(options => options.LowercaseUrls = true);

		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;

			options.ApiVersionReader = ApiVersionReader.Combine(
				new UrlSegmentApiVersionReader(),
				new HeaderApiVersionReader("x-api-version"),
				new MediaTypeApiVersionReader("x-api-version")
			);
		});
		
		services.AddCors(options =>
		{
			options.AddPolicy(name: MainCorsPolicy,
				policy =>
				{
					var authOptions = _configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>();
			
					policy.WithOrigins(authOptions.AllowedOrigins);
					policy.AllowAnyHeader();
					policy.AllowAnyMethod();
				});
		});

		services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
				options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
				options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
			})
			.AddFriendlyJwtAuthentication(configuration =>
			{
				var authOptions = _configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>();
				configuration.Bind(authOptions);
			})
			.AddMvcOptions(options =>
			{
				options.Filters.Add<ExceptionFilter>();
			});

		ValidatorOptions.Global.LanguageManager.Enabled = false;
		services.AddValidatorsFromAssemblyContaining<UserRegisterRequestValidator>();

		services.AddVersionedApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});
		
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(options =>
		{
			options.AddSecurityDefinition("Bearer",
				new OpenApiSecurityScheme
				{
					Name = HeaderNames.Authorization,
					Type = SecuritySchemeType.ApiKey,
					In = ParameterLocation.Header,
					Description = "Obtained JWT."
				});

			options.MapType<DateOnly>(() => new OpenApiSchema()
			{
				Type = "string",
				Format = "date",
			});
			
			options.OperationFilter<AuthOperationFilter>();
			options.ExampleFilters();
		});

		services.ConfigureOptions<ConfigureSwaggerOptions>();
		services.AddSwaggerExamplesFromAssemblyOf<Startup>();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
			var apiVersionDescriptionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
				
			foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
			{
				options.SwaggerEndpoint(
					$"/swagger/{description.GroupName}/swagger.json", 
					description.GroupName.ToUpperInvariant()
				);
			}
		});

		app.UseHttpsRedirection();
		app.UseRouting();

		app.UseCors(MainCorsPolicy);
		
		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();

			endpoints.MapGet("/ping",
				async context => { await context.Response.WriteAsync($"Pong! [{DateTime.UtcNow}]"); }
			);
		});
	}
}