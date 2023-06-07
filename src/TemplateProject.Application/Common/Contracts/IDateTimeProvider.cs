using System;

namespace TemplateProject.Application.Common.Contracts;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}