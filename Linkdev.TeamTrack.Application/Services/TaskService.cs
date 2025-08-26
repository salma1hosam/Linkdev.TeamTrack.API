using AutoMapper;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Contract.DTOs;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Core.Enums;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Linkdev.TeamTrack.Application.Services
{
    public class TaskService(IUnitOfWork _unitOfWork, UserManager<TeamTrackUser> _userManager, 
                             IMapper _mapper, IEmailService _emailService) : ITaskService
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

            if (task.Project.ProjectStatus == ProjectStatus.Suspended)
                throw new ForbiddenException("Cannot update the task because the project is suspended");

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

        public async Task<string> DeleteTaskAsync(string userId, int taskId)
        {
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var task = await _unitOfWork.TaskRepository
                      .FindWithIncludeExp(T => T.Id == taskId
                                          ,T => T.Include(x => x.Project).ThenInclude(P => P.ProjectManager)
                                          ,T => T.Include(x => x.AssignedUser))
                      .FirstOrDefaultAsync() ?? throw new NotFoundException("Task is Not Found");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User is Not Found");
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.Contains("Project Manager") && task.Project.ProjectManagerId != userId)
                throw new ForbiddenException("You're Not authorized to delete this task");

            task.IsActive = false;
            task.LastUpdatedDate = DateTime.Now;
            _unitOfWork.TaskRepository.Update(task);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to delete this task");

            await _emailService.SendEmailAsync(toEmails: [task.Project.ProjectManager.Email , task.AssignedUser.Email]
                                               ,subject: "Task Deleted"
                                               ,messageBody: $"{task.Title} Task under {task.Project.Name} Project has been deleted");
            
            return $"Task {taskId} is successfully deleted";
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

            var assignedUser = await _userManager.FindByIdAsync(setTeamMemberDto.AssignedUserId)
                ?? throw new NotFoundException("Assigned Team Member is Not Found");

            var assignedUserRole = _userManager.GetRolesAsync(assignedUser).Result.FirstOrDefault() 
                ?? throw new BadRequestException("This User has No Role");

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

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == projectId && P.IsActive == true
                                                                   ,nameof(Project.Tasks))
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

            var paginatedResponse = new PaginatedResponse<GetAllTasksDto>
            {
                TotalCount = allTasksPaginated.TotalCount,
                PageNumber = allTasksPaginated.PageNumber,
                PageSize = allTasksPaginated.PageSize
            };

            if (allTasksPaginated.Data?.Any() == false)
            {
                paginatedResponse.Message = "There is no data to be displayed";
                return paginatedResponse;
            }

            var mappedTasks = allTasksPaginated.Data.Select(T => new GetAllTasksDto()
            {
                Title = T.Title,
                AssignedUserId = T.AssignedUserId,
                StartDate = T.StartDate,
                FinishDate = T.FinishDate,
                CompletedTaskPercent = T.CompletedTaskPercent
            }).ToList();

            paginatedResponse.Data = mappedTasks;
            return paginatedResponse;
        }

        public async Task<TaskCompletePercentDto> UpdateTaskCompletePercentAsync(string userId, UpdateTaskCompletePercentDto updateTaskCompletePercentDto)
        {
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");
            if (updateTaskCompletePercentDto is null) throw new BadRequestException("Invalid Data");

            var task = await _unitOfWork.TaskRepository.Find(T => T.Id == updateTaskCompletePercentDto.Id
                                                             && T.IsActive == true
                                                             && T.AssignedUserId.Equals(userId)
                                                             , nameof(ProjectTask.Project))
                                                       .FirstOrDefaultAsync() ?? throw new NotFoundException("Task is Not Found");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == task.ProjectId && P.IsActive == true
                                                                   , nameof(Project.Tasks))
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("Project is Not Found");

            if (project.ProjectStatus == ProjectStatus.Suspended)
                throw new ForbiddenException("Cannot update the task % complete because the project is suspended");

            _mapper.Map(updateTaskCompletePercentDto, task);
            task.LastUpdatedDate = DateTime.Now;
            _unitOfWork.TaskRepository.Update(task);

            if (project.Tasks.Where(T => T.IsActive == true).All(T => T.CompletedTaskPercent == 100))
                project.ProjectStatus = ProjectStatus.Completed;
            else
                project.ProjectStatus = ProjectStatus.InProgress;

            _unitOfWork.ProjectRepository.Update(project);

            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to update the task % complete or the project status");

            return _mapper.Map<ProjectTask, TaskCompletePercentDto>(task);
        }
    }
}
