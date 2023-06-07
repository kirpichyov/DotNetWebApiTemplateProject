using System;

namespace TemplateProject.Application.Auth.Models;

public sealed class RefreshTokenResponse
{
    public string Token { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}