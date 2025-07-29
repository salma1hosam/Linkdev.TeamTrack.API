namespace Linkdev.TeamTrack.Core.Models
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedDate { get; set; }
    }
}
