using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Linkdev.TeamTrack.Application.Services
{
    public class ProjectService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<TeamTrackUser> _userManager) : IProjectService
    {
        public async Task<GenericResponse<ProjectDto>> AddProjectAsync(CreateProjectDto createProjectDto)
        {
            var genericResponse = new GenericResponse<ProjectDto>();

            if (createProjectDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter a Valid Data";
                return genericResponse;
            }

            var user = await _userManager.FindByIdAsync(createProjectDto.ProjectManagerId);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Project Manager is Not Found";
                return genericResponse;
            }

            if (_userManager.GetRolesAsync(user).Result.FirstOrDefault() != "Project Manager")
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "The selected user is not in Project Manager Role";
                return genericResponse;
            }

            var project = _mapper.Map<CreateProjectDto, Project>(createProjectDto);

            await _unitOfWork.ProjectRepository.AddAsync(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status201Created;
                genericResponse.Message = "Project Created Successfully";
                genericResponse.Data = _mapper.Map<Project, ProjectDto>(project);
                return genericResponse;
            }
            genericResponse.StatusCode = StatusCodes.Status400BadRequest;
            genericResponse.Message = "Failed to Create the Project";
            return genericResponse;
        }

        public async Task<GenericResponse<ReturnedProjectUpdateDto>> UpdateProjectDetailsAsync(string userId, UpdateProjectDetailsDto updateProjectDetailsDto)
        {
            var genericResponse = new GenericResponse<ReturnedProjectUpdateDto>();

            if (updateProjectDetailsDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter a Valid Data";
                return genericResponse;
            }

            if (userId.IsNullOrEmpty())
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User Id is Invalid";
                return genericResponse;
            }

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateProjectDetailsDto.Id && P.IsActive == true,
                                                                   nameof(Project.ProjectManager))
                                                             .FirstOrDefaultAsync();

            if (project is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Project is Not Found";
                return genericResponse;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User is Not Found";
                return genericResponse;
            }

            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();

            if (role.Contains("Project Manager") && project.ProjectManagerId != userId)
            {
                genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                genericResponse.Message = "You're Not Authorized to Update this Project";
                return genericResponse;
            }

            _mapper.Map(updateProjectDetailsDto, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Project Updated Successfully";
                genericResponse.Data = _mapper.Map<Project, ReturnedProjectUpdateDto>(project);
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status400BadRequest;
            genericResponse.Message = "Failed to Update the Project";
            return genericResponse;


        }

        public async Task<GenericResponse<ProjectStatusDto>> UpdateProjectStatusAsync(string userId, UpdateProjectStatusDto updateProjectStatusDto)
        {
            var genericResponse = new GenericResponse<ProjectStatusDto>();

            if (updateProjectStatusDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter a Valid Data";
                return genericResponse;
            }

            if (userId.IsNullOrEmpty())
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User Id is Invalid";
                return genericResponse;
            }

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateProjectStatusDto.Id &&
                                                                   P.IsActive == true &&
                                                                   P.ProjectManagerId == userId,
                                                                   nameof(Project.ProjectManager)).FirstOrDefaultAsync();

            if (project is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Project is Not Found";
                return genericResponse;
            }

            _mapper.Map(updateProjectStatusDto, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Project Status Updated Successfully";
                genericResponse.Data = _mapper.Map<Project, ProjectStatusDto>(project);
                return genericResponse;
            }
            genericResponse.StatusCode = StatusCodes.Status400BadRequest;
            genericResponse.Message = "Failed to Update the Project Status";
            return genericResponse;
        }
    }
}
