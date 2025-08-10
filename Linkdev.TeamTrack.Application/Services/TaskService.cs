using AutoMapper;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Linkdev.TeamTrack.Application.Services
{
    public class TaskService(IUnitOfWork _unitOfWork , UserManager<TeamTrackUser> _userManager , IMapper _mapper) : ITaskService
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
    }
}
