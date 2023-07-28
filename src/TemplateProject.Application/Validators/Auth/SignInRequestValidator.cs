using FluentValidation;
using TemplateProject.Application.Models.Auth;

namespace TemplateProject.Application.Validators.Auth;

public sealed class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(model => model.Email).NotEmpty();
        RuleFor(model => model.Password).NotEmpty();
    }
}