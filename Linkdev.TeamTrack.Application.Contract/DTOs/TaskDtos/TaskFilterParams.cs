namespace Linkdev.TeamTrack.Contract.DTOs.TaskDtos
{
    public class TaskFilterParams : Paging
    {
        public string? Title { get; set; }
        public string? AssignedUserId { get; set; }
    }
}
