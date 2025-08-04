using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
