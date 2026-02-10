using Microsoft.EntityFrameworkCore;
using Speedrun.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Speedrun.Data
{
    /// <summary>
    /// Database context for the Speedrun application
    /// Manages the connection to SQLite and defines tables
    /// </summary>
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

        /// <summary>
        /// Configure relationships and constraints
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Game entity
            modelBuilder.Entity<Game>()
                .HasKey(g => g.Id);

            // Configure Run entity
            modelBuilder.Entity<Run>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Run>()
                .HasOne(r => r.Game)
                .WithMany(g => g.Runs)
                .HasForeignKey(r => r.GameId);

            // Configure Comment entity
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Run)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RunId);

            // Seed initial data (optional)
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Name = "Super Mario 64",
                    GameImageUrl = "https://upload.wikimedia.org/wikipedia/en/6/6a/Super_Mario_64_box_cover.jpg"
                },
                new Game
                {
                    Id = 2,
                    Name = "Celeste",
                    GameImageUrl = "https://upload.wikimedia.org/wikipedia/commons/0/0f/Celeste_box_art_final.png"
                },
                new Game
                {
                    Id = 3,
                    Name = "Portal",
                    GameImageUrl = "https://upload.wikimedia.org/wikipedia/en/6/63/Portal_PC_boxart.jpg"
                }
            );
        }
    }
}