namespace TemplateProject.Application.Auth.Models;

public sealed record UserRegisterRequest
{
    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}