namespace Linkdev.TeamTrack.Contract.DTOs
{
    public class Paging
    {
        private int pageSize = 10;

        public Paging() { }
        public Paging(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > 0 && value < pageSize) ? value : pageSize; }
        }
    }
}
