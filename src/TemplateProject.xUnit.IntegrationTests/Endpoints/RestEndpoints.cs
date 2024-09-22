using RestSharp;
using TemplateProject.Application.Models.Auth;
using TemplateProject.Application.Models.Common;

namespace TemplateProject.xUnit.IntegrationTests.Endpoints;

public sealed class RestEndpoints()
{
    public UiApiEndpoints InternalApi => new();

    public class UiApiEndpoints()
    {
        public RestRequest Profile_GetCurrent() => 
            GetLoggedRequest(EndpointConstants.UiApi.Profile.GetCurrent, Method.Get);
        
        public RestRequest Auth_SignIn(SignInRequest request) => 
            GetLoggedRequest(EndpointConstants.UiApi.Auth.SignIn, Method.Post)
                .AddJsonBody(request);
        
        public RestRequest Auth_Register(UserRegisterRequest request) => 
            GetLoggedRequest(EndpointConstants.UiApi.Auth.Register, Method.Post)
                .AddJsonBody(request);
    }
    
    public class PublicApiEndpoints
    {
    }
    
    private static RestRequest ApplyPageFilter(RestRequest request, PagingRequest pagingRequest)
    {
        if (pagingRequest is not null)
        {
            return request
                .AddParameter(nameof(pagingRequest.PageSize), pagingRequest.PageSize)
                .AddParameter(nameof(pagingRequest.Page), pagingRequest.Page);
        }

        return request;
    }
    
    private static RestRequest GetLoggedRequest(
        string path,
        Method method)
    {
        return new RestRequest(path, method)
        {
            Interceptors =
            [
                new RestSharpLoggingInterceptor()
            ],
        };
    }
}