using System.Threading.Tasks;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.Application.Auth.Contracts;

public interface IAuthService
{
    Task<UserCreatedResponse> CreateUser(UserRegisterRequest request);
    Task<AuthResponse> GenerateJwtSession(SignInRequest request);
    Task<AuthResponse> RefreshJwtSession(RefreshTokenRequest request);
}