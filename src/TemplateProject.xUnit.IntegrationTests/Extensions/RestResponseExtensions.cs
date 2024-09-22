using RestSharp;

namespace TemplateProject.xUnit.IntegrationTests.Extensions;

internal static class RestResponseExtensions
{
    public static void ThrowOnFailStatusCode(this RestResponse response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new InvalidOperationException($"Expected to receive success result from {response.Request.Method} {response.Request.Resource}. " +
                                            $"But actual status code was {response.StatusCode} with content: {response.Content}");
    }
    
    public static async Task ThrowOnFailStatusCode(this Task<RestResponse> response)
    {
        var restResponse = await response;
        restResponse.ThrowOnFailStatusCode();
    }
    
    public static async Task<T> ThrowOnFailStatusCode<T>(this Task<RestResponse<T>> response)
    {
        var restResponse = await response;
        restResponse.ThrowOnFailStatusCode();
        return restResponse.Data;
    }
}