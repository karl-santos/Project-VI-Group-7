using Speedrun.Data;
using Speedrun.Models;

namespace Speedrun.Services
{

    // Service implementation for speedrun management
    // Handles all business logic for run submissions
    public class RunService : IRunService
    {

        private readonly SpeedrunDbContext _context;
        public RunService(SpeedrunDbContext context)
        {
            _context = context;
        }


        // Get all runs for a specific game, sorted by time (fastest first)
        public List<Run> GetRunsByGame(int gameId)
        {
            return _context.Runs
                .Where(r => r.GameId == gameId) // filter runs by game id
                .OrderBy(r => r.Time)           // sort runs by time (fastest first)
                .ToList();                      // execute query and return list
        }


        // Get a single run by ID
        public Run? GetRunById(int id)
        {
            // Find run where Id matches, return null if not found
            return _context.Runs.FirstOrDefault(r => r.Id == id); 
        }

        // Create a new run and save it to the database
        public Run CreateRun(int gameId, string playerName, string category, TimeSpan time, string? videoUrl, string? notes)
        {
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

            return run; 
        }

        // Partially updates a run (PATCH)
        // Only modifies fields that are provided (non-null)
        public Run? UpdateRun(int id, TimeSpan? time, string? videoUrl, string? notes)
        {
            // find the run in the database
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return null;

            // Update only the fields that were provided (not null)
            if (time.HasValue) run.Time = time.Value;
            if (videoUrl != null) run.VideoUrl = videoUrl;
            if (notes != null) run.Notes = notes;

            _context.SaveChanges();

            return run;
        }

        // Completely replaces a run with new data (PUT)
        public Run? ReplaceRun(int id, Run updatedRun)
        {
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return null;

            // Updates all fields except ID
            run.PlayerName = updatedRun.PlayerName;
            run.Category = updatedRun.Category;
            run.Time = updatedRun.Time;
            run.VideoUrl = updatedRun.VideoUrl;
            run.Notes = updatedRun.Notes;

            _context.SaveChanges();

            return run;
        }

        // Removes a run from the system
        public bool DeleteRun(int id)
        {
            // find the run
            var run = _context.Runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return false;

            _context.Runs.Remove(run);

            _context.SaveChanges();

            return true;
        }
    }
}