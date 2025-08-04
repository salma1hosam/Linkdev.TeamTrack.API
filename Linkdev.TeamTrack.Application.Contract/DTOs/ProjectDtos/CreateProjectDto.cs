using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class CreateProjectDto
    {
        [MaxLength(1000 , ErrorMessage = "Project Name field can not be more than 1000 character")]
        public string Name { get; set; }

        [MaxLength(2000 , ErrorMessage = "Description field can not be more than 2000 character")]
        public string? Description { get; set; }
        public string ProjectManagerId { get; set; }
        
        //public ProjectStatus ProjectStatus { get; set; }
    }
}
