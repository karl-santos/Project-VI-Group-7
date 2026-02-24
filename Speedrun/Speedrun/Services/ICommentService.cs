using Speedrun.Models;

namespace Speedrun.Services
{

    /// <summary>
    /// Interface for comment management operations.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Gets all comments for a specific run sorted by newest first.
        /// </summary>
        /// <param name="runId">The ID of the run.</param>
        /// <returns>A list of comments sorted newest first.</returns>
        List<Comment> GetCommentsByRun(int runId);

        /// <summary>
        /// Creates a new comment on a run.
        /// </summary>
        /// <param name="runId">The ID of the run being commented on.</param>
        /// <param name="username">The username of the commenter.</param>
        /// <param name="content">The content of the comment.</param>
        /// <returns>The newly created comment.</returns>
        Comment CreateComment(int runId, string username, string content);
    }
}