using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{
    // Service implementation for comment management
    public class CommentService : ICommentService
    {
        private readonly SpeedrunDbContext _context;

        public CommentService(SpeedrunDbContext context)
        {
            _context = context;
        }


        // Retrieves all comments for a specific run sorted by newest first
        public List<Comment> GetCommentsByRun(int runId)
        {
            return _context.Comments
                .Where(c => c.RunId == runId)           // Filter by run
                .OrderByDescending(c => c.CreatedAt)    // Sort newest first
                .ToList();
        }

        // Creates a new comment on a run
        public Comment CreateComment(int runId, string username, string content)
        {
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

            return comment;
        }
    }
}