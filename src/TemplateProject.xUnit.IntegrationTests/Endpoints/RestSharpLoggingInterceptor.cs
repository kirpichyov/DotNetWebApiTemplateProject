using RestSharp.Interceptors;

namespace TemplateProject.xUnit.IntegrationTests.Endpoints;

public sealed class RestSharpLoggingInterceptor : Interceptor
{
    public override async ValueTask AfterHttpRequest(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        if (responseMessage.RequestMessage is null)
        {
            return;
        }
        
        var method = responseMessage.RequestMessage.Method;
        var path = responseMessage.RequestMessage.RequestUri?.ToString();
        var statusCode = (int)responseMessage.StatusCode;
        
        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        
        var logMessage = $"[RestSharp] {method.ToString().ToUpperInvariant()} {path} responded with code {statusCode}. " +
                         $"Content: {content}";

        Console.WriteLine($"[{DateTime.UtcNow:T} INF] {logMessage}");
    }
}