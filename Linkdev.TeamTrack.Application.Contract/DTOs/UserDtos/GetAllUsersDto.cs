namespace Linkdev.TeamTrack.Contract.DTOs.UserDtos
{
    public class GetAllUsersDto
    {
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }
}
