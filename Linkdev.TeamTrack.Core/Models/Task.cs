namespace Linkdev.TeamTrack.Core.Models
{
    public class Task : BaseEntity<int>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int CompletedTaskPercent { get; set; }

        public string AssignedUserId { get; set; } //FK
        public TeamTrackUser AssignedUser { get; set; }

        public int ProjectId { get; set; } //FK
        public Project Project { get; set; }
    }
}
