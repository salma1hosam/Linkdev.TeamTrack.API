using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class UpdateTaskCompletePercentDto
    {
        public int Id { get; set; }
        
        [Range(0, 100)]
        public int CompletedTaskPercent { get; set; }
    }
}
