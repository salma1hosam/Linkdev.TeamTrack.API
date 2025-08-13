using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkdev.TeamTrack.Core.Models
{
    public class ProjectTask : BaseEntity<int>
    {
        [MaxLength(1000)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }

        [Range(0, 100)]
        public int CompletedTaskPercent { get; set; } = 0;

        [ForeignKey(nameof(AssignedUser))]
        public string AssignedUserId { get; set; } = string.Empty; //FK

        [InverseProperty(nameof(TeamTrackUser.Tasks))]
        public TeamTrackUser AssignedUser { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; } //FK

        [InverseProperty(nameof(Project.Tasks))]
        public Project Project { get; set; }
    }
}
