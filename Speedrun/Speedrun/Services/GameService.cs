using Speedrun.Models;
namespace Speedrun.Services
{
    // Service implementation for game management
    public class GameService : IGameService
    {
        // Static list (in-memory database simulation)
        private static List<Game> _games = new()
        {
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
        };

        // Returns all games available for speedrunning
        public List<Game> GetAllGames()
        {
            return _games;
        }

        // Finds a specific game by ID
        public Game? GetGameById(int id)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }

    }
}
