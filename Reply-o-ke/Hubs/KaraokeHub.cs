using Microsoft.AspNetCore.SignalR;
using Reply_o_ke.Services;

namespace Reply_o_ke.Hubs
{
    public class KaraokeHub : Hub
    {
        private readonly KaraokeService _karaokeService;

        public KaraokeHub(KaraokeService karaokeService)
        {
            _karaokeService = karaokeService;
        }

        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task AddToQueue(string sessionId, string userName, string videoId, string videoTitle, string videoUrl)
        {
            var success = await _karaokeService.AddToQueueAsync(sessionId, userName, videoId, videoTitle, videoUrl);
            
            if (success)
            {
                // Notifica todos os clients da sessão sobre o novo item na fila
                await Clients.Group(sessionId).SendAsync("QueueUpdated", sessionId);
                
                // Notifica especificamente o host sobre o novo item
                await Clients.Group($"host_{sessionId}").SendAsync("NewQueueItem", new
                {
                    UserName = userName,
                    VideoTitle = videoTitle,
                    VideoUrl = videoUrl,
                    VideoId = videoId
                });
            }
        }

        public async Task JoinAsHost(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"host_{sessionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task VideoEnded(string sessionId, int queueItemId)
        {
            await _karaokeService.MarkAsPlayedAsync(queueItemId);
            
            // Notifica sobre a atualização da fila
            await Clients.Group(sessionId).SendAsync("QueueUpdated", sessionId);
            
            // Busca o próximo item da fila
            var nextItem = await _karaokeService.GetNextInQueueAsync(sessionId);
            if (nextItem != null)
            {
                await Clients.Group($"host_{sessionId}").SendAsync("PlayNext", nextItem);
            }
            else
            {
                await Clients.Group($"host_{sessionId}").SendAsync("QueueEmpty");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
