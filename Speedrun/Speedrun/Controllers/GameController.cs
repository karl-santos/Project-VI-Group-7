using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speedrun.Models;
using Speedrun.Services;

namespace Speedrun.Controllers
{

    /// <summary>
    /// API Controller for game related operations.
    /// Handles requests for viewing available games.
    /// </summary>
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;


        public GameController(ILogger<GameController> logger, IGameService gamesService)
        {
            _logger = logger;
            _gameService = gamesService;
        }


        /// <summary>
        /// Retrieves all available games.
        /// </summary>
        /// <returns>A list of all games with a 200 OK response.</returns>
        [HttpGet]
        [ResponseCache(Duration = 60)]
        public IActionResult GetAllGames()
        {
            _logger.LogInformation($"[{GetType().Name}] GET /api/games - Request received");

            var games = _gameService.GetAllGames();

            _logger.LogInformation($"[{GetType().Name}] GET /api/games - Returning {games.Count} games");
            return Ok(games);
        }

        /// <summary>
        /// Retrieves a specific game by its ID.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>The game if found, 404 Not Found otherwise.</returns>
        [HttpGet("{gameId}")]
        public IActionResult GetGameById(int gameId)
        {
            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId} - Request received");

            var game = _gameService.GetGameById(gameId);
            if (game == null)
            {
                _logger.LogWarning($"[{GetType().Name}] GET /api/games/{gameId} - Game not found");
                return NotFound(new { message = "Game not found" });
            }
            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId} - Returning game: {game.Name}");
            return Ok(game);
        }

        /// <summary>
        /// Returns the allowed HTTP methods for the games endpoint.
        /// </summary>
        /// <returns>200 OK with Allow header set to GET and OPTIONS.</returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            _logger.LogInformation($"[{GetType().Name}] OPTIONS /api/games - Request received");
            Response.Headers.Add("Allow", "GET, OPTIONS");
            return Ok();
        }
    }
}
