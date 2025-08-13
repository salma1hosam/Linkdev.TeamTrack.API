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
        public async Task<IActionResult> AddProject(CreateProjectDto createProjectDto)
        {
            var result = await _projectService.AddProjectAsync(createProjectDto);
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("AssignProjectManager")]
        public async Task<IActionResult> AssignProjectManager(SetProjectManagerDto setProjectManagerDto)
        {
            var result = await _projectService.AssignProjectManagerAsync(setProjectManagerDto);
            return Ok(result);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpPut("UpdateProjectStatus")]
        public async Task<IActionResult> UpdateProjectStatus(UpdateProjectStatusDto updateProjectStatus)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.UpdateProjectStatusAsync(userId, updateProjectStatus);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPut("UpdateProjectDetails")]
        public async Task<IActionResult> UpdateProjectDetails(UpdateProjectDetailsDto updateProjectDetailsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.UpdateProjectDetailsAsync(userId, updateProjectDetailsDto);
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("DeleteProject/{projectId}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var result = await _projectService.DeleteProjectAsync(projectId);
            return Ok(result);
        }

        [Authorize(Roles = "Project Manager,Team Member")]
        [HttpPost("ViewAllProjects")]
        public async Task<IActionResult> ViewAllProjects(ProjectFilterParams projectFilterParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _projectService.ViewAllProjectsAsync(userId, projectFilterParams);
            return Ok(result);
        }
    }
}
