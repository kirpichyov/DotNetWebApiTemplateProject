namespace TemplateProject.Core.Options;

public sealed class SeqLoggingOptions
{
    public bool Enabled { get; init; }
    public string ServerUrl { get; init; }
    public string ApiKey { get; init; }
}