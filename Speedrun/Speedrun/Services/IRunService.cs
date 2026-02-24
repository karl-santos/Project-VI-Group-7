using Speedrun.Models;

namespace Speedrun.Services
{
    /// <summary>
    /// Interface for speedrun management operations.
    /// </summary>
    public interface IRunService
    {
        /// <summary>
        /// Gets all runs for a specific game sorted by time ascending.
        /// </summary>
        List<Run> GetRunsByGame(int gameId, int page = 1, int pageSize = 50);

        /// <summary>
        /// Gets a single run by its ID.
        /// </summary>
        Run? GetRunById(int id);

        /// <summary>
        /// Creates a new speedrun submission.
        /// </summary>
        Run CreateRun(int gameId, string playerName, string category, TimeSpan time, string? videoUrl, string? notes);

        /// <summary>
        /// Partially updates an existing run.
        /// </summary>
        Run? UpdateRun(int id, TimeSpan? time, string? videoUrl, string? notes);

        /// <summary>
        /// Completely replaces a run with new data.
        /// </summary>
        Run? ReplaceRun(int id, Run run);

        /// <summary>
        /// Deletes a run from the system.
        /// </summary>
        bool DeleteRun(int id);
    }
}
