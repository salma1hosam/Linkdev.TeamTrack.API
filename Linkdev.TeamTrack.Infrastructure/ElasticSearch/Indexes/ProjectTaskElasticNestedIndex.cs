namespace Linkdev.TeamTrack.Infrastructure.ElasticSearch.Indexes
{
    public class ProjectTaskElasticNestedIndex
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AssignedUserId { get; set; } = string.Empty;
        public int ProjectId { get; set; }
    }
}
