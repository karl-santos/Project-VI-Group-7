using Microsoft.Extensions.Logging;
using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{

    /// <summary>
    /// Service implementation for speedrun management.
    /// Handles all CRUD operations for run submissions.
    /// </summary>
    public class RunService : IRunService
    {

        private readonly SpeedrunDbContext _context;
        private readonly ILogger<RunService> _logger;
        public RunService(SpeedrunDbContext context, ILogger<RunService> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Gets all runs for a specific game sorted by time ascending.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of runs per page.</param>
        /// <returns>A sorted list of runs for the given game.</returns>
        public List<Run> GetRunsByGame(int gameId, int page = 1, int pageSize = 50)
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching runs for game ID: {gameId} (page {page})");

            var runs = _context.Runs
                .Where(r => r.GameId == gameId)
                .OrderBy(r => r.TimeMs)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation($"[{GetType().Name}] Retrieved {runs.Count} runs for game {gameId}");
            return runs;
        }



        /// <summary>
        /// Gets a single run by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the run.</param>
        /// <returns>The run if found, null otherwise.</returns>
        public Run? GetRunById(int id)
        {

            _logger.LogInformation($"[{GetType().Name}] Fetching run with ID: {id}");

            var run = _context.Runs.FirstOrDefault(r => r.Id == id);

            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Run with ID {id} not found");
            }
            else
            {
                _logger.LogInformation($"[{GetType().Name}] Found run by {run.PlayerName} with time {run.Time}");
            }

            return run;
 
        }



        /// <summary>
        /// Creates a new speedrun submission and saves it to the database.
        /// </summary>
        /// <param name="gameId">The ID of the game being run.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="category">The run category e.g. Any% or 100%.</param>
        /// <param name="time">The completion time of the run.</param>
        /// <param name="videoUrl">Optional URL to the run video.</param>
        /// <param name="notes">Optional notes about the run.</param>
        /// <returns>The newly created run.</returns>
        public Run CreateRun(int gameId, string playerName, string category, TimeSpan time, string? videoUrl, string? notes)
        {

            _logger.LogInformation($"[{GetType().Name}] Creating new run: Player={playerName}, Game={gameId}, Category={category}, Time={time}");

            // Create new Run object (in memory, not in database yet)
            var run = new Run
            {
                GameId = gameId,
                PlayerName = playerName,
                Category = category,
                Time = time,
                SubmittedAt = DateTime.UtcNow,
                VideoUrl = videoUrl,
                Notes = notes
                // Note: Id is not set because the database will auto-generate it
            };

            _context.Runs.Add(run);

            _context.SaveChanges(); // Save changes to database

            _logger.LogInformation($"[{GetType().Name}] Run created successfully with ID: {run.Id}");

            return run; 
        }




        /// <summary>
        /// Partially updates a run, only modifying fields that are provided.
        /// </summary>
        /// <param name="id">The ID of the run to update.</param>
        /// <param name="time">Optional new time value.</param>
        /// <param name="videoUrl">Optional new video URL.</param>
        /// <param name="notes">Optional new notes.</param>
        /// <returns>The updated run if found, null otherwise.</returns>
        public Run? UpdateRun(int id, TimeSpan? time, string? videoUrl, string? notes)
        {

            _logger.LogInformation($"[{GetType().Name}] Updating run ID: {id}");

            // find the run in the database
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return null;

            // Update only the fields that were provided (not null)
            if (time.HasValue) run.Time = time.Value;
            if (videoUrl != null) run.VideoUrl = videoUrl;
            if (notes != null) run.Notes = notes;

            _context.SaveChanges();

            _logger.LogInformation($"[{GetType().Name}] Run {id} updated successfully");

            return run;
        }




        /// <summary>
        /// Completely replaces a run with new data.
        /// </summary>
        /// <param name="id">The ID of the run to replace.</param>
        /// <param name="updatedRun">The new run data.</param>
        /// <returns>The replaced run if found, null otherwise.</returns>
        public Run? ReplaceRun(int id, Run updatedRun)
        {

            _logger.LogInformation($"[{GetType().Name}] Replacing run ID: {id}");

            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Cannot replace - run with ID {id} not found");
                return null;
            }

            // Updates all fields except ID
            run.PlayerName = updatedRun.PlayerName;
            run.Category = updatedRun.Category;
            run.Time = updatedRun.Time;
            run.VideoUrl = updatedRun.VideoUrl;
            run.Notes = updatedRun.Notes;

            _context.SaveChanges();
            _logger.LogInformation($"[{GetType().Name}] Run {id} replaced successfully");

            return run;
        }

        /// <summary>
        /// Deletes a run from the database.
        /// </summary>
        /// <param name="id">The ID of the run to delete.</param>
        /// <returns>True if deleted, false if not found.</returns>
        public bool DeleteRun(int id)
        {
            _logger.LogInformation($"[{GetType().Name}] Attempting to delete run ID: {id}");

            // find the run
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null)
            {
                _logger.LogWarning($"[{GetType().Name}] Cannot delete - run with ID {id} not found");
                return false;
            }

            _context.Runs.Remove(run);

            _context.SaveChanges();
            _logger.LogInformation($"[{GetType().Name}] Run {id} deleted successfully");

            return true;
        }
    }
}