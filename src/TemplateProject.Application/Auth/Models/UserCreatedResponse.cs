using System;

namespace TemplateProject.Application.Auth.Models;

public sealed class UserCreatedResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}