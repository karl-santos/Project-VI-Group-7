using System.Text.Json;

namespace Speedrun.Models
{
    public class Run
    {
        public string Id { get; set; }
        public string GameId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., "Any%", "100%", "Glitchless"
        public TimeSpan Time { get; set; } // run duration
        public DateTime Date { get; set; }
        public string? VideoUrl { get; set; }


        public Game? Game { get; set; } // One run belongs to one game
        public List<Comment> Comments { get; set; } = new(); // One run has many comments

    }
}
