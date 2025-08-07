using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto> AddProjectAsync(CreateProjectDto createProjectDto);
        Task<ProjectStatusDto> UpdateProjectStatusAsync(string userId , UpdateProjectStatusDto updateProjectStatus);
        Task<ReturnedProjectUpdateDto> UpdateProjectDetailsAsync(string userId , UpdateProjectDetailsDto updateProjectDetailsDto);
        Task<PaginatedResponse<GetAllProjectsDto>> ViewAllProjectsAsync(string userId , ProjectQueryParams projectQueryParams);
        Task<ReturnedProjectUpdateDto> AssignProjectManagerAsync(SetProjectManagerDto setProjectManagerDto);
    }
}
