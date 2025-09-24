using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Infrastructure.Interfaces
{
    public interface IProjectElasticService
    {
        Task IndexProjectAsync(Project project);
        Task<PaginatedResponse<GetAllProjectsDto>> SearchProjectAsync(ProjectFilterParams filterParams, string userId);
    }
}
