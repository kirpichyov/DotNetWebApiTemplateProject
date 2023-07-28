using System;
using System.Text.RegularExpressions;
using FluentValidation;

namespace TemplateProject.Application.Extensions;

public static class ValidationExtensions
{
    public static void ApplyPasswordValidationRules<TModel>(this IRuleBuilderInitial<TModel, string> initial)
    {
        initial.Cascade(CascadeMode.Stop);
        
        initial
            .IsRequired()
            .Min(8)
            .Max(32);

        initial
            .Must(password => !password.Contains(' '))
            .WithMessage("Can't contain a whitespace")
            .Must(password => Regex.IsMatch(password, @"(?=.*[\W_])"))
            .WithMessage("Must have at least 1 special character")
            .Must(password => Regex.IsMatch(password, @"(?=.*\d)"))
            .WithMessage("Must have at least 1 number")
            .Must(password => Regex.IsMatch(password, @"(?=.*[A-Z])"))
            .WithMessage("Must have at least 1 upper case character");
    }

    public static IRuleBuilder<TModel, string> IsRequired<TModel>(this IRuleBuilderInitial<TModel, string> initial)
    {
        return initial.NotEmpty().WithMessage("Can't be empty");
    }
    
    public static IRuleBuilder<TModel, Guid> IsRequired<TModel>(this IRuleBuilderInitial<TModel, Guid> initial)
    {
        return initial.NotEmpty().WithMessage("Can't be empty.");
    }
    
    public static IRuleBuilder<TModel, string> Max<TModel>(this IRuleBuilder<TModel, string> initial, int maxLength)
    {
        return initial.MaximumLength(maxLength).WithMessage($"Can't be longer than {maxLength} characters.");
    }
    
    public static IRuleBuilder<TModel, string> Min<TModel>(this IRuleBuilder<TModel, string> initial, int minLength)
    {
        return initial.MinimumLength(minLength).WithMessage($"Can't be shorter than {minLength} characters.");
    }
    
    public static IRuleBuilder<TModel, uint> IsGreaterThan<TModel>(this IRuleBuilder<TModel, uint> initial, uint value)
    {
        return initial.GreaterThan(value).WithMessage($"Must be greater than {value}.");
    }
    
    public static IRuleBuilder<TModel, uint> IsLessThan<TModel>(this IRuleBuilder<TModel, uint> initial, uint value)
    {
        return initial.LessThan(value).WithMessage($"Must be less than {value}.");
    }
}