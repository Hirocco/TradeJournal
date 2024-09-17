using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using TradeJournal.Models;

namespace TradeJournal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auth> Auths { get; set; }
        public DbSet<RefreshToken> Tokens { get; set; }
        public DbSet<Playbook> Playbooks { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Auth)
                .WithOne(a => a.User)
                .HasForeignKey<Auth>(a => a.UserId);

            modelBuilder.Entity<Auth>()
                .HasMany(a => a.RefreshToken)
                .WithOne(t => t.Auth)
                .HasForeignKey(t => t.AuthId);

            modelBuilder.Entity<Playbook>()
               .HasMany(p => p.Conditions)
               .WithOne(c => c.Playbook)
               .HasForeignKey(c => c.PlaybookId);
        }
    }
}
