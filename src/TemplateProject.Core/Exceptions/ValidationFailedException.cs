namespace TemplateProject.Core.Exceptions;

public sealed class ValidationFailedException : CoreException
{
    public ValidationFailedException(string message)
        : base(new [] {new PropertyErrors(null, message)}, ExceptionsInfo.Identifiers.ValidationFailed)
    {
    }

    public ValidationFailedException(string propertyName, string propertyError)
        : base(new[] {new PropertyErrors(propertyName, propertyError)}, ExceptionsInfo.Identifiers.ValidationFailed)
    {
    }
}