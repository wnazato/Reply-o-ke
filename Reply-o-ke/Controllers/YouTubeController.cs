using Microsoft.AspNetCore.Mvc;
using Reply_o_ke.Services;

namespace Reply_o_ke.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YouTubeController : ControllerBase
    {
        private readonly YouTubeService _youtubeService;

        public YouTubeController(YouTubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required");
            }

            try
            {
                var videos = await _youtubeService.SearchVideosAsync(query);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching videos: {ex.Message}");
            }
        }
    }
}
