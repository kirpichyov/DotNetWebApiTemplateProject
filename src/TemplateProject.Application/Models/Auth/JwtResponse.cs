using System;

namespace TemplateProject.Application.Models.Auth;

public sealed record JwtResponse
{
    public string AccessToken { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
}