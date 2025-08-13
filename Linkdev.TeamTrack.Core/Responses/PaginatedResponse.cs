namespace Linkdev.TeamTrack.Core.Responses
{
    public class PaginatedResponse<T>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public string Message { get; set; } = "Data retrieved succesfully";
        public IEnumerable<T>? Data { get; set; }
    }
}
