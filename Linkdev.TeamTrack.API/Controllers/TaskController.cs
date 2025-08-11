using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Linkdev.TeamTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService _taskService) : ControllerBase
    {
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost("AddTask")]
        public async Task<ActionResult<TaskDto>> AddTask(CreateTaskDto createTaskDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _taskService.AddTaskAsync(userId, createTaskDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPut("UpdateTaskDetails")]
        public async Task<ActionResult<ReturnedTaskUpdateDto>> UpdateTaskDetails(UpdateTaskDetailsDto updateTaskDetailsDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _taskService.UpdateTaskDetailsAsync(userId, updateTaskDetailsDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPut("AssignTeamMemberOnTask")]
        public async Task<ActionResult<ReturnedTeamMemberUpdateDto>> AssignTeamMemberOnTask(SetTeamMemberDto setTeamMemberDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _taskService.AssignTeamMemberOnTaskAsync(userId, setTeamMemberDto);
            return Ok(result);
        }
    }
}
