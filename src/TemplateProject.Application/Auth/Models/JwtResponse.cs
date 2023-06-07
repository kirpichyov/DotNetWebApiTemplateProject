using System;

namespace TemplateProject.Application.Auth.Models;

public sealed record JwtResponse
{
    public string AccessToken { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}