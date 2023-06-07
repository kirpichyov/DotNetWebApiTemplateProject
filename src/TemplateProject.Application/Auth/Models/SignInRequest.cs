namespace TemplateProject.Application.Auth.Models;

public sealed class SignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}