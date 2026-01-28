namespace Speedrun.Models
{
    public class Game
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }


        public List<Run> Runs { get; set; } = new();
    }

}
