using System.Net.Mail;
using FluentValidation;
using TemplateProject.Application.Auth.Models;
using TemplateProject.Application.Common.Extensions;

namespace TemplateProject.Application.Auth.Validators;

public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
{
    public UserRegisterRequestValidator()
    {
        RuleFor(model => model.Firstname)
            .IsRequired()
            .MaximumLength(100);

        RuleFor(model => model.Lastname)
            .IsRequired()
            .MaximumLength(100);
        
        RuleFor(model => model.Email)
            .IsRequired()
            .Must(email => MailAddress.TryCreate(email, out _))
            .WithMessage("Has invalid format.");
        
        RuleFor(model => model.Password).ApplyPasswordValidationRules();
    }
}