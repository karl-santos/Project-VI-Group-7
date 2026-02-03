using Speedrun.Models;
namespace Speedrun.Services
{
    public class CommentsService
    {

        public CommentsService() { }
        
        public Comment AddComment(int runId, string userId, string content)
        {
            var comment = new Comment
            {
                RunId = runId,
                Username = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };


            // save to database (not implemented)

            return comment;

        }
    }
}
