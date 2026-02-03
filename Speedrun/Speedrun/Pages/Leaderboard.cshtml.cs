using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Speedrun.Pages
{
    public class LeaderboardModel : PageModel
    {
        public string _gameId;
        public void OnGet(string gameId)
        {
            _gameId = gameId;
        }
    }
}
