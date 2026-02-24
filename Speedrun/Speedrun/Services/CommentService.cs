using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{
    /// <summary>
    /// Service implementation for comment management.
    /// Handles retrieval and creation of comments on runs.
    /// </summary>
    public class CommentService : ICommentService
    {
        private readonly SpeedrunDbContext _context;
        private readonly ILogger<CommentService> _logger;

        public CommentService(SpeedrunDbContext context, ILogger<CommentService> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Retrieves all comments for a specific run sorted by newest first.
        /// </summary>
        /// <param name="runId">The ID of the run.</param>
        /// <returns>A list of comments sorted by newest first.</returns>
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

        /// <summary>
        /// Creates a new comment on a run and saves it to the database.
        /// </summary>
        /// <param name="runId">The ID of the run being commented on.</param>
        /// <param name="username">The username of the commenter.</param>
        /// <param name="content">The content of the comment.</param>
        /// <returns>The newly created comment.</returns>
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