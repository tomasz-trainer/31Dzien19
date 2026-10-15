using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P06Shop.Shared;
using P06Shop.Shared.Auth;
using P06Shop.Shared.Services.AuthService;
using System.Security.Claims;

namespace P05Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  // [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly AuthApiClientService _authService;

        public AuthController(AuthApiClientService authService)
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

        [HttpPost("changePassword"), Authorize]

        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string newPassword)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _authService.ChangePassword(userId, newPassword);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}
