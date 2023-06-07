using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Application.Profile.Contracts;
using TemplateProject.Application.Profile.Models;

namespace TemplateProject.Api.Controller.v1;

[ApiVersion("1.0")]
public sealed class ProfileController : ApiControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("current")]
    [ProducesResponseType(typeof(CurrentUserProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentProfile()
    {
        var response = await _profileService.GetCurrentProfile();
        return Ok(response);
    }
}