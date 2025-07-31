namespace Linkdev.TeamTrack.Contract.DTOs.AuthDtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; } = "Unasiigned";
        public DateTime CreatedDate { get; set; }
        public string Token { get; set; }
    }
}
