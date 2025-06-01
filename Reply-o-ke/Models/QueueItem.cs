using System.ComponentModel.DataAnnotations;

namespace Reply_o_ke.Models
{
    public class QueueItem
    {
        [Key]
        public int Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string VideoId { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public bool IsPlayed { get; set; } = false;
        
        public KaraokeSession? Session { get; set; }
    }
}
