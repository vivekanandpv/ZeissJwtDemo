using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeissJwtDemo.Context;

namespace ZeissJwtDemo.Apis
{
    [Authorize(Policy = "Admin")]
    [Route("api/v1/persons")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        [HttpGet("authenticated")]
        public async Task<IActionResult> GetAuthenticatedAsync()
        {
            return Ok(await Task.FromResult("Authenticated: OK"));
        }

        [HttpGet("authorized")]
        public async Task<IActionResult> GetAuthorizedAsync()
        {
            return Ok(await Task.FromResult("Authorized: OK"));
        }

        [AllowAnonymous]
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenAsync()
        {
            return Ok(await Task.FromResult("Open: OK"));
        }
    }
}
