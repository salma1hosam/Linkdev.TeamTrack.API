using System.ComponentModel.DataAnnotations;

namespace Linkdev.TeamTrack.Contract.DTOs
{
    public class ReturnedTaskUpdateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FinishDate { get; set; }
        public string ProjectName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdatedDate { get; set; }
    }
}
