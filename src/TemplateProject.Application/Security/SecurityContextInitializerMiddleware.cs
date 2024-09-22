using System.Threading.Tasks;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TemplateProject.Application.Extensions;
using LogContext = Serilog.Context.LogContext;

namespace TemplateProject.Application.Security;

public sealed class SecurityContextInitializerMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityContextInitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }
        
        var logger = context.RequestServices.GetRequiredService<ILogger<SecurityContextInitializerMiddleware>>();
        var securityContext = context.RequestServices.GetRequiredService<SecurityContext>();
        var jwtTokenReader = context.RequestServices.GetRequiredService<IJwtTokenReader>();
        
        if (endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
        {
            securityContext.InitializeAsAnonymous();
            await _next(context);
            return;
        }
        
        if (!jwtTokenReader.IsLoggedIn)
        {
            logger.LogInformation("Request is not authenticated. Skipping security context initialization.");
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        
        var userId = jwtTokenReader.GetUserId();
        
        securityContext.Initialize(userId);

        var userContextLogData = new UserContextLogData()
        {
            UserId = userId.ToString(),
        };
        
        using var _ = LogContext.PushProperty("UserContext", userContextLogData, destructureObjects: true);
        
        await _next(context);
    }
    
    private sealed class UserContextLogData
    {
        public string UserId { get; set; }
    }
}