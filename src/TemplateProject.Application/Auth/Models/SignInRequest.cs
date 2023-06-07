namespace TemplateProject.Application.Auth.Models;

public sealed record SignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}