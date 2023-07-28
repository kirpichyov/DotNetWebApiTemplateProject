namespace TemplateProject.Application.Models.Auth;

public sealed record SignInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}