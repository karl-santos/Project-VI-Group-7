using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Run? Run { get; set; }
    }
}
