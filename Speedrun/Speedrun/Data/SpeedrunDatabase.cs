using Microsoft.EntityFrameworkCore;
using Speedrun.Models;

namespace Speedrun.Data
{

    // Manages the connection to SQLite and defines tables
    public class SpeedrunDbContext : DbContext
    {
        public SpeedrunDbContext(DbContextOptions<SpeedrunDbContext> options)
            : base(options) 
        {
        }

        // Tables
        public DbSet<Game> Games { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<Comment> Comments { get; set; }




        // Configure relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Game entity
            modelBuilder.Entity<Game>()
                .HasKey(g => g.Id); 

            // Configure Run entity
            modelBuilder.Entity<Run>()
                .HasKey(r => r.Id);


            // Define relationship: One Game has many Runs
            modelBuilder.Entity<Run>()
                .HasOne(r => r.Game)            // each run has one game
                .WithMany(g => g.Runs)          // each game has many runs
                .HasForeignKey(r => r.GameId);  // gameId is the foreign key in the Runs table


            // Configure Comment entity
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);



            // Define relationship: One Run has many Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Run)             // Each Comment belongs to one Run
                .WithMany(r => r.Comments)      // Each Run has many Comments
                .HasForeignKey(c => c.RunId);   // RunId is the foreign key


            modelBuilder.Entity<Run>()
            .HasIndex(r => r.GameId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.RunId);

            // Seed initial data
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Name = "Super Mario Galaxy",
                    GameImageUrl = "https://upload.wikimedia.org/wikipedia/en/7/76/SuperMarioGalaxy.jpg"
                },
                new Game
                {
                    Id = 2,
                    Name = "NBA 2K26",
                    GameImageUrl = "https://image.api.playstation.com/vulcan/ap/rnd/202506/2509/ec1eec85d9130210701491db769cb9874cc09f6512ebca20.png"
                },
                new Game
                {
                    Id = 3,
                    Name = "Minecraft",
                    GameImageUrl = "https://preview.redd.it/which-one-is-the-superior-cover-art-v0-g4urr2d8zu3d1.png?width=1080&crop=smart&auto=webp&s=4ad01254b4f3bd2f8997aad4dd47a6ee40a6e426"
                }
            );
        }
    }
}