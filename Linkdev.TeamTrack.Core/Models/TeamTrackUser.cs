using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkdev.TeamTrack.Core.Models
{
    public class TeamTrackUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        [InverseProperty(nameof(Project.ProjectManager))]
        public ICollection<Project>? Projects { get; set; } = [];

        [InverseProperty(nameof(ProjectTask.AssignedUser))]
        public ICollection<ProjectTask>? Tasks { get; set; } = [];
    }
}
