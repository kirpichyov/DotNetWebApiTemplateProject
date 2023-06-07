using Serilog.Events;

namespace TemplateProject.Core.Options;

public sealed class LoggingOptions
{
    public LogEventLevel ConsoleLogLevel { get; init; }
}