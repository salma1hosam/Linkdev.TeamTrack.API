namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ProjectManagerName { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
