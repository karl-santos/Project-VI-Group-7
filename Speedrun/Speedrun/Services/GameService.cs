using Speedrun.Data;
using Speedrun.Models;
namespace Speedrun.Services
{
    // Service implementation for game management
    public class GameService : IGameService
    {
        private readonly SpeedrunDbContext _context;
        public GameService(SpeedrunDbContext context)
        {
            _context = context;
        }

        // Returns all games available for speedrunning
        public List<Game> GetAllGames()
        {
            return _context.Games.ToList();
        }

        // Finds a specific game by Id. Returns null if not found
        public Game? GetGameById(int id)
        {
            return _context.Games.FirstOrDefault(g => g.Id == id);
        }

    }
}
