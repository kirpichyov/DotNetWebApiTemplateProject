using System;

namespace TemplateProject.Application.Auth.Models;

public sealed class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public Guid RefreshToken { get; set; }
}