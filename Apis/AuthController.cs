using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeissJwtDemo.Services;
using ZeissJwtDemo.ViewModels;

namespace ZeissJwtDemo.Apis
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel viewModel)
        {
            return Ok(await _service.LoginAsync(viewModel));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(UserRegisterViewModel viewModel)
        {
            await _service.RegisterAsync(viewModel);
            return Ok();
        }
    }
}
