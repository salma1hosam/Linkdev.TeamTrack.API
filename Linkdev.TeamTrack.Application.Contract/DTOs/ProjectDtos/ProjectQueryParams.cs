namespace Linkdev.TeamTrack.Contract.DTOs.ProjectDtos
{
    public class ProjectQueryParams : Paging
    {
        public string? Name { get; set; }
        public int? ProjectStatus { get; set; }

    }
}
