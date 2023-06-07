using System;
using TemplateProject.Application.Common.Contracts;

namespace TemplateProject.Application.Common;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}