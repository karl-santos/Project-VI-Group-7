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
            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs/{runId}/comments - Request received");

            // Validate that run exists before getting comments
            var run = _runService.GetRunById(runId);
            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] GET /api/games/{gameId}/runs/{runId}/comments - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            // Get comments for this run from database
            var comments = _commentService.GetCommentsByRun(runId);

            _logger.LogInformation($"[{GetType().Name}] GET /api/games/{gameId}/runs/{runId}/comments - Returning {comments.Count} comments");
            return Ok(comments);  // 200 OK with comments array
        }






        // POST: api/games/1/runs/5/comments
        [HttpPost]
        public IActionResult CreateComment(int gameId, int runId, [FromBody] CreateCommentRequest request)
        {
            _logger.LogInformation($"[{GetType().Name}] POST /api/games/{gameId}/runs/{runId}/comments - Request received from user: {request.Username}");

            // Validate that run exists
            var run = _runService.GetRunById(runId);
            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] POST /api/games/{gameId}/runs/{runId}/comments - Run not found");
                return NotFound(new { message = "Run not found" });
            }

            // Validate required fields are present
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Content))
            {
                _logger.LogWarning($"[{GetType().Name}] POST /api/games/{gameId}/runs/{runId}/comments - Missing required fields");
                return BadRequest(new { message = "Username and Content are required" });
            }

            // Create the comment in the database
            var comment = _commentService.CreateComment(runId, request.Username, request.Content);

            _logger.LogInformation($"[{GetType().Name}] POST /api/games/{gameId}/runs/{runId}/comments - Comment created with ID: {comment.Id}");
            return CreatedAtAction(nameof(GetComments), new { gameId, runId }, comment);  // 201 Created
        }
    }






    public class CreateCommentRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}