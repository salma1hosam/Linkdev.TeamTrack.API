namespace Linkdev.TeamTrack.Core.Responses
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = default!;
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }
    }
}
