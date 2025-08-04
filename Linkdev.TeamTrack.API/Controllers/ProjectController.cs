using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
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
        public async Task<ActionResult<GenericResponse<ProjectDto>>> AddProject(CreateProjectDto createProjectDto)
        {
            var result = await _projectService.AddProjectAsync(createProjectDto);
            return Ok(result);
        }


        [Authorize(Roles = "Project Manager")]
        [HttpPut("UpdateProjectStatus")]
        public async Task<ActionResult<GenericResponse<ProjectDto>>> UpdateProjectStatus(UpdateProjectStatus updateProjectStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.UpdateProjectStatusAsync(userId , updateProjectStatus);
            return Ok(result);
        }
    }
}
