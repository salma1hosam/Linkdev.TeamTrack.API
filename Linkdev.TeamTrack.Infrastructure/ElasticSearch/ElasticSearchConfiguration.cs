namespace Linkdev.TeamTrack.Infrastructure.ElasticSearch
{
    public sealed class ElasticSearchConfiguration
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public string Url { get; init; }
        public string DefaultIndex { get; init; } = "projects";
    }
}
