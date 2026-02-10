using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Speedrun.Controllers;
using System.IO;
using Speedrun.Models;
using Speedrun.Services;
using Microsoft.Extensions.Logging;

namespace Speedrun.Data
{
    public class SpeedrunDatabase : ControllerBase
    {
        private readonly string _dbPath;

        [HttpPost]

        public IActionResult Addrun([FromBody] NewRunDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body required" });

            if (string.IsNullOrWhiteSpace(_dbPath) || !System.IO.File.Exists(_dbPath))
            {
                //ILogger.LogError("Database file not found. Looked for: {path}", _dbPath);
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
            catch (Exception)
            {
                //_logger.LogError(ex, "Error inserting run");
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
