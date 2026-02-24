using Speedrun.Models;

namespace Speedrun.Services
{
    /// <summary>
    /// Interface for game related operations.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Retrieves all available games in the system.
        /// </summary>
        List<Game> GetAllGames();


        /// <summary>
        /// Gets a game by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the game.</param>
        /// <returns>The game if found, null otherwise.</returns>
        Game? GetGameById(int id);
    }
}
