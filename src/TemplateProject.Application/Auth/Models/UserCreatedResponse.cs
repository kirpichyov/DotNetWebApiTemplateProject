using System;

namespace TemplateProject.Application.Auth.Models;

public sealed record UserCreatedResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}