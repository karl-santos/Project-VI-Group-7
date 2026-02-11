using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{
    // Service implementation for comment management
    public class CommentService : ICommentService
    {
        private readonly SpeedrunDbContext _context;
        private readonly ILogger<CommentService> _logger;

        public CommentService(SpeedrunDbContext context, ILogger<CommentService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Retrieves all comments for a specific run sorted by newest first
        public List<Comment> GetCommentsByRun(int runId)
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching comments for run ID: {runId}");

            var comments = _context.Comments
                .Where(c => c.RunId == runId)           // Filter by run
                .OrderByDescending(c => c.CreatedAt)    // Sort newest first
                .ToList();

            _logger.LogInformation($"[{GetType().Name}] Retrieved {comments.Count} comments for run {runId}");
            return comments;
        }

        // Creates a new comment on a run
        public Comment CreateComment(int runId, string username, string content)
        {
            _logger.LogInformation($"[{GetType().Name}] Creating comment on run {runId} by user: {username}");

            // Create comment object in memory
            var comment = new Comment
            {
                RunId = runId,
                Username = username,
                Content = content, 
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            _context.SaveChanges();
            _logger.LogInformation($"[{GetType().Name}] Comment created successfully with ID: {comment.Id}");

            return comment;
        }
    }
}