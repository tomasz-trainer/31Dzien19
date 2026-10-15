using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P06Shop.Shared;
using P06Shop.Shared.Auth;
using P06Shop.Shared.Services.AuthService;

namespace P05Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  // [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var response = await _authService.Login(userLoginDto.Email, userLoginDto.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var user = new User
            {
                UserName = userRegisterDto.UserName,
                Email = userRegisterDto.Email
            };
            var response = await _authService.Register(user, userRegisterDto.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("secret"), Authorize]
        public string SecretKey()
        {
            return "This is my secret key for JWT token generation";
        }



    }
}
