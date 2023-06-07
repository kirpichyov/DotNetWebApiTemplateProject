using System;

namespace TemplateProject.Application.Auth.Models;

public sealed class JwtResponse
{
    public string AccessToken { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}