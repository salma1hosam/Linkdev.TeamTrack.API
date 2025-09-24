namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class GetAllProjectsDto
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProjectManagerId { get; set; }
        public string ProjectStatus { get; set; }
    }
}
