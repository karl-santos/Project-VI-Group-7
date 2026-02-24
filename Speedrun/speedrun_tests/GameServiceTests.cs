using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Speedrun.Data;
using Speedrun.Models;
using Speedrun.Services;

namespace speedrun_tests
{

    // GAME-FUNC-001, GAME-FUNC-002, GAME-FUNC-003
    public class GameServiceTests
    {
        //creates a fresh in-memory database with a unique name each run
        private SpeedrunDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<SpeedrunDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new SpeedrunDbContext(options);
        }

        // GAME-FUNC-001
        // Verify GetAllGames() returns all 3 games from the database
        [Fact]
        public void GetAllGames_ReturnsAllThreeGames()
        {
            // Arrange - seed in-memory DB with 3 games
            using var context = CreateInMemoryDb("GetAllGames_Test");
            context.Games.AddRange(
                new Game { Id = 1, Name = "Super Mario Galaxy" },
                new Game { Id = 2, Name = "NBA 2K26" },
                new Game { Id = 3, Name = "Minecraft" }
            );
            context.SaveChanges();

            var service = new GameService(context, NullLogger<GameService>.Instance);

            // Act
            var result = service.GetAllGames();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, g => g.Name == "Super Mario Galaxy");
            Assert.Contains(result, g => g.Name == "NBA 2K26");
            Assert.Contains(result, g => g.Name == "Minecraft");
        }


        // GAME-FUNC-002
        // Verify GetGameById() returns correct game when valid ID given
        [Fact]
        public void GetGameById_ValidId_ReturnsCorrectGame()
        {
            // Arrange - seed DB with a game at ID 2
            using var context = CreateInMemoryDb("GetGameById_Valid_Test");
            context.Games.Add(new Game { Id = 2, Name = "NBA 2K26" });
            context.SaveChanges();

            var service = new GameService(context, NullLogger<GameService>.Instance);

            // Act
            var result = service.GetGameById(2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("NBA 2K26", result.Name);
        }


        // GAME-FUNC-003
        // Verify GetGameById() returns null when invalid ID given
        [Fact]
        public void GetGameById_InvalidId_ReturnsNull()
        {
            // Arrange - DB has no game with ID 999
            using var context = CreateInMemoryDb("GetGameById_Invalid_Test");
            context.Games.Add(new Game { Id = 1, Name = "Super Mario Galaxy" });
            context.SaveChanges();

            var service = new GameService(context, NullLogger<GameService>.Instance);

            // Act
            var result = service.GetGameById(999);

            // Assert
            Assert.Null(result);
        }
    }
}