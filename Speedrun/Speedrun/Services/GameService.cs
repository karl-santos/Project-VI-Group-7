using Speedrun.Data;
using Speedrun.Models;
namespace Speedrun.Services
{
    // Service implementation for game management
    public class GameService : IGameService
    {
        private readonly SpeedrunDbContext _context;
        private readonly ILogger<GameService> _logger;
        public GameService(SpeedrunDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Returns all games available for speedrunning
        public List<Game> GetAllGames()
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching all games from database");

            var games = _context.Games.ToList();

            _logger.LogInformation($"[{GetType().Name}] Retrieved {games.Count} games");

            return games;
        }

        // Finds a specific game by Id. Returns null if not found
        public Game? GetGameById(int id)
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching game with ID: {id}");

            var game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Game with ID {id} not found");
            }
            else
            {
                _logger.LogInformation($"[{GetType().Name}] Found game: {game.Name}");
            }

            return game;

        }

    }
}
