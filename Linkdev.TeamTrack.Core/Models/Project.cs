using Linkdev.TeamTrack.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkdev.TeamTrack.Core.Models
{
    public class Project : BaseEntity<int>
    {
        [MaxLength(1000)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(2000)]
        public string? Description { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        
        [ForeignKey(nameof(ProjectManager))]
        public string ProjectManagerId { get; set; } = string.Empty; //FK

        [InverseProperty(nameof(TeamTrackUser.Projects))]
        public TeamTrackUser ProjectManager { get; set; }

        [InverseProperty(nameof(ProjectTask.Project))]
        public ICollection<ProjectTask> Tasks { get; set; } = [];
    }
}
