using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Linkdev.TeamTrack.Application.Services
{
    public class ProjectService(IUnitOfWork _unitOfWork, IMapper _mapper, UserManager<TeamTrackUser> _userManager) : IProjectService
    {
        public async Task<GenericResponse<ProjectDto>> AddProjectAsync(CreateProjectDto createProjectDto)
        {
            var genericReponse = new GenericResponse<ProjectDto>();

            if (createProjectDto is null)
            {
                genericReponse.StatusCode = StatusCodes.Status400BadRequest;
                genericReponse.Message = "Enter a Valid Data";
                return genericReponse;
            }

            var user = await _userManager.FindByIdAsync(createProjectDto.ProjectManagerId);
            if (user is null)
            {
                genericReponse.StatusCode = StatusCodes.Status404NotFound;
                genericReponse.Message = "Project Manager is Not Found";
                return genericReponse;
            }

            if (_userManager.GetRolesAsync(user).Result.FirstOrDefault() != "Project Manager")
            {
                genericReponse.StatusCode = StatusCodes.Status400BadRequest;
                genericReponse.Message = "The selected user is not in Project Manager Role";
                return genericReponse;
            }

            var project = _mapper.Map<CreateProjectDto, Project>(createProjectDto);

            await _unitOfWork.ProjectRepository.AddAsync(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if (rows < 0)
            {
                genericReponse.StatusCode = StatusCodes.Status400BadRequest;
                genericReponse.Message = "Failed to Create the Project";
                return genericReponse;
            }

            var mappedProject = _mapper.Map<Project, ProjectDto>(project);

            genericReponse.StatusCode = StatusCodes.Status201Created;
            genericReponse.Message = "Project Created Successfully";
            genericReponse.Data = mappedProject;
            return genericReponse;
        }

        public async Task<GenericResponse<ProjectStatusDto>> UpdateProjectStatusAsync(string userId , UpdateProjectStatus updateProjectStatus)
        {
            var genericReponse = new GenericResponse<ProjectStatusDto>();

            if(updateProjectStatus is null)
            {
                genericReponse.StatusCode = StatusCodes.Status400BadRequest;
                genericReponse.Message = "Enter a Valid Data";
                return genericReponse;
            }

            if(userId is null)
            {
                genericReponse.StatusCode = StatusCodes.Status404NotFound;
                genericReponse.Message = "User Id is Not Found";
                return genericReponse;
            }

            var project = await _unitOfWork.ProjectRepository.Find(P => P.Id == updateProjectStatus.Id &&
                                                                   P.IsActive == true &&
                                                                   P.ProjectManagerId == userId ,
                                                                   nameof(Project.ProjectManager)).FirstOrDefaultAsync();

            if(project is null)
            {
                genericReponse.StatusCode = StatusCodes.Status404NotFound;
                genericReponse.Message = "Project is Not Found";
                return genericReponse;
            }

            _mapper.Map(updateProjectStatus, project);
            project.LastUpdatedDate = DateTime.Now;

            _unitOfWork.ProjectRepository.Update(project);
            var rows = await _unitOfWork.SaveChangesAsync();
            if(rows < 0)
            {
                genericReponse.StatusCode = StatusCodes.Status400BadRequest;
                genericReponse.Message = "Failed to Update the Project Status";
                return genericReponse;
            }

            genericReponse.StatusCode = StatusCodes.Status200OK;
            genericReponse.Message = "Project Status Updated Successfully";
            genericReponse.Data = _mapper.Map<Project , ProjectStatusDto>(project);
            return genericReponse;
        }
    }
}
