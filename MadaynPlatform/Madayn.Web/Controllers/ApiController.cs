using Microsoft.AspNetCore.Mvc;

namespace Madayn.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { message = "pong" });
    }
}

