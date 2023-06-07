using FluentValidation;
using TemplateProject.Application.Auth.Models;

namespace TemplateProject.Application.Auth.Contracts;

public interface IAuthValidatorsAggregate
{
    public IValidator<SignInRequest> SignInValidator { get; }
    public IValidator<RefreshTokenRequest> RefreshTokenValidator { get; }
    public IValidator<UserRegisterRequest> UserRegisterValidator { get; }
}