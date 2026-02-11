using System.Text.Json.Serialization;

namespace Speedrun.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameImageUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Run> Runs { get; set; } = new();

    }

}
