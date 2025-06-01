namespace Reply_o_ke.Models
{
    public class YouTubeVideo
    {
        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Url => $"https://www.youtube.com/watch?v={VideoId}";
        public string ChannelTitle { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }
}
