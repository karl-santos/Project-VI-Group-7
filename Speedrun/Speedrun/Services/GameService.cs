using Speedrun.Data;
using Speedrun.Models;
namespace Speedrun.Services
{
    /// <summary>
    /// Service implementation for game management.
    /// Handles retrieval of game data from the database.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly SpeedrunDbContext _context;
        private readonly ILogger<GameService> _logger;
        public GameService(SpeedrunDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns all games available for speedrunning.
        /// </summary>
        /// <returns>A list of all games in the database.</returns>
        public List<Game> GetAllGames()
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching all games from database");

            var games = _context.Games.ToList();

            _logger.LogInformation($"[{GetType().Name}] Retrieved {games.Count} games");

            return games;
        }

        /// <summary>
        /// Finds a specific game by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the game.</param>
        /// <returns>The game if found, null otherwise.</returns>
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
