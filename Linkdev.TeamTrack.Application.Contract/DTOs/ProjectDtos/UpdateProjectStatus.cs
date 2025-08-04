using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class UpdateProjectStatus
    {
        public int Id { get; set; }
        
        [AllowedValues(1,2 , ErrorMessage = "You can only set the Project Status with In Progress or Suspended")]
        public int ProjectStatus { get; set; }
    }
}
