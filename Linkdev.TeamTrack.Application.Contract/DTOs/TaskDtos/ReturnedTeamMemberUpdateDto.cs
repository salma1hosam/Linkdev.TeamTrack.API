using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class ReturnedTeamMemberUpdateDto
    {
        public int Id { get; set; }
        public string AssignedUserId { get; set; }
        public string UserName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdatedDate { get; set; }
    }
}
