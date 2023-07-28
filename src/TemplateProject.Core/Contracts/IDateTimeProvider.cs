using System;

namespace TemplateProject.Core.Contracts;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}