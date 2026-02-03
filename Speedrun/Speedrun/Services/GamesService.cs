using Speedrun.Models;
namespace Speedrun.Services
{
    public class GamesService
    {
        public List<Game> Games { get; set; } = new();

        public GamesService()
        {
        }

        public Game? GetGameById(int id)
        {
            return Games.FirstOrDefault(g => g.Id == id);
        }
        public void RemoveGameById(int id)
        {
            var game = GetGameById(id);
            if (game != null)
            {
                Games.Remove(game);
            }
        }

        public void AddGame(Game game)
        {
            Games.Add(game);
        }

        public List<Game> GetAllGames()
        {
            return Games;
        }

    }
}
