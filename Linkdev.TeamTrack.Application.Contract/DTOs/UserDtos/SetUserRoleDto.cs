using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.UserDtos
{
    public class SetUserRoleDto
    {
        public string UserId { get; set; }

        [AllowedValues("Admin", "Project Manager", "Team Member", ErrorMessage = "Role is Not Allowed")]
        public string Role { get; set; }
    }
}
