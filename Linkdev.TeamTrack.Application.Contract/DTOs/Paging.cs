namespace Linkdev.TeamTrack.Contract.DTOs
{
    public class Paging
    {
        private int pageSize = 10;
        private int pageNumber = 1;

        public Paging() { }
        public Paging(int pageSize, int pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = (value > 0) ? value : pageNumber;
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > 0 && value < pageSize) ? value : pageSize;
        }
    }
}
