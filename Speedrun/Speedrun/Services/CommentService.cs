using Speedrun.Models;

namespace Speedrun.Services
{
    // Service implementation for comment management
    public class CommentService : ICommentService
    {
        private static List<Comment> _comments = new(); // Static list maintains comment data across requests
        private static int _nextId = 1; // Auto-incrementing ID generator


        // Retrieves all comments for a specific run sorted by newest first
        public List<Comment> GetCommentsByRun(int runId)
        {
            return _comments
                .Where(c => c.RunId == runId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        // Creates a new comment on a run
        public Comment CreateComment(int runId, string username, string content)
        {
            var comment = new Comment
            {
                Id = _nextId++,
                RunId = runId,
                Username = username,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _comments.Add(comment);
            return comment;
        }
    }
}