using Microsoft.AspNetCore.Mvc;

namespace HorseRace.Controllers
{
    [Route("api/[controller]")]
    public class PositionsController : Controller
    {
        private readonly HorseRacer _horseRacer;

        public PositionsController(HorseRacer horseRacer)
        {
            _horseRacer = horseRacer;
        }

        [HttpGet]
        public HorsePosition[] Get()
        {
            return _horseRacer.CurrentPositions;
        }
    }
}
