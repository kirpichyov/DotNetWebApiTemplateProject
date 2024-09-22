using FluentValidation;

namespace TemplateProject.Application.Contracts;

public interface IFluentValidatorFactory
{
    IValidator<TModel> GetFor<TModel>()
        where TModel : class;
    
    void ValidateAndThrow<TModel>(TModel model)
        where TModel : class;
}