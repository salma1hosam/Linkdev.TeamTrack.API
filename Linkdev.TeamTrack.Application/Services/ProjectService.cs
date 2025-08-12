using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Exceptions;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Contract.Application.Interfaces;
using Linkdev.TeamTrack.Core.Enums;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Linkdev.TeamTrack.Application.Services
{
    public class ProjectService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<TeamTrackUser> _userManager , 
                                IEmailService _emailService) : IProjectService
    {
        public async Task<ProjectDto> AddProjectAsync(CreateProjectDto createProjectDto)
        {
            if (createProjectDto is null) throw new BadRequestException("Invalid Data");

            var user = await _userManager.FindByIdAsync(createProjectDto.ProjectManagerId)
                ?? throw new NotFoundException("Project Manager is Not Found");

            if (_userManager.GetRolesAsync(user).Result.FirstOrDefault() != "Project Manager")
                throw new BadRequestException("The selected user is not in Project Manager Role");

            var project = _mapper.Map<CreateProjectDto, Project>(createProjectDto);

            await _unitOfWork.ProjectRepository.AddAsync(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Create the Project");

            return _mapper.Map<Project, ProjectDto>(project);
        }

        public async Task<ReturnedProjectUpdateDto> AssignProjectManagerAsync(SetProjectManagerDto setProjectManagerDto)
        {
            if (setProjectManagerDto is null) throw new BadRequestException("Invalid Data");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == setProjectManagerDto.Id && P.IsActive == true)
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("Project is Not Found");

            if (project.ProjectManagerId == setProjectManagerDto.ProjectManagerId)
                throw new BadRequestException("This Project Manager is already assigned to this project");

            var user = await _userManager.FindByIdAsync(setProjectManagerDto.ProjectManagerId)
                ?? throw new NotFoundException("This Project Manager is Not Found");

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.IsNullOrEmpty() || !role.Contains("Project Manager"))
                throw new BadRequestException("The selected user is not in Project Manager Role");

            _mapper.Map(setProjectManagerDto, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Assign Project Manager to this Project");

            return _mapper.Map<Project, ReturnedProjectUpdateDto>(project);
        }

        public async Task<ReturnedProjectUpdateDto> UpdateProjectDetailsAsync(string userId, UpdateProjectDetailsDto updateProjectDetailsDto)
        {
            if (updateProjectDetailsDto is null) throw new BadRequestException("Invalid Data");

            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateProjectDetailsDto.Id && P.IsActive == true,
                                                                   nameof(Project.ProjectManager))
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("Project is Not Found");

            if (project.ProjectStatus == ProjectStatus.Suspended)
                throw new ForbiddenException("Project is suspended. Only 'Project Status' can be modified.");

            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User is Not Found");

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role.Contains("Project Manager") && project.ProjectManagerId != userId)
                throw new ForbiddenException("You're Not Authorized to Update this Project");

            _mapper.Map(updateProjectDetailsDto, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Update the Project");

            return _mapper.Map<Project, ReturnedProjectUpdateDto>(project);
        }

        public async Task<ProjectStatusDto> UpdateProjectStatusAsync(string userId, UpdateProjectStatusDto updateProjectStatusDto)
        {
            if (updateProjectStatusDto is null) throw new BadRequestException("Invalid Data");

            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateProjectStatusDto.Id && P.IsActive == true
                                                                   && P.ProjectManagerId == userId
                                                                   , nameof(Project.ProjectManager))
                                                             .FirstOrDefaultAsync()
                                                             ?? throw new NotFoundException("Project is Not Found");

            _mapper.Map(updateProjectStatusDto, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Update the Project Status");

            return _mapper.Map<Project, ProjectStatusDto>(project);
        }

        public async Task<PaginatedResponse<GetAllProjectsDto>> ViewAllProjectsAsync(string userId, ProjectQueryParams projectQueryParams)
        {
            if (userId.IsNullOrEmpty()) throw new UnauthorizedException("Invalid User Id");

            var allprojectsPaginated = await _unitOfWork.ProjectRepository
                                      .FindAsync(P => (P.ProjectManagerId.Equals(userId) || P.Tasks.Any(T => T.AssignedUserId == userId))
                                                 && P.IsActive == true
                                                 && (projectQueryParams.Name.IsNullOrEmpty() || P.Name.ToLower().Contains(projectQueryParams.Name.ToLower()))
                                                 && (!projectQueryParams.ProjectStatus.HasValue || P.ProjectStatus == (ProjectStatus)projectQueryParams.ProjectStatus.Value)
                                                 , new Paging(projectQueryParams.PageSize, projectQueryParams.PageNumber)
                                                 , P => P.CreatedDate
                                                 , "desc"
                                                 , nameof(Project.ProjectManager), nameof(Project.Tasks)
                                                 );

            //if (allprojectsPaginated.Data?.Any() == false)
            //{
            //    genericResponse.StatusCode = StatusCodes.Status200OK;
            //    genericResponse.Message = "There is no data to be displayed";
            //    return genericResponse;
            //}

            var mappedProjects = allprojectsPaginated.Data.Select(P => new GetAllProjectsDto()
            {
                Name = P.Name,
                CreatedDate = P.CreatedDate,
                ProjectManagerName = P.ProjectManager.UserName,
                ProjectStatus = P.ProjectStatus.ToString()
            }).ToList();

            return new PaginatedResponse<GetAllProjectsDto>()
            {
                TotalCount = allprojectsPaginated.TotalCount,
                PageNumber = allprojectsPaginated.PageNumber,
                PageSize = allprojectsPaginated.PageSize,
                Data = mappedProjects
            };
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == projectId && P.IsActive == true
                                                                   , nameof(Project.Tasks), nameof(Project.ProjectManager))
                                                             .FirstOrDefaultAsync() ?? throw new NotFoundException("Project is Not Found");

            project.IsActive = false;
            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 1) throw new Exception("Failed to Delete the project");

            if (project.Tasks?.Any() == true)
            {
                foreach (var task in project.Tasks)
                    task.IsActive = false;
                _unitOfWork.TaskRepository.UpdateList(project.Tasks);
                var rowsNumber = await _unitOfWork.SaveChangesAsync();
                if (rowsNumber < 1) throw new Exception("Failed to delete the Project related Tasks ");
            }

            await _emailService.SendEmailAsync(toEmail: project.ProjectManager.Email,
                                               subject: "Project Deleted",
                                               messageBody: $"{project.Name} Project has been deleted");

            return true;
        }
    }
}
