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
            var games = _gameService.GetAllGames();
            return Ok(games);
        }

        // GET: api/games/1
        [HttpGet("{gameId}")]
        public IActionResult GetGameById(int gameId)
        {
            var game = _gameService.GetGameById(gameId);
            if (game == null)
                return NotFound(new { message = "Game not found" });

            return Ok(game);
        }

        // OPTIONS: api/games
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS");
            return Ok();
        }
    }
}
