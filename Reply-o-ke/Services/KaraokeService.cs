using Microsoft.EntityFrameworkCore;
using Reply_o_ke.Data;
using Reply_o_ke.Models;

namespace Reply_o_ke.Services
{
    public class KaraokeService
    {
        private readonly KaraokeDbContext _context;

        public KaraokeService(KaraokeDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateSessionAsync()
        {
            var session = new KaraokeSession();
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return session.Id;
        }

        public async Task<KaraokeSession?> GetSessionAsync(string sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Queue.Where(q => !q.IsPlayed))
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.IsActive);
        }

        public async Task<bool> AddToQueueAsync(string sessionId, string userName, string videoId, string videoTitle, string videoUrl)
        {
            var session = await GetSessionAsync(sessionId);
            if (session == null) return false;

            var queueItem = new QueueItem
            {
                SessionId = sessionId,
                UserName = userName,
                VideoId = videoId,
                VideoTitle = videoTitle,
                VideoUrl = videoUrl
            };

            _context.QueueItems.Add(queueItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<QueueItem>> GetQueueAsync(string sessionId)
        {
            return await _context.QueueItems
                .Where(q => q.SessionId == sessionId && !q.IsPlayed)
                .OrderBy(q => q.AddedAt)
                .ToListAsync();
        }

        public async Task<QueueItem?> GetNextInQueueAsync(string sessionId)
        {
            return await _context.QueueItems
                .Where(q => q.SessionId == sessionId && !q.IsPlayed)
                .OrderBy(q => q.AddedAt)
                .FirstOrDefaultAsync();
        }

        public async Task MarkAsPlayedAsync(int queueItemId)
        {
            var item = await _context.QueueItems.FindAsync(queueItemId);
            if (item != null)
            {
                item.IsPlayed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CloseSessionAsync(string sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RemoveQueueItemAsync(string sessionId, int itemId)
        {
            var item = await _context.QueueItems.FirstOrDefaultAsync(q => q.SessionId == sessionId && q.Id == itemId);
            if (item == null)
                return false;
            _context.QueueItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
