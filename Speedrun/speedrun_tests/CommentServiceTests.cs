using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Speedrun.Data;
using Speedrun.Models;
using Speedrun.Services;

namespace speedrun_tests
{

    // COMMENT-FUNC-001
    public class CommentServiceTests
    {
        //creates a fresh in-memory database with a unique name each run
        private SpeedrunDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<SpeedrunDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SpeedrunDbContext(options);
        }



        // COMMENT-FUNC-001
        // Verify GetCommentsByRun() returns comments sorted newest first
        // Input: 3 comments with dates Feb 11, 13, 14
        // Expected order: Feb 14, Feb 13, Feb 11
        [Fact]
        public void GetCommentsByRun_ReturnsSortedByNewestFirst()
        {
            // Arrange
            using var context = CreateInMemoryDb("GetCommentsByRun_Sort_Test");

            // Need a game + run for FK constraints
            context.Games.Add(new Game { Id = 1, Name = "Test Game" });
            context.Runs.Add(new Run
            {
                Id = 1,
                GameId = 1,
                PlayerName = "Player1",
                Category = "Any%",
                Time = TimeSpan.FromMinutes(5)
            });

            context.Comments.AddRange(
                new Comment { Id = 1, RunId = 1, Username = "user1", Content = "First!", CreatedAt = new DateTime(2025, 2, 14) },
                new Comment { Id = 2, RunId = 1, Username = "user2", Content = "Nice run!", CreatedAt = new DateTime(2025, 2, 13) },
                new Comment { Id = 3, RunId = 1, Username = "user3", Content = "GG", CreatedAt = new DateTime(2025, 2, 11) }
            );
            context.SaveChanges();

            var service = new CommentService(context, NullLogger<CommentService>.Instance);

            // Act
            var result = service.GetCommentsByRun(1);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new DateTime(2025, 2, 14), result[0].CreatedAt);  // newest first
            Assert.Equal(new DateTime(2025, 2, 13), result[1].CreatedAt);
            Assert.Equal(new DateTime(2025, 2, 11), result[2].CreatedAt);  // oldest last
        }
    }
}