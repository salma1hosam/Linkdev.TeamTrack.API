using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.AuthDtos
{
    public class RegisterDto
    {
        [MaxLength(1000, ErrorMessage = "User Name can Not be more than 1000 character")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
