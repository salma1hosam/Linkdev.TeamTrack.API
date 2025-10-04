namespace Linkdev.TeamTrack.Contract.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }

        public BadRequestException(List<string> errors) : base("Validation Failed")
        {
            Errors = errors;
        }

        public List<string> Errors { get; }
    }
}
