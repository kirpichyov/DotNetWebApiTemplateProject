﻿using System.Threading.Tasks;
using TemplateProject.Application.Models.Auth;

namespace TemplateProject.Application.Contracts;

public interface IAuthService
{
    Task<UserCreatedResponse> CreateUser(UserRegisterRequest request);
    Task<AuthResponse> GenerateJwtSession(SignInRequest request);
    Task<AuthResponse> RefreshJwtSession(RefreshTokenRequest request);
}