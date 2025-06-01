using Microsoft.AspNetCore.SignalR.Client;

namespace Reply_o_ke.Client.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly string _hubUrl;

        public SignalRService(string baseUrl)
        {
            _hubUrl = $"{baseUrl.TrimEnd('/')}/karaokehub";
        }

        public async Task StartAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            await _hubConnection.StartAsync();
        }

        public async Task JoinSessionAsync(string sessionId)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("JoinSession", sessionId);
            }
        }

        public async Task JoinAsHostAsync(string sessionId)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("JoinAsHost", sessionId);
            }
        }

        public async Task AddToQueueAsync(string sessionId, string userName, string videoId, string videoTitle, string videoUrl)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("AddToQueue", sessionId, userName, videoId, videoTitle, videoUrl);
            }
        }

        public async Task VideoEndedAsync(string sessionId, int queueItemId)
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("VideoEnded", sessionId, queueItemId);
            }
        }

        public void OnQueueUpdated(Action<string> handler)
        {
            _hubConnection?.On("QueueUpdated", handler);
        }

        public void OnNewQueueItem(Action<object> handler)
        {
            _hubConnection?.On("NewQueueItem", handler);
        }

        public void OnPlayNext(Action<object> handler)
        {
            _hubConnection?.On("PlayNext", handler);
        }

        public void OnQueueEmpty(Action handler)
        {
            _hubConnection?.On("QueueEmpty", handler);
        }

        public bool IsConnected =>
            _hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
