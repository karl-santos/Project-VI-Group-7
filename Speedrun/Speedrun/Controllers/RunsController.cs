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
            var game = _gameService.GetGameById(gameId);
            if (game == null)
                return NotFound(new { message = "Game not found" });

            var runs = _runService.GetRunsByGame(gameId);
            return Ok(runs);
        }

        // POST: api/games/1/runs
        [HttpPost]
        public IActionResult CreateRun(int gameId, [FromBody] CreateRunRequest request)
        {
            var game = _gameService.GetGameById(gameId);
            if (game == null)
                return NotFound(new { message = "Game not found" });

            if (string.IsNullOrEmpty(request.PlayerName) || string.IsNullOrEmpty(request.Category))
                return BadRequest(new { message = "PlayerName and Category are required" });

            var run = _runService.CreateRun(
                gameId,
                request.PlayerName,
                request.Category,
                request.Time,
                request.VideoUrl,
                request.Notes
            );

            return CreatedAtAction(nameof(GetRunsByGame), new { gameId }, run);
        }

        // PATCH: api/games/1/runs/5
        [HttpPatch("{runId}")]
        public IActionResult UpdateRunPartial(int gameId, int runId, [FromBody] UpdateRunRequest request)
        {
            var run = _runService.UpdateRun(runId, request.Time, request.VideoUrl, request.Notes);
            if (run == null)
                return NotFound(new { message = "Run not found" });

            return Ok(run);
        }

        // PUT: api/games/1/runs/5
        [HttpPut("{runId}")]
        public IActionResult UpdateRunFull(int gameId, int runId, [FromBody] Run run)
        {
            var updated = _runService.ReplaceRun(runId, run);
            if (updated == null)
                return NotFound(new { message = "Run not found" });

            return Ok(updated);
        }

        // DELETE: api/games/1/runs/5
        [HttpDelete("{runId}")]
        public IActionResult DeleteRun(int gameId, int runId)
        {
            var deleted = _runService.DeleteRun(runId);
            if (!deleted)
                return NotFound(new { message = "Run not found" });

            return NoContent();
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