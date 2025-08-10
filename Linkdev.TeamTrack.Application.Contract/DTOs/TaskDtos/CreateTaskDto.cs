using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class CreateTaskDto
    {
        [MaxLength(1000 , ErrorMessage = "Title field can Not have more than 1000 character")]
        public string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Title field can Not have more than 2000 character")]
        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime FinishDate { get; set; }
        public int ProjectId { get; set; }
        public string AssignedUserId { get; set; }
    }
}
