using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Reply_o_ke.Models;

namespace Reply_o_ke.Services
{
    public class YouTubeService
    {
        private readonly Google.Apis.YouTube.v3.YouTubeService _youtubeService;
        private readonly IConfiguration _configuration;

        public YouTubeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _configuration["YouTube:ApiKey"],
                ApplicationName = "Reply-o-ke"
            });
        }

        public async Task<List<YouTubeVideo>> SearchVideosAsync(string query)
        {
            var searchRequest = _youtubeService.Search.List("snippet");
            searchRequest.Q = $"{query} karaoke";
            searchRequest.Type = "video";
            searchRequest.MaxResults = 10;
            searchRequest.VideoEmbeddable = SearchResource.ListRequest.VideoEmbeddableEnum.True__;
            searchRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;

            var searchResponse = await searchRequest.ExecuteAsync();

            var videos = new List<YouTubeVideo>();

            foreach (var searchResult in searchResponse.Items)
            {
                videos.Add(new YouTubeVideo
                {
                    VideoId = searchResult.Id.VideoId,
                    Title = searchResult.Snippet.Title,
                    ThumbnailUrl = searchResult.Snippet.Thumbnails.Medium?.Url ?? "",
                    ChannelTitle = searchResult.Snippet.ChannelTitle
                });
            }

            return videos;
        }
    }
}
