using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TemplateProject.Core.Exceptions;
using TemplateProject.Core.Models.Api;

namespace TemplateProject.Api.Configuration.Swagger;

public sealed class ApiErrorResponseExample : IMultipleExamplesProvider<ApiErrorResponse>
{
    public IEnumerable<SwaggerExample<ApiErrorResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Internal validation failed",
            new ApiErrorResponse(ExceptionsInfo.Identifiers.ValidationFailed, new[]
            {
                new ApiErrorResponseNode(null, "Here the error message.")
            }));
        
        yield return SwaggerExample.Create(
            "Request model validation failed",
            new ApiErrorResponse(ExceptionsInfo.Identifiers.ModelValidationFailed, new[]
            {
                new ApiErrorResponseNode("PropertyName", new[] { "Here the error message.", "And another error message." }),
                new ApiErrorResponseNode("OtherPropertyName", "Here the error message."),
            }));
        
        yield return SwaggerExample.Create(
            "Resource not found",
            new ApiErrorResponse(ExceptionsInfo.Identifiers.ResourceNotFound, new[]
            {
                new ApiErrorResponseNode(null, "Here the error message.")
            }));
        
        yield return SwaggerExample.Create(
            "Unexpected error",
            new ApiErrorResponse(ExceptionsInfo.Identifiers.Generic, new[]
            {
                new ApiErrorResponseNode(null, "Unexpected error occured during request")
            }));
    }
}