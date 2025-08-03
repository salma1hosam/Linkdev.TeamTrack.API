namespace Linkdev.TeamTrack.Contract.DTOs.UserDtos
{
    public class UserQueryParams
    {
        private int pageSize = 10;
        
        public string? UserName { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > 0 && value < pageSize) ? value : pageSize; }
        }
    }
}
