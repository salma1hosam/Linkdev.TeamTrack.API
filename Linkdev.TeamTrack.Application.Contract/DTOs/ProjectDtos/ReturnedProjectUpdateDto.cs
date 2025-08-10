namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class ReturnedProjectUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProjectManagerName { get; set; }        
        public DateTime LastUpdatedDate { get; set; }
    }
}
