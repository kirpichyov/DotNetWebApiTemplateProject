using System.Threading.Tasks;
using TemplateProject.Application.Models.Profile;

namespace TemplateProject.Application.Contracts;

public interface IProfileService
{
    Task<CurrentUserProfileResponse> GetCurrentProfile();
}