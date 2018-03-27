using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Api.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}
