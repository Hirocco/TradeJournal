using Microsoft.EntityFrameworkCore;
using TradeJournal.Models;

namespace TradeJournal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TradingAccount> TradingAccounts { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Journal> Journals { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Trade>()
                .HasMany(e => e.Journals)
                .WithOne(e => e.Trade)
                .HasForeignKey(e => e.TradeId)
                .HasPrincipalKey(e => e.Id);

        }
    }
}
