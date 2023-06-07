namespace TemplateProject.Core.Models.Api;

public class ApiErrorResponse
{
    public ApiErrorResponse(string errorType, ApiErrorResponseNode[] errorNodes)
    {
        ErrorType = errorType;
        Errors = errorNodes;
    }

    public ApiErrorResponse(string errorType, ApiErrorResponseNode errorNode)
    {
        ErrorType = errorType;
        Errors = new[] { errorNode };
    }
    
    public string ErrorType { get; init; }
    public ApiErrorResponseNode[] Errors { get; init; }
}