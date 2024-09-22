namespace TemplateProject.xUnit.IntegrationTests.Factory;

public sealed class IntegrationTestsAppSettings
{
    public static readonly KeyValuePair<string, string>[] Settings =
    [
        new("AuthOptions:Secret", Guid.NewGuid().ToString()),
        new("AuthOptions:AllowedOrigins:0", "http://localhost"),
        new("RateLimitOptions:IsEnabled", "false"),
    ];
}