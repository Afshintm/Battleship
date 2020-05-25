using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BattleShip.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class BoardController: ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.FromResult(Ok(1));
        }

    }
}