using Speedrun.Models;

namespace Speedrun.Services
{
    // Interface for speedrun management operations
    // Handles CRUD operations for run submissions
    public interface IRunService
    {
        // Gets all runs for a specific game, sorted by time (fastest first)
        List<Run> GetRunsByGame(int gameId, int page = 1, int pageSize = 50);

        // Retrieves a single run by its ID
        Run? GetRunById(int id);


        // Creates a new speedrun submission
        Run CreateRun(int gameId, string playerName, string category, TimeSpan time, string? videoUrl, string? notes);


        // Partially updates an existing run (PATCH operation)
        // Only updates fields that are provided
        Run? UpdateRun(int id, TimeSpan? time, string? videoUrl, string? notes);


        // Completely replaces a run with new data (PUT operation)
        Run? ReplaceRun(int id, Run run);

        // Deletes a run from the system
        bool DeleteRun(int id);
    }
}
