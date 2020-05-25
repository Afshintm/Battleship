using System.Threading.Tasks;
using BattleShip.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BattleShip.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class BoardController: ControllerBase
    {
        private readonly IGameTrackerService _gameTrackerService;
        public BoardController(IGameTrackerService gameTrackerService)
        {
            _gameTrackerService = gameTrackerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok(_gameTrackerService.GetGameStatus()));
        }

    }
}