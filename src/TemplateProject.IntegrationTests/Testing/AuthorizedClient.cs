using Flurl.Http;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.IntegrationTests.Testing;

public sealed record AuthorizedClient(IFlurlClient FlurlClient, UserRegisterRequest UserInfo, AuthResponse AuthInfo);