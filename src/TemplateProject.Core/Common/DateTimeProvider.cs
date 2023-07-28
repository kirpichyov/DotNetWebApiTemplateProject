using System;
using TemplateProject.Core.Contracts;

namespace TemplateProject.Core.Common;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}