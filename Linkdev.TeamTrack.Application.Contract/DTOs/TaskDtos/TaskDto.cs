using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class TaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FinishDate { get; set; }
        public string ProjectName { get; set; }
        public string AssignedUserName { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
    }
}
