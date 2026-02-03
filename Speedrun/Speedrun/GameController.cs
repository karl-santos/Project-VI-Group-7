using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Speedrun.Models;

namespace Speedrun
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        // GET: GameController
        [HttpGet]
        public ActionResult Index()
        {
            var x = new List<Game>();
            var g = new Game()
            {
                Id = "1",
                Description = "hhhhhh",
                Name = "Game1",
                ReleaseYear = 2000,
                Runs = new List<Run>()
            };

            var g2 = new Game()
            {
                Id = "2",
                Description = "hhhhhh",
                Name = "Game2",
                ReleaseYear = 2000,
                Runs = new List<Run>()
            };

            x.Add(g);
            x.Add(g2);
            return new JsonResult(x);
        }
    }
}
