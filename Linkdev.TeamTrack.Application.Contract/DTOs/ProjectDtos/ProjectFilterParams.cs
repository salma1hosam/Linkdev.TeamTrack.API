using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class ProjectFilterParams : Paging
    {
        [MaxLength(1000, ErrorMessage = "Project Name field can not have more than 1000 character")]
        public string? Name { get; set; }
        public int? ProjectStatus { get; set; }

    }
}
