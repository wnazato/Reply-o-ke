using Microsoft.AspNetCore.Mvc;
using Reply_o_ke.Services;

namespace Reply_o_ke.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly KaraokeService _karaokeService;
        private readonly QRCodeService _qrCodeService;

        public SessionController(KaraokeService karaokeService, QRCodeService qrCodeService)
        {
            _karaokeService = karaokeService;
            _qrCodeService = qrCodeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSession()
        {
            var sessionId = await _karaokeService.CreateSessionAsync();
            var joinUrl = $"{Request.Scheme}://{Request.Host}/join/{sessionId}";
            var qrCodeBase64 = _qrCodeService.GenerateQRCodeBase64(joinUrl);

            return Ok(new
            {
                SessionId = sessionId,
                JoinUrl = joinUrl,
                QRCode = qrCodeBase64
            });
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSession(string sessionId)
        {
            var session = await _karaokeService.GetSessionAsync(sessionId);
            if (session == null)
            {
                return NotFound();
            }
            var joinUrl = $"{Request.Scheme}://{Request.Host}/join/{sessionId}";
            var qrCodeBase64 = _qrCodeService.GenerateQRCodeBase64(joinUrl);
            return Ok(new
            {
                SessionId = session.Id,
                JoinUrl = joinUrl,
                QRCode = qrCodeBase64,
                CreatedAt = session.CreatedAt,
                IsActive = session.IsActive,
                Queue = session.Queue
            });
        }

        [HttpGet("{sessionId}/queue")]
        public async Task<IActionResult> GetQueue(string sessionId)
        {
            var queue = await _karaokeService.GetQueueAsync(sessionId);
            return Ok(queue);
        }

        [HttpPost("{sessionId}/close")]
        public async Task<IActionResult> CloseSession(string sessionId)
        {
            await _karaokeService.CloseSessionAsync(sessionId);
            return Ok();
        }

        [HttpDelete("{sessionId}/queue/{itemId}")]
        public async Task<IActionResult> RemoveQueueItem(string sessionId, int itemId)
        {
            var removed = await _karaokeService.RemoveQueueItemAsync(sessionId, itemId);
            if (!removed)
                return NotFound();
            return NoContent();
        }
    }
}
