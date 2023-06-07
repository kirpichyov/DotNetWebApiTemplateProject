using FluentValidation;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.Application.Auth.Validators;

public sealed class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(model => model.Email).NotEmpty();
        RuleFor(model => model.Password).NotEmpty();
    }
}