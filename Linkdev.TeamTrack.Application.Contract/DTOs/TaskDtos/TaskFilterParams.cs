using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class TaskFilterParams : Paging
    {
        [MaxLength(1000, ErrorMessage = "Title field can Not have more than 1000 character")]
        public string? Title { get; set; }
        public string? AssignedUserId { get; set; }
    }
}
