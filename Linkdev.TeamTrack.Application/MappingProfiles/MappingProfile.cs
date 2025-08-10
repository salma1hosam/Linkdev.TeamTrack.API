using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.DTOs.TaskDtos;
using Linkdev.TeamTrack.Core.Models;

namespace Linkdev.TeamTrack.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ProjectProfile();
            TaskProfile();
        }

        private void ProjectProfile()
        {
            CreateMap<CreateProjectDto, Project>();
            CreateMap<Project, ProjectDto>()
                .ForMember(dto => dto.ProjectManagerName, option => option.MapFrom(src => src.ProjectManager.UserName));
            CreateMap<UpdateProjectStatusDto, Project>();
            CreateMap<Project, ProjectStatusDto>()
                .ForMember(dto => dto.ProjectManagerName, option => option.MapFrom(src => src.ProjectManager.UserName));
            CreateMap<UpdateProjectDetailsDto, Project>();
            CreateMap<Project, ReturnedProjectUpdateDto>()
                .ForMember(dto => dto.ProjectManagerName, option => option.MapFrom(src => src.ProjectManager.UserName));
            CreateMap<SetProjectManagerDto, Project>();
        }

        private void TaskProfile()
        {
            CreateMap<CreateTaskDto, ProjectTask>();
            CreateMap<ProjectTask, TaskDto>()
                .ForMember(dto => dto.ProjectName , option => option.MapFrom(src => src.Project.Name))
                .ForMember(dto => dto.AssignedUserName, option => option.MapFrom(src => src.AssignedUser.UserName));
        }
    }
}
