using Microsoft.EntityFrameworkCore;
using Reply_o_ke.Models;

namespace Reply_o_ke.Data
{
    public class KaraokeDbContext : DbContext
    {
        public KaraokeDbContext(DbContextOptions<KaraokeDbContext> options) : base(options)
        {
        }

        public DbSet<KaraokeSession> Sessions { get; set; }
        public DbSet<QueueItem> QueueItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QueueItem>()
                .HasOne(q => q.Session)
                .WithMany(s => s.Queue)
                .HasForeignKey(q => q.SessionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
