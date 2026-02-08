using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Speedrun.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RunSubmissionController : ControllerBase
    {
        private readonly ILogger<RunSubmissionController> _logger;
        private readonly string _dbPath;

        public RunSubmissionController(ILogger<RunSubmissionController> logger, IConfiguration config)
        {
            _logger = logger;

            // Try to locate the database. 
            _dbPath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_dbPath))
            {
                // Try a few relative paths 
                var tryPaths = new[]
                {
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Project6.db"), // when running from bin/Debug/net*/...
                    Path.Combine(AppContext.BaseDirectory, "..", "Project6.db"),
                    Path.Combine(Directory.GetCurrentDirectory(), "Project6.db"),
                    "/mnt/data/Project6.db" // during local testing 
                };

                foreach (var p in tryPaths)
                {
                    var full = Path.GetFullPath(p);
                    if (System.IO.File.Exists(full))
                    {
                        _dbPath = full;
                        break;
                    }
                }
            }
        }

        // POST api/RunSubmission
        // Expects JSON like:
        // { "gameId": 2, "runnerName": "Alice", "category": "Any%", "runTime": "00:12:34.567", "runDate":"2024-09-01", "gameVersion":"1.0" }
        [HttpPost]
        public IActionResult SubmitRun([FromBody] NewRunDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body required" });

            if (string.IsNullOrWhiteSpace(_dbPath) || !System.IO.File.Exists(_dbPath))
            {
                _logger.LogError("Database file not found. Looked for: {path}", _dbPath);
                return StatusCode(500, new { message = "Database file not found on server" });
            }

            // Basic validation
            if (dto.GameId <= 0 || string.IsNullOrWhiteSpace(dto.RunnerName) || string.IsNullOrWhiteSpace(dto.RunCategory))
            {
                return BadRequest(new { message = "gameId, runnerName and runCategory are required" });
            }

            try
            {
                using var conn = new SqliteConnection($"Data Source={_dbPath}");
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Run (GameID, RunCategory, RunnerName, RunTime, RunDate, GameVersion)
                    VALUES (@gameId, @runCategory, @runnerName, @runTime, @runDate, @gameVersion);
                    SELECT last_insert_rowid();
                ";
                cmd.Parameters.AddWithValue("@gameId", dto.GameId);
                cmd.Parameters.AddWithValue("@runCategory", dto.RunCategory);
                cmd.Parameters.AddWithValue("@runnerName", dto.RunnerName);
                cmd.Parameters.AddWithValue("@runTime", dto.RunTime ?? string.Empty);
                cmd.Parameters.AddWithValue("@runDate", dto.RunDate ?? DateTime.UtcNow.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@gameVersion", dto.GameVersion ?? string.Empty);

                var idObj = cmd.ExecuteScalar();
                long newId = (idObj != null && long.TryParse(idObj.ToString(), out var lid)) ? lid : 0;

                return CreatedAtAction(nameof(GetRunById), new { id = newId }, new { runId = newId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting run");
                return StatusCode(500, new { message = "Error saving run" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetRunById(long id)
        {
            if (string.IsNullOrWhiteSpace(_dbPath) || !System.IO.File.Exists(_dbPath))
                return NotFound();

            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT RunID, GameID, RunCategory, RunnerName, RunTime, RunDate, GameVersion FROM Run WHERE RunID = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return NotFound();

            var run = new
            {
                RunID = rdr.GetInt64(0),
                GameID = rdr.IsDBNull(1) ? (int?)null : rdr.GetInt32(1),
                RunCategory = rdr.IsDBNull(2) ? null : rdr.GetString(2),
                RunnerName = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                RunTime = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                RunDate = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                GameVersion = rdr.IsDBNull(6) ? null : rdr.GetString(6)
            };
            return Ok(run);
        }
    }

    public class NewRunDto
    {
        public int GameId { get; set; }
        public string RunCategory { get; set; } = string.Empty;
        public string RunnerName { get; set; } = string.Empty;
        public string? RunTime { get; set; }
        public string? RunDate { get; set; }
        public string? GameVersion { get; set; }
    }
}
