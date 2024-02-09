using FluentValidation;
using TemplateProject.Application.Extensions;
using TemplateProject.Application.Models.UserNamespaces;

namespace TemplateProject.Application.Validators.UserNamespaces;

public sealed class CreateNamespaceRequestValidator : AbstractValidator<CreateNamespaceRequest>
{
    public CreateNamespaceRequestValidator()
    {
        RuleFor(model => model.Name)
            .IsRequired()
            .MaximumLength(64);
    }
}