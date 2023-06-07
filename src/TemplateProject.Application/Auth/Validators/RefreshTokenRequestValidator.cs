using FluentValidation;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.Application.Auth.Validators;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.AccessToken).NotEmpty();
        RuleFor(request => request.RefreshToken).NotEmpty();
    }
}
