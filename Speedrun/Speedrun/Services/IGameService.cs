using Speedrun.Models;

namespace Speedrun.Services
{
    // interface for game related operations
    // handles retrieval of game data
    public interface IGameService
    {
        List<Game> GetAllGames(); // Retrieves all available games in the system
        Game? GetGameById(int id); // gets a game by its unique identifier, returns null if not found
    }
}
