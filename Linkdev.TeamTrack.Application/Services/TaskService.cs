using AutoMapper;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Contract.DTOs;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Linkdev.TeamTrack.Application.Services
{
    public class TaskService(IUnitOfWork _unitOfWork, UserManager<TeamTrackUser> _userManager, IMapper _mapper) : ITaskService
    {
        public async Task<TaskDto> AddTaskAsync(string userId, CreateTaskDto createTaskDto)
        {
            if (createTaskDto is null) throw new BadRequestException("Invalid Data");
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User is Not Found");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == createTaskDto.ProjectId && P.IsActive == true)
                                                             .FirstOrDefaultAsync() ?? throw new NotFoundException("Project is Not Found");

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.Contains("Project Manager") && project.ProjectManagerId != userId)
                throw new ForbiddenException("You're Not authorized to create task under this project");

            if (createTaskDto.FinishDate <= createTaskDto.StartDate)
                throw new BadRequestException("Finish Date should be greater than Start Date");

            var teamMember = await _userManager.FindByIdAsync(createTaskDto.AssignedUserId)
                ?? throw new NotFoundException("The assigned team member user is Not Found");

            var task = _mapper.Map<CreateTaskDto, ProjectTask>(createTaskDto);

            await _unitOfWork.TaskRepository.AddAsync(task);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Create the Task");

            return _mapper.Map<ProjectTask, TaskDto>(task);
        }

        public async Task<ReturnedTaskUpdateDto> UpdateTaskDetailsAsync(string userId, UpdateTaskDetailsDto updateTaskDetailsDto)
        {
            if (updateTaskDetailsDto is null) throw new BadRequestException("Invalid Data");
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User is Not Found");

            var task = await _unitOfWork.TaskRepository.Find(T => T.Id == updateTaskDetailsDto.Id && T.IsActive == true
                                                             , nameof(ProjectTask.Project))
                                                       .FirstOrDefaultAsync() ?? throw new NotFoundException("Task is Not Found");

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.Contains("Project Manager") && task.Project.ProjectManagerId != userId)
                throw new ForbiddenException("You're Not authorized to update this task");

            if (updateTaskDetailsDto.FinishDate <= updateTaskDetailsDto.StartDate)
                throw new BadRequestException("Finish Date should be greater than Start Date");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateTaskDetailsDto.ProjectId && P.IsActive == true)
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("The selected project is Not Found");

            _mapper.Map(updateTaskDetailsDto, task);
            task.LastUpdatedDate = DateTime.Now;

            _unitOfWork.TaskRepository.Update(task);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Update the Task");

            return _mapper.Map<ProjectTask, ReturnedTaskUpdateDto>(task);
        }

        public async Task<ReturnedTeamMemberUpdateDto> AssignTeamMemberOnTaskAsync(string userId, SetTeamMemberDto setTeamMemberDto)
        {
            if (setTeamMemberDto is null) throw new BadRequestException("Invalid Data");
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var task = await _unitOfWork.TaskRepository.Find(T => T.Id == setTeamMemberDto.Id && T.IsActive == true
                                                             , nameof(ProjectTask.AssignedUser), nameof(ProjectTask.Project))
                                                       .FirstOrDefaultAsync() ?? throw new NotFoundException("Task is Not Found");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User is Not Found");

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.Contains("Project Manager") && task.Project.ProjectManagerId != userId)
                throw new ForbiddenException("You're Not authorized to assign team member on this project task");

            if (task.AssignedUserId == setTeamMemberDto.AssignedUserId)
                throw new BadRequestException("This Team Member is already assigned to this task");

            var teamMember = await _userManager.FindByIdAsync(setTeamMemberDto.AssignedUserId)
                ?? throw new NotFoundException("Assigned Team Member is Not Found");

            _mapper.Map(setTeamMemberDto, task);
            task.LastUpdatedDate = DateTime.Now;

            _unitOfWork.TaskRepository.Update(task);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to assign this Team Member to the task");

            return _mapper.Map<ProjectTask, ReturnedTeamMemberUpdateDto>(task);
        }

        public async Task<PaginatedResponse<GetAllTasksDto>> ViewAllTasksAsync(string userId, int projectId, TaskFilterParams taskFilterParams)
        {
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid UserId");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == projectId && P.IsActive == true)
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("Project is Not Found");

            if (!project.ProjectManagerId.Equals(userId) && project.Tasks?.Any(PT => PT.AssignedUserId.Equals(userId)) == false)
                throw new ForbiddenException("You're Not authorized to view this project or its tasks");

            var allTasksPaginated = await _unitOfWork.TaskRepository
                                   .FindAsync(T => T.IsActive == true
                                              && T.ProjectId == projectId
                                              && (taskFilterParams.Title.IsNullOrEmpty() || T.Title.ToLower().Contains(taskFilterParams.Title.ToLower()))
                                              && (taskFilterParams.AssignedUserId.IsNullOrEmpty() || T.AssignedUserId.Equals(taskFilterParams.AssignedUserId))
                                              , new Paging(taskFilterParams.PageSize, taskFilterParams.PageNumber)
                                              , T => T.CreatedDate
                                              , "desc"
                                              );

            var mappedTasks = allTasksPaginated.Data.Select(T => new GetAllTasksDto()
            {
                Title = T.Title,
                AssignedUserId = T.AssignedUserId,
                StartDate = T.StartDate,
                FinishDate = T.FinishDate,
                CompletedTaskPercent = T.CompletedTaskPercent
            }).ToList();

            return new PaginatedResponse<GetAllTasksDto>()
            {
                TotalCount = allTasksPaginated.TotalCount,
                PageNumber = allTasksPaginated.PageNumber,
                PageSize = allTasksPaginated.PageSize,
                Data = mappedTasks
            };
        }
    }
}
