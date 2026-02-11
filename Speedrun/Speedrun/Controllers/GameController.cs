using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speedrun.Models;
using Speedrun.Services;

namespace Speedrun.Controllers
{

    // API Controller for game-related operations
    // Handles requests for viewing available games
    // Base route: /api/games
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


        // GET: api/games
        [HttpGet]
        public IActionResult GetAllGames()
        {
            _logger.LogInformation($"[{GetType().Name}] GET /api/games - Request received");

            var games = _gameService.GetAllGames();

            _logger.LogInformation($"[{GetType().Name}] GET /api/games - Returning {games.Count} games");
            return Ok(games);
        }

        // GET: api/games/1
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

        // OPTIONS: api/games
        [HttpOptions]
        public IActionResult GetOptions()
        {
            _logger.LogInformation($"[{GetType().Name}] OPTIONS /api/games - Request received");
            Response.Headers.Add("Allow", "GET, OPTIONS");
            return Ok();
        }
    }
}
