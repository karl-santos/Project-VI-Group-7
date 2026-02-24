using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    /// <summary>
    /// Represents a comment left on a speedrun submission.
    /// </summary>
    public class Comment
    {
        /// <summary>The unique identifier of the comment.</summary>
        public int Id { get; set; }

        /// <summary>The ID of the run this comment belongs to.</summary>
        public int RunId { get; set; }

        /// <summary>The username of the commenter.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>The content of the comment.</summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>The date and time the comment was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>The run this comment belongs to.</summary>
        [JsonIgnore]
        public Run? Run { get; set; }
    }
}
