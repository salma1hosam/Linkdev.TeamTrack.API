using Linkdev.TeamTrack.Core.Enums;

namespace Linkdev.TeamTrack.Infrastructure.ElasticSearch.Indexes
{
    public class ProjectElasticDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProjectStatus { get; set; }
        public string ProjectManagerId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<ProjectTaskElasticNestedIndex> Tasks { get; set; } = [];
    }
}
