using System.Threading.Tasks;
using TemplateProject.Application.Profile.Models;

namespace TemplateProject.Application.Profile.Contracts;

public interface IProfileService
{
    Task<CurrentUserProfileResponse> GetCurrentProfile();
}