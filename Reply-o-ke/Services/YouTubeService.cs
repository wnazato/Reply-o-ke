using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Reply_o_ke.Models;

namespace Reply_o_ke.Services
{
    public class YouTubeService
    {
        private readonly Google.Apis.YouTube.v3.YouTubeService _youtubeService;
        private readonly IConfiguration _configuration;

        private static string[] GetApiKeysFromConfig(IConfiguration config)
        {
            // Espera uma string separada por vírgula em YouTube:ApiKeys
            var keys = config["YouTube:ApiKeys"];
            if (string.IsNullOrWhiteSpace(keys))
                return new string[0];
            return keys.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
        }

        public YouTubeService(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKeys = GetApiKeysFromConfig(configuration);
            if (apiKeys.Length == 0)
                throw new System.Exception("No YouTube API keys found in configuration (YouTube:ApiKeys)");
            var random = new System.Random();
            var apiKey = apiKeys[random.Next(apiKeys.Length)];
            _youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "Reply-o-ke"
            });
        }

        public async Task<List<YouTubeVideo>> SearchVideosAsync(string query)
        {
            try
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
            catch
            {
                // Fallback: Scrape YouTube public search page
                var fallbackVideos = new List<YouTubeVideo>();
                try
                {
                    using (var http = new System.Net.Http.HttpClient())
                    {
                        var searchUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query + " karaoke")}";
                        var html = await http.GetStringAsync(searchUrl);

                        // Regex para encontrar blocos de vídeo (simples, não cobre todos os casos)
                        var videoRegex = new System.Text.RegularExpressions.Regex(
    "\"videoId\":\"([^\"]{11})\".*?\"title\":\\{\"runs\":\\[\\{\"text\":\"([^\"]+)\"\\}\\].*?\"thumbnail\":\\{\"thumbnails\":\\[\\{\"url\":\"(https:[^\\\"]+)\"",
    System.Text.RegularExpressions.RegexOptions.Singleline);
                        System.Text.RegularExpressions.MatchCollection matches = videoRegex.Matches(html);
                        int count = 0;
                        foreach (System.Text.RegularExpressions.Match match in matches)
                        {
                            if (count >= 10) break;
                            string videoId = match.Groups[1].Value;
                            string title = System.Web.HttpUtility.HtmlDecode(match.Groups[2].Value);
                            string thumb = match.Groups[3].Value.Replace("\\u0026", "&");

                            // Filtro: só adiciona vídeos que podem ser embedados (não são shorts, não são playlists, etc)
                            // O critério mais seguro é: vídeoId tem 11 caracteres (vídeos normais do YouTube)
                            // E não pode ser do canal Recisio ou ter Recisio no título
                            if (videoId.Length == 11 &&
                                !title.Contains("recisio", System.StringComparison.OrdinalIgnoreCase))
                            {
                                // Validação extra: checa se é embeddable via noembed
                                var noembedUrl = $"https://noembed.com/embed?url=https://www.youtube.com/watch?v={videoId}";
                                try
                                {
                                    var noembedResp = await http.GetAsync(noembedUrl);
                                    if (!noembedResp.IsSuccessStatusCode)
                                        continue; // Não é embeddable
                                }
                                catch { continue; }

                                fallbackVideos.Add(new YouTubeVideo
                                {
                                    VideoId = videoId,
                                    Title = title,
                                    ThumbnailUrl = thumb,
                                    ChannelTitle = "YouTube"
                                });
                                count++;
                            }
                        }
                    }
                }
                catch { /* Se o scraping falhar, retorna lista vazia */ }
                return fallbackVideos;
            }
        }
    }
}
