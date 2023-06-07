using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateProject.Core.Exceptions;

public class CoreException : Exception
{
    protected CoreException(string message, string identifier)
        : base(message)
    {
        Identifier = identifier;
        PropertyErrors = Array.Empty<PropertyErrors>();
    }

    protected CoreException(IEnumerable<PropertyErrors> propertyErrors, string identifier)
        : base("Validation failed.")
    {
        Identifier = identifier;
        PropertyErrors = propertyErrors.ToArray();
    }

    public string Identifier { get; }
    public PropertyErrors[] PropertyErrors { get; }
}