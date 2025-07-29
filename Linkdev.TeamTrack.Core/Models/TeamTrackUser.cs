using Microsoft.AspNetCore.Identity;

namespace Linkdev.TeamTrack.Core.Models
{
    public class TeamTrackUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public ICollection<Project>? Projects { get; set; } = [];
        public ICollection<Task>? Tasks { get; set; } = [];
    }
}
