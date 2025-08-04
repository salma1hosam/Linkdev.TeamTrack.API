using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Service.Interfaces
{
    public interface IProjectService
    {
        Task<GenericResponse<ProjectDto>> AddProjectAsync(CreateProjectDto createProjectDto);
        Task<GenericResponse<ProjectStatusDto>> UpdateProjectStatusAsync(string userId , UpdateProjectStatus updateProjectStatus);
    }
}
