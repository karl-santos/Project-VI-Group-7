using Microsoft.AspNetCore.Mvc;
using Speedrun.Services;
using Speedrun.Models;

namespace Speedrun.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/runs")]
    public class RunsController : ControllerBase
    {
        private readonly IRunService _runService;
        private readonly IGameService _gameService;
        private readonly ILogger<RunsController> _logger;

        public RunsController(IRunService runService, IGameService gameService, ILogger<RunsController> logger)
        {
            _runService = runService;
            _gameService = gameService;
            _logger = logger;
        }

        // GET: api/games/1/runs
        [HttpGet]
        public IActionResult GetRunsByGame(int gameId)
        {

            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs - Request received");


            var game = _gameService.GetGameById(gameId);
            if (game == null)
            {
                _logger.LogWarning($"[{GetType().Name}] GET /api/games/{gameId}/runs - Game not found");
                return NotFound(new { message = "Game not found" });
            }

            var runs = _runService.GetRunsByGame(gameId);


            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs - Returning {runs.Count} runs");
            return Ok(runs);  


        }


        // GET: api/games/1/runs/1
        [HttpGet("{runId}")]
        public IActionResult GetRun(int gameId, int runId)
        {
            var game = _gameService.GetGameById(gameId);
            if (game == null)
                return NotFound(new { message = "Game not found" });

            var run = _runService.GetRunById(runId);
            if (run == null)
                return NotFound(new { message = "Run not found" });
            
            // verify the run actually belongs to this game
            if (run.GameId != gameId)
                return NotFound(new { message = "Run not found for this game" });

            return Ok(run);
        }



        // POST: api/games/1/runs
        [HttpPost]
        public IActionResult CreateRun(int gameId, [FromBody] CreateRunRequest request)
        {

            _logger.LogInformation($"[{GetType().Name}] POST /api/games/{gameId}/runs - Request received for player: {request.PlayerName}");

            // Validate that game exists
            var game = _gameService.GetGameById(gameId);
            if (game == null)
            {
                _logger.LogWarning($"[{GetType().Name}] POST /api/games/{gameId}/runs - Game not found");
                return NotFound(new { message = "Game not found" });
            }



            // Validate required fields are present
            if (string.IsNullOrEmpty(request.PlayerName) || string.IsNullOrEmpty(request.Category))
            {
                _logger.LogWarning($"[{GetType().Name}] POST /api/games/{gameId}/runs - Missing required fields");
                return BadRequest(new { message = "PlayerName and Category are required" });
            }


            var run = _runService.CreateRun(
                gameId,
                request.PlayerName,
                request.Category,
                request.Time,
                request.VideoUrl,
                request.Notes
            );

            _logger.LogInformation($"[{GetType().Name}] POST /api/games/{gameId}/runs - Run created with ID: {run.Id}");
            return CreatedAtAction(nameof(GetRunsByGame), new { gameId }, run);
        }

        // PATCH: api/games/1/runs/5
        [HttpPatch("{runId}")]
        public IActionResult UpdateRunPartial(int gameId, int runId, [FromBody] UpdateRunRequest request)
        {
            _logger.LogInformation($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Request received");

            // Log incoming update request
            _logger.LogInformation($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Request received");

            // Update the run (only provided fields)
            var run = _runService.UpdateRun(runId, request.Time, request.VideoUrl, request.Notes);

            // If run not found, return 404
            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            // Log successful update
            _logger.LogInformation($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Run updated successfully");
            return Ok(run);  // 200 OK with updated run
        }





        // PUT: api/games/1/runs/5
        [HttpPut("{runId}")]
        public IActionResult UpdateRunFull(int gameId, int runId, [FromBody] Run run)
        {
            _logger.LogInformation($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Request received");

            // Replace the entire run
            var updated = _runService.ReplaceRun(runId, run);

            // If run not found, return 404
            if (updated == null)
            {
                _logger.LogWarning($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            _logger.LogInformation($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Run replaced successfully");
            return Ok(updated);  // 200 OK with updated run
        }




        // DELETE: api/games/1/runs/5
        [HttpDelete("{runId}")]
        public IActionResult DeleteRun(int gameId, int runId)
        {
            _logger.LogInformation($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Request received");

            // Delete the run
            var deleted = _runService.DeleteRun(runId);

            // If run not found, return 404
            if (!deleted)
            {
                _logger.LogWarning($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            _logger.LogInformation($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Run deleted successfully");
            return NoContent();  // 204 No Content

        }
    }

    // Request models
    public class CreateRunRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public TimeSpan Time { get; set; }
        public string? VideoUrl { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateRunRequest
    {
        public TimeSpan? Time { get; set; }
        public string? VideoUrl { get; set; }
        public string? Notes { get; set; }
    }
}