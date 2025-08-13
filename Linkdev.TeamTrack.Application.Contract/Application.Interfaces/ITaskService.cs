using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Linkdev.TeamTrack.Core.Responses;

namespace Linkdev.TeamTrack.Contract.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> AddTaskAsync(string userId, CreateTaskDto createTaskDto);
        Task<ReturnedTaskUpdateDto> UpdateTaskDetailsAsync(string userId, UpdateTaskDetailsDto updateTaskDetailsDto);
        Task<string> DeleteTaskAsync(string userId, int taskId);
        Task<ReturnedTeamMemberUpdateDto> AssignTeamMemberOnTaskAsync(string userId, SetTeamMemberDto setTeamMemberDto);
        Task<PaginatedResponse<GetAllTasksDto>> ViewAllTasksAsync(string userId, int projectId, TaskFilterParams taskQueryParams);
        Task<TaskCompletePercentDto> UpdateTaskCompletePercentAsync(string userId, UpdateTaskCompletePercentDto updateTaskCompletePercentDto);
    }
}
