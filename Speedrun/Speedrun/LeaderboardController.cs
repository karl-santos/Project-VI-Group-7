using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speedrun.Models;

namespace Speedrun
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderBoardController : Controller
    {
        // GET: GameController
        [HttpGet]
        public ActionResult Index(string gameId)
        {
       
            TimeSpan duration = new TimeSpan(2, 30, 0);
            var u = new List<Run>();
            var g = new Run()
            
            {
                Id = "1",
                Category = "idk",
                Comments = new List<Comment>(), 
                Date = DateTime.Now,
                Game = new Game(),
                GameId = "1",
                PlayerName = "Albert",
                Time = duration,
                VideoUrl = "video" 

            };

            var g2 = new Run()
            {
                Id = "2",
                Category = "idk",
                Comments = new List<Comment>(),
                Date = DateTime.Now,
                Game = new Game(),
                GameId = "2",
                PlayerName = "Alberto",
                Time = duration,
                VideoUrl = "video"
            };

            u.Add(g);
            u.Add(g2);
            u = u.Where(r => r.GameId == gameId).ToList();
            return new JsonResult(u);
        }
    }
}
