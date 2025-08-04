using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Repository.Interfaces;
using Linkdev.TeamTrack.Contract.Service.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
    }
}
