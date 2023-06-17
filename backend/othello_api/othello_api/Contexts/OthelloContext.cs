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

    /* private readonly IConfiguration Configuration; */

    public OthelloContext(DbContextOptions<OthelloContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(builder =>
        {
            builder.OwnsMany(g => g.Moves);
            //builder.OwnsOne(g => g.State);
        });
    }

    public async Task ApplyMigrationsAsync()
    {
        await Database.MigrateAsync();
    }
}





















/* protected override void OnModelCreating(ModelBuilder modelBuilder) */
/* { */
/*     // do not use plural form for table names */
/*     modelBuilder.Model.GetEntityTypes() */
/*         .Where(x => !x.ClrType.IsSubclassOf(typeof(ValueObject))) */
/*         .Configure(e => e.SetTableName(e.DisplayName())); */
/**/
/*     base.OnModelCreating(modelBuilder); */
/**/
/*     modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); */
/* } */

//private string DBConnectionString { get; }
//
/* public OthelloContext(IConfiguration configuration){ */
/*   Configuration = configuration; */
/* } */
/* { */
/*     /* var folder = Environment.SpecialFolder.LocalApplicationData; */
/*     /* var path = Environment.GetFolderPath(folder); */
/**/
/*     //DbPath = System.IO.Path.Join(path, "othello.db"); */
/* } */

// The following configures EF to create a Sqlite database file in the
// special "local" folder for your platform.
/* protected override void OnConfiguring(DbContextOptionsBuilder options) */
/*     => options.UseSqlServer(Configuration["ConnectionStrings:sql_database"]); */

// Server=tcp:othello.database.windows.net,1433;Initial Catalog=othello;Persist Security Info=False;User ID=othelloserver;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;
// Connection Timeout = 30;
