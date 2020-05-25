using System.Threading.Tasks;
using BattleShip.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace BattleShip.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController: ControllerBase
    {
        private readonly IGameTrackerService _gameTrackerService;

        public GameController(IGameTrackerService gameTrackerService)
        {
            _gameTrackerService = gameTrackerService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok("Game"));
        }
    }
}