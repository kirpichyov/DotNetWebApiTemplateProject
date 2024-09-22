using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TemplateProject.Application.Contracts;

namespace TemplateProject.Application.Services;

public class FluentValidatorFactory : IFluentValidatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IValidator<TModel> GetFor<TModel>() where TModel : class
    {
        return _serviceProvider.GetRequiredService<IValidator<TModel>>();
    }

    public void ValidateAndThrow<TModel>(TModel model) where TModel : class
    {
        var validator = _serviceProvider.GetService<IValidator<TModel>>();
        validator?.ValidateAndThrow(model);
    }
}