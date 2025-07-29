using Linkdev.TeamTrack.Core.Enums;

namespace Linkdev.TeamTrack.Core.Models
{
    public class Project : BaseEntity<int>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public string ProjectManagerId { get; set; } //FK
        public User ProjectManager { get; set; }
        public ICollection<Task> Tasks { get; set; } = [];
    }
}
