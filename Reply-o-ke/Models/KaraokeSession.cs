using System.ComponentModel.DataAnnotations;

namespace Reply_o_ke.Models
{
    public class KaraokeSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public List<QueueItem> Queue { get; set; } = new List<QueueItem>();
    }
}
