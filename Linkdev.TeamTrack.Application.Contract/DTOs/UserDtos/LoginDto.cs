using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.UserDtos
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }

        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
