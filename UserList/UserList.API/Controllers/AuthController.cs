using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserList.API.Services.AuthService;
using UserList.API.Services.UserService;
using UserList.Domain.Entities;

namespace UserList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            bool correctCredentials = await _authService.VerifyCredentialsAsync(email, password);

            if (correctCredentials) 
            {
                var token = _authService.GenerateToken(await _userService.GetUserByEmail(email));
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest("Incorrect credentials");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(errors);
            }

            var res = await _userService.CreateUserAsync(user);

            if (!res.Success)
            {
                return BadRequest(res.ErrorMessage);
            }

            var token = _authService.GenerateToken(await _userService.GetUserByEmail(user.Email));
            return CreatedAtAction("GetUser", "Users", new { id = res.Data.Id }, new { User = res.Data, Token = token });

        }
    }
}
