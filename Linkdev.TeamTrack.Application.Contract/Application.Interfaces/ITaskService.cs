using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;

namespace Linkdev.TeamTrack.Contract.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskDto> AddTaskAsync(string userId , CreateTaskDto createTaskDto);
    }
}
