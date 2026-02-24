using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    /// <summary>
    /// Represents a game that users can submit speedruns for.
    /// </summary>
    public class Game
    {
        /// <summary>The unique identifier of the game.</summary>
        public int Id { get; set; }

        /// <summary>The name of the game.</summary>
        public string Name { get; set; }

        /// <summary>The URL of the game's cover image.</summary>
        public string GameImageUrl { get; set; } = string.Empty;


        /// <summary>The list of runs submitted for this game.</summary>
        [JsonIgnore]
        public List<Run> Runs { get; set; } = new();

    }

}
