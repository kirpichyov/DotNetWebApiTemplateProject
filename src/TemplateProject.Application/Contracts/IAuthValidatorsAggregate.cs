using FluentValidation;
using TemplateProject.Application.Models.Auth;

namespace TemplateProject.Application.Contracts;

public interface IAuthValidatorsAggregate
{
    public IValidator<SignInRequest> SignInValidator { get; }
    public IValidator<RefreshTokenRequest> RefreshTokenValidator { get; }
    public IValidator<UserRegisterRequest> UserRegisterValidator { get; }
}