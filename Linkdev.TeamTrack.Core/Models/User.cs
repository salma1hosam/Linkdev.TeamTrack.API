using Microsoft.AspNetCore.Identity;

namespace Linkdev.TeamTrack.Core.Models
{
    public class User : IdentityUser
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<Project>? Projects { get; set; } = [];
        public ICollection<Task>? Tasks { get; set; } = [];
    }
}
