using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class TaskCompletePercentDto
    {
        public int Id { get; set; }
        public int CompletedTaskPercent { get; set; }
        public string ProjectStatus { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdatedDate { get; set; }
    }
}
