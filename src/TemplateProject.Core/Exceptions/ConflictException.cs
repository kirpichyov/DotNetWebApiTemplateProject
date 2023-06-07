using System.Collections.Generic;

namespace TemplateProject.Core.Exceptions;

public sealed class ConflictException : CoreException
{
    public ConflictException(string message)
        : base(new []{ new PropertyErrors(null, message) }, ExceptionsInfo.Identifiers.Conflict)
    {
    }

    public ConflictException(IEnumerable<PropertyErrors> propertyErrors)
        : base(propertyErrors, ExceptionsInfo.Identifiers.Conflict)
    {
    }
}