using AutoMapper;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Core.Models;

namespace Linkdev.TeamTrack.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProjectDto, Project>();
            CreateMap<Project , ProjectDto>()
                .ForMember(dto => dto.ProjectManagerName , option => option.MapFrom(src => src.ProjectManager.UserName));
            CreateMap<UpdateProjectStatus, Project>();
            CreateMap<Project, ProjectStatusDto>()
                .ForMember(dto => dto.ProjectManagerName, option => option.MapFrom(src => src.ProjectManager.UserName));
        }
    }
}
