using System;

namespace TemplateProject.Application.Auth.Models;

public sealed class AuthResponse
{
    public JwtResponse Jwt { get; set; }
    public RefreshTokenResponse RefreshToken { get; set; }
    public Guid UserId { get; set; }
}