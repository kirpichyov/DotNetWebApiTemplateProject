using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Application.Auth.Contracts;
using TemplateProject.Application.Auth.Models;
using TemplateProject.Core.Models.Api;

namespace TemplateProject.Api.Controller.v1;

[ApiVersion("1.0")]
[AllowAnonymous]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthValidatorsAggregate _authValidatorsAggregate;
    private readonly IAuthService _authService;

    public AuthController(
        IAuthValidatorsAggregate authValidatorsAggregate,
        IAuthService authService)
    {
        _authValidatorsAggregate = authValidatorsAggregate;
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAppUser([FromBody] UserRegisterRequest request)
    {
        await _authValidatorsAggregate.UserRegisterValidator.ValidateAndThrowAsync(request);
        var userCreatedResponse = await _authService.CreateUser(request);

        return StatusCode(StatusCodes.Status201Created, userCreatedResponse);
    }

    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignInAppUser([FromBody] SignInRequest request)
    {
        await _authValidatorsAggregate.SignInValidator.ValidateAndThrowAsync(request);
        var authResponse = await _authService.GenerateJwtSession(request);

        return Ok(authResponse);
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAppUser([FromBody] RefreshTokenRequest request)
    {
        await _authValidatorsAggregate.RefreshTokenValidator.ValidateAndThrowAsync(request);
        var authResponse = await _authService.RefreshJwtSession(request);

        return Ok(authResponse);
    }
}