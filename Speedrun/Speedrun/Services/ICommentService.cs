using Speedrun.Models;

namespace Speedrun.Services
{

    // Interface for comment management operations
    public interface ICommentService
    {
        
        // Gets all comments for a specific run
        // Comments are sorted newest first
        List<Comment> GetCommentsByRun(int runId);
       
        // Creates a new comment on a rum
        Comment CreateComment(int runId, string username, string content);
    }
}