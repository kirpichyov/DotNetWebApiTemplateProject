using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Application.Contracts;
using TemplateProject.Application.Models.Auth;
using TemplateProject.Core.Models.Api;

namespace TemplateProject.Api.Controller.v1;

[ApiVersion("1.0")]
[AllowAnonymous]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAppUser([FromBody] UserRegisterRequest request)
    {
        var userCreatedResponse = await _authService.CreateUser(request);

        return StatusCode(StatusCodes.Status201Created, userCreatedResponse);
    }

    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignInAppUser([FromBody] SignInRequest request)
    {
        var authResponse = await _authService.GenerateJwtSession(request);

        return Ok(authResponse);
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAppUser([FromBody] RefreshTokenRequest request)
    {
        var authResponse = await _authService.RefreshJwtSession(request);

        return Ok(authResponse);
    }
}