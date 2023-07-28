using Flurl.Http;
using TemplateProject.Application.Models.Auth;

namespace TemplateProject.IntegrationTests.Testing;

public sealed record AuthorizedClient(IFlurlClient FlurlClient, UserRegisterRequest UserInfo, AuthResponse AuthInfo);