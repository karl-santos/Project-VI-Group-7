using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    public class Run
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., "Any%", "100%", "Glitchless"



        public long TimeMs { get; set; }  // Store as milliseconds in DB

        [NotMapped]  // Don't store this in DB
        public TimeSpan Time
        {
            get => TimeSpan.FromMilliseconds(TimeMs);
            set => TimeMs = (long)value.TotalMilliseconds;
        }


        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public string? VideoUrl { get; set; }
        public string? Notes { get; set; }

        [JsonIgnore]
        public Game? Game { get; set; } // One run belongs to one game

        [JsonIgnore]
        public List<Comment> Comments { get; set; } = new(); // One run has many comments

    }
}
