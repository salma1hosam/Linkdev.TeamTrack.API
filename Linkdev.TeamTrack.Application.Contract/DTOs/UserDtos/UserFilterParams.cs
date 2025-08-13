using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs.UserDtos
{
    public class UserFilterParams : Paging
    {
        [MaxLength(1000, ErrorMessage = "User Name can Not be more than 1000 character")]
        public string? UserName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDateFrom { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDateTo { get; set; }
    }
}
