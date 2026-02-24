using Microsoft.AspNetCore.Mvc;
using Speedrun.Services;
using Speedrun.Models;

namespace Speedrun.Controllers
{
    /// <summary>
    /// API Controller for speedrun management operations.
    /// Handles CRUD operations for run submissions.
    /// </summary>
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

        /// <summary>
        /// Retrieves all runs for a specific game sorted by time ascending.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of runs per page.</param>
        /// <returns>A list of runs with a 200 OK response.</returns>
        [HttpGet]
        public IActionResult GetRunsByGame(int gameId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs - Request received (page {page})");

            var game = _gameService.GetGameById(gameId);
            if (game == null)
            {
                _logger.LogWarning($"[{GetType().Name}] GET /api/games/{gameId}/runs - Game not found");
                return NotFound(new { message = "Game not found" });
            }

            var runs = _runService.GetRunsByGame(gameId, page, pageSize);

            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs - Returning {runs.Count} runs");
            return Ok(runs);
        }


        /// <summary>
        /// Retrieves a specific run by its ID.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="runId">The ID of the run.</param>
        /// <returns>The run if found, 404 Not Found otherwise.</returns>
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



        /// <summary>
        /// Creates a new speedrun submission.
        /// </summary>
        /// <param name="gameId">The ID of the game being run.</param>
        /// <param name="request">The run data to create.</param>
        /// <returns>201 Created with the new run, 404 if game not found.</returns>
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
            return CreatedAtAction(nameof(GetRun), new { gameId, runId = run.Id }, run);
        }

        /// <summary>
        /// Partially updates an existing run.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="runId">The ID of the run to update.</param>
        /// <param name="request">The fields to update.</param>
        /// <returns>200 OK with updated run, 404 if not found.</returns>
        [HttpPatch("{runId}")]
        public IActionResult UpdateRunPartial(int gameId, int runId, [FromBody] UpdateRunRequest request)
        {
            _logger.LogInformation($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Request received");

            // Verify game exists
            if (_gameService.GetGameById(gameId) == null)
            {
                _logger.LogWarning($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Game not found");
                return NotFound(new { message = "Game not found" });
            }

            // Verify run exists and belongs to this game
            var existingRun = _runService.GetRunById(runId);
            if (existingRun == null || existingRun.GameId != gameId)
            {
                _logger.LogWarning($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            var run = _runService.UpdateRun(runId, request.Time, request.VideoUrl, request.Notes);

            _logger.LogInformation($"[{GetType().Name}] PATCH /api/games/{gameId}/runs/{runId} - Run updated successfully");
            return Ok(run);
        }





        /// <summary>
        /// Completely replaces an existing run with new data.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="runId">The ID of the run to replace.</param>
        /// <param name="run">The new run data.</param>
        /// <returns>200 OK with replaced run, 404 if not found.</returns>
        [HttpPut("{runId}")]
        public IActionResult UpdateRunFull(int gameId, int runId, [FromBody] Run run)
        {
            _logger.LogInformation($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Request received");


            // Verify game exists
            if (_gameService.GetGameById(gameId) == null)
            {
                _logger.LogWarning($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Game not found");
                return NotFound(new { message = "Game not found" });
            }

            // Verify run exists and belongs to this game
            var existingRun = _runService.GetRunById(runId);
            if (existingRun == null || existingRun.GameId != gameId)
            {
                _logger.LogWarning($"[{GetType().Name}] PUT /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

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




        /// <summary>
        /// Deletes a run from the system.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="runId">The ID of the run to delete.</param>
        /// <returns>204 No Content if deleted, 404 if not found.</returns>
        [HttpDelete("{runId}")]
        public IActionResult DeleteRun(int gameId, int runId)
        {
            _logger.LogInformation($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Request received");


            // Verify game exists
            if (_gameService.GetGameById(gameId) == null)
            {
                _logger.LogWarning($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Game not found");
                return NotFound(new { message = "Game not found" });
            }

            // Verify run exists and belongs to this game
            var existingRun = _runService.GetRunById(runId);
            if (existingRun == null || existingRun.GameId != gameId)
            {
                _logger.LogWarning($"[{GetType().Name}] DELETE /api/games/{gameId}/runs/{runId} - Run not found");
                return NotFound(new { message = "Run not found" });
            }

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

    /// <summary>
    /// Request model for creating a new speedrun submission.
    /// </summary>
    public class CreateRunRequest
    {
        /// <summary>The name of the player submitting the run.</summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>The category of the run e.g. Any% or 100%.</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>The completion time of the run.</summary>
        public TimeSpan Time { get; set; }

        /// <summary>Optional URL to the run video.</summary>
        public string? VideoUrl { get; set; }

        /// <summary>Optional notes about the run.</summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request model for partially updating an existing run.
    /// Only provided fields will be updated.
    /// </summary>
    public class UpdateRunRequest
    {
        /// <summary>Optional new completion time.</summary>
        public TimeSpan? Time { get; set; }

        /// <summary>Optional new video URL.</summary>
        public string? VideoUrl { get; set; }

        /// <summary>Optional new notes.</summary>
        public string? Notes { get; set; }
    }
}