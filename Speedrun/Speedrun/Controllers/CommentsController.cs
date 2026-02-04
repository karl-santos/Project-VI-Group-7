using Microsoft.AspNetCore.Mvc;
using Speedrun.Services;

namespace Speedrun.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/runs/{runId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IRunService _runService;
        private readonly ILogger<GameController> _logger;

        public CommentsController(ICommentService commentService, IRunService runService, ILogger<GameController> logger)
        {
            _commentService = commentService;
            _runService = runService;
            _logger = logger;
        }

        // GET: api/games/1/runs/5/comments
        [HttpGet]
        public IActionResult GetComments(int gameId, int runId)
        {
            var run = _runService.GetRunById(runId);
            if (run == null)
                return NotFound(new { message = "Run not found" });

            var comments = _commentService.GetCommentsByRun(runId);
            return Ok(comments);
        }

        // POST: api/games/1/runs/5/comments
        [HttpPost]
        public IActionResult CreateComment(int gameId, int runId, [FromBody] CreateCommentRequest request)
        {
            var run = _runService.GetRunById(runId);
            if (run == null)
                return NotFound(new { message = "Run not found" });

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Content))
                return BadRequest(new { message = "Username and Content are required" });

            var comment = _commentService.CreateComment(runId, request.Username, request.Content);
            return CreatedAtAction(nameof(GetComments), new { gameId, runId }, comment);
        }
    }

    public class CreateCommentRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}