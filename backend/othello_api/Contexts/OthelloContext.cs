using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using othello_api.Models;

namespace othello_api.Contexts;


public class OthelloContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GameMove> GameMoves { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    public OthelloContext(DbContextOptions<OthelloContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(builder =>
        {
            builder.OwnsMany(g => g.Moves);
        });
    }

    public async Task ApplyMigrationsAsync()
    {
        await Database.MigrateAsync();
    }
}

