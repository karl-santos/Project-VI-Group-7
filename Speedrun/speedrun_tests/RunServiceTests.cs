using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Speedrun.Data;
using Speedrun.Models;
using Speedrun.Services;

namespace speedrun_tests
{
    // RUN-FUNC-001, RUN-FUNC-002, RUN-FUNC-003
    public class RunServiceTests
    {
        // creates a fresh in-memory database with a unique name each run
        private SpeedrunDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<SpeedrunDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SpeedrunDbContext(options);
        }


        // RUN-FUNC-001
        // Verify GetRunsByGame() returns runs sorted by time ascending (fastest first)
        [Fact]
        public void GetRunsByGame_ReturnsSortedByTimeAscending()
        {
            // Arrange
            using var context = CreateInMemoryDb("GetRunsByGame_Sort_Test");

            // Need a game for the FK constraint
            context.Games.Add(new Game { Id = 1, Name = "Test Game" });

            context.Runs.AddRange(
                new Run { Id = 1, GameId = 1, PlayerName = "Player1", Category = "Any%", Time = TimeSpan.FromMinutes(10) },
                new Run { Id = 2, GameId = 1, PlayerName = "Player2", Category = "Any%", Time = TimeSpan.FromMinutes(5) },
                new Run { Id = 3, GameId = 1, PlayerName = "Player3", Category = "Any%", Time = TimeSpan.FromMinutes(8) }
            );
            context.SaveChanges();

            var service = new RunService(context, NullLogger<RunService>.Instance);

            // Act
            var result = service.GetRunsByGame(1);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(TimeSpan.FromMinutes(5), result[0].Time);   // fastest first
            Assert.Equal(TimeSpan.FromMinutes(8), result[1].Time);
            Assert.Equal(TimeSpan.FromMinutes(10), result[2].Time);  // slowest last
        }

        // RUN-FUNC-002
        // Verify UpdateRun() only modifies the provided fields,
        // leaving PlayerName, VideoUrl, and Notes unchanged when null is passed
        [Fact]
        public void UpdateRun_PartialUpdate_OnlyChangesProvidedFields()
        {
            // Arrange - seed DB with one run
            using var context = CreateInMemoryDb("UpdateRun_Partial_Test");

            context.Games.Add(new Game { Id = 1, Name = "Test Game" });
            context.Runs.Add(new Run
            {
                Id = 1,
                GameId = 1,
                PlayerName = "og Player",
                Category = "Any%",
                Time = TimeSpan.FromMinutes(10),
                VideoUrl = "ogURL.com",
                Notes = "og notes"
            });
            context.SaveChanges();

            var service = new RunService(context, NullLogger<RunService>.Instance);

            // Act - only update time, leave VideoUrl and Notes as null (don't update)
            var result = service.UpdateRun(1, TimeSpan.FromMinutes(5), null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TimeSpan.FromMinutes(5), result.Time);   // time updated
            Assert.Equal("ogURL.com", result.VideoUrl);           // unchanged
            Assert.Equal("og notes", result.Notes);               // unchanged
            Assert.Equal("og Player", result.PlayerName);         // unchanged
        }




        // RUN-FUNC-003
        // Verify DeleteRun() removes the run from the database and returns true
        [Fact]
        public void DeleteRun_ExistingRun_DeletesAndReturnsTrue()
        {
            // Arrange - seed DB with a run at ID 5
            using var context = CreateInMemoryDb("DeleteRun_Test");

            context.Games.Add(new Game { Id = 1, Name = "Test Game" });
            context.Runs.Add(new Run
            {
                Id = 5,
                GameId = 1,
                PlayerName = "Player1",
                Category = "Any%",
                Time = TimeSpan.FromMinutes(10)
            });
            context.SaveChanges();

            var service = new RunService(context, NullLogger<RunService>.Instance);

            // Act
            var deleted = service.DeleteRun(5);

            // Assert
            Assert.True(deleted);                               // method returned true
            Assert.Null(service.GetRunById(5));                 // run is gone from DB
        }
    }
}