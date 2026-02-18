using Microsoft.Extensions.Logging;
using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{

    // Service implementation for speedrun management
    // Handles all business logic for run submissions
    public class RunService : IRunService
    {

        private readonly SpeedrunDbContext _context;
        private readonly ILogger<RunService> _logger;
        public RunService(SpeedrunDbContext context, ILogger<RunService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Get all runs for a specific game, sorted by time (fastest first)
        public List<Run> GetRunsByGame(int gameId)
        {
            _logger.LogInformation($"[{GetType().Name}] Fetching runs for game ID: {gameId}");

            var runs = _context.Runs
                .Where(r => r.GameId == gameId) // filter runs by game id
                .ToList()
                .OrderBy(r => r.Time)           // sort runs by time (fastest first)
                .ToList();                      // execute query and return list

            _logger.LogInformation($"[{GetType().Name}] Retrieved {runs.Count} runs for game {gameId}");
            return runs;
        }


        // Get a single run by ID
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

        // Create a new run and save it to the database
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

        // Partially updates a run (PATCH)
        // Only modifies fields that are provided (non-null)
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

        // Completely replaces a run with new data (PUT)
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

        // Removes a run from the system
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