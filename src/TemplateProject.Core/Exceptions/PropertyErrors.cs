namespace TemplateProject.Core.Exceptions;

public sealed class PropertyErrors
{
    public PropertyErrors(string property, string[] errors)
    {
        Property = property;
        Errors = errors;
    }

    public PropertyErrors(string property, string error)
    {
        Property = property;
        Errors = new[] {error};
    }

    public string Property { get; init; }
    public string[] Errors { get; init; }
}