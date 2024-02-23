using Microsoft.AspNetCore.Mvc;

namespace ZeissJwtDemo.Apis
{
    [Route("api/v1/sample")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Greet()
        {
            return Ok(await Task.FromResult("Good morning"));
        }
    }
}
