using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace BattleShip.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController: ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok("Game"));
        }
    }
}