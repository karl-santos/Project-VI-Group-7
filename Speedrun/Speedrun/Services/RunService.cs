using Speedrun.Models;

namespace Speedrun.Services
{

    // Service implementation for speedrun management
    // Handles all business logic for run submissions
    public class RunService : IRunService
    {
        private static List<Run> _runs = new();  // Static list maintains run data across requests
        private static int _nextId = 1; // Auto-incrementing ID generator


        // Retrieves all runs for a specific game, sorted by time
        // Fastest times appear first for leaderboard display
        public List<Run> GetRunsByGame(int gameId)
        {
            return _runs
                .Where(r => r.GameId == gameId)
                .OrderBy(r => r.Time)
                .ToList();
        }

        // Finds a specific run by ID
        public Run? GetRunById(int id)
        {
            return _runs.FirstOrDefault(r => r.Id == id);
        }

        // Creates a new speedrun submission
        // Automatically assigns ID and timestamp
        public Run CreateRun(int gameId, string playerName, string category, TimeSpan time, string? videoUrl, string? notes)
        {
            var run = new Run
            {
                Id = _nextId++,
                GameId = gameId,
                PlayerName = playerName,
                Category = category,
                Time = time,
                SubmittedAt = DateTime.UtcNow,
                VideoUrl = videoUrl,
                Notes = notes
            };

            _runs.Add(run);
            return run;
        }

        // Partially updates a run (PATCH)
        // Only modifies fields that are provided (non-null)
        public Run? UpdateRun(int id, TimeSpan? time, string? videoUrl, string? notes)
        {
            var run = _runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return null;

            if (time.HasValue) run.Time = time.Value;
            if (videoUrl != null) run.VideoUrl = videoUrl;
            if (notes != null) run.Notes = notes;

            return run;
        }

        // Completely replaces a run with new data (PUT)
        // Updates all fields except ID
        public Run? ReplaceRun(int id, Run updatedRun)
        {
            var run = _runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return null;

            run.PlayerName = updatedRun.PlayerName;
            run.Category = updatedRun.Category;
            run.Time = updatedRun.Time;
            run.VideoUrl = updatedRun.VideoUrl;
            run.Notes = updatedRun.Notes;

            return run;
        }

        // Removes a run from the system
        public bool DeleteRun(int id)
        {
            var run = _runs.FirstOrDefault(r => r.Id == id);
            if (run == null) return false;

            _runs.Remove(run);
            return true;
        }
    }
}