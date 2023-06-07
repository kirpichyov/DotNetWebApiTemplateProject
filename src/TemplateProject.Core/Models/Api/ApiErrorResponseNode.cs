namespace TemplateProject.Core.Models.Api;

public sealed class ApiErrorResponseNode
{
    public ApiErrorResponseNode(string property, string[] messages)
    {
        Property = property;
        Messages = messages;
    }

    public ApiErrorResponseNode(string property, string message)
    {
        Property = property;
        Messages = new[] { message };
    }

    public string Property { get; }
    public string[] Messages { get; }
}