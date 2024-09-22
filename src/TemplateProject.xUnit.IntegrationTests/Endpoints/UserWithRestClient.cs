using RestSharp;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.xUnit.IntegrationTests.Endpoints;

public sealed class UserWithRestClient
{
    public User User { get; init; }
    public RestClient RestClient { get; init; }
}