using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Linkdev.TeamTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IProjectService _projectService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("AddProject")]
        public async Task<ActionResult<ProjectDto>> AddProject(CreateProjectDto createProjectDto)
        {
            var result = await _projectService.AddProjectAsync(createProjectDto);
            return Ok(result);
        }


        [Authorize(Roles = "Project Manager")]
        [HttpPut("UpdateProjectStatus")]
        public async Task<ActionResult<ProjectStatusDto>> UpdateProjectStatus(UpdateProjectStatusDto updateProjectStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.UpdateProjectStatusAsync(userId , updateProjectStatus);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPut("UpdateProjectDetails")]
        public async Task<ActionResult<ReturnedProjectUpdateDto>> UpdateProjectDetails(UpdateProjectDetailsDto updateProjectDetailsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.UpdateProjectDetailsAsync(userId , updateProjectDetailsDto);
            return Ok(result);
        }

        [Authorize(Roles = "Project Manager,Team Member")]
        [HttpGet("ViewAllProjects")]
        public async Task<ActionResult<PaginatedResponse<GetAllProjectsDto>>> ViewAllProjects([FromQuery] ProjectQueryParams projectQueryParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.ViewAllProjectsAsync(userId , projectQueryParams);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("AssignProjectManager")]
        public async Task<ActionResult<ReturnedProjectUpdateDto>> AssignProjectManager(SetProjectManagerDto setProjectManagerDto)
        {
            var result = await _projectService.AssignProjectManagerAsync(setProjectManagerDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("DeleteProject")]
        public async Task<ActionResult<bool>> DeleteProject(int projectId)
        {
            var result = await _projectService.DeleteProjectAsync(projectId);
            return Ok(result);
        }
    }
}
