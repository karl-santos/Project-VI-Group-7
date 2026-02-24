using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    /// <summary>
    /// Represents a speedrun submission for a game.
    /// </summary>
    public class Run
    {
        /// <summary>The unique identifier of the run.</summary>
        public int Id { get; set; }

        /// <summary>The ID of the game this run belongs to.</summary>
        public int GameId { get; set; }

        /// <summary>The name of the player who submitted the run.</summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>The category of the run e.g. Any% or 100%.</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>The completion time stored as milliseconds in the database.</summary>
        public long TimeMs { get; set; }

        /// <summary>
        /// The completion time as a TimeSpan.
        /// Computed from TimeMs, not stored directly in the database.
        /// </summary>
        [NotMapped]
        public TimeSpan Time
        {
            get => TimeSpan.FromMilliseconds(TimeMs);
            set => TimeMs = (long)value.TotalMilliseconds;
        }

        /// <summary>The date and time the run was submitted.</summary>
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        /// <summary>Optional URL to the run video.</summary>
        public string? VideoUrl { get; set; }

        /// <summary>Optional notes about the run.</summary>
        public string? Notes { get; set; }

        /// <summary>The game this run belongs to.</summary>
        [JsonIgnore]
        public Game? Game { get; set; }

        /// <summary>The list of comments on this run.</summary>
        [JsonIgnore]
        public List<Comment> Comments { get; set; } = new();
    }
}
