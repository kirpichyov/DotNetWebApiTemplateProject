using System;

namespace TemplateProject.Application.Auth.Models;

public sealed record RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public Guid RefreshToken { get; set; }
}