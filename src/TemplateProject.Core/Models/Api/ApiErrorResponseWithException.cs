using System;

namespace TemplateProject.Core.Models.Api;

public sealed class ApiErrorResponseWithException : ApiErrorResponse
{
    public ApiErrorResponseWithException(ApiErrorResponse original, Exception exception)
        : base(original.ErrorType, original.Errors)
    {
        ExceptionMessage = exception.Message;
    }
    
    public string ExceptionMessage { get; }
}