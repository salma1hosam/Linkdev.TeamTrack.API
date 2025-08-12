namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class GetAllTasksDto
    {
        public string Title { get; set; }
        public string AssignedUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int CompletedTaskPercent { get; set; }
    }
}
