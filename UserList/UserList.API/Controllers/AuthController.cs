using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserList.API.Services.AuthService;
using UserList.API.Services.UserService;
using UserList.Domain.Entities;

namespace UserList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public AuthController(IAuthService authService, IUserService userService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login to account",
            Description = "Login to account using the specified email and password.",
            OperationId = "Login"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a valid jwt token.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect credentials.")]
        public async Task<IActionResult> Login(
            [SwaggerParameter("User email.")]  string email,
            [SwaggerParameter("User password.")]  string password)
        {
            _logger.LogInformation("Method 'Login' called with email : {email}", email);
            bool correctCredentials = await _authService.VerifyCredentialsAsync(email, password);

            if (correctCredentials) 
            {
                _logger.LogInformation("Method 'Login' completed successfully");
                var token = _authService.GenerateToken(await _userService.GetUserByEmail(email));
                return Ok(new { Token = token });
            }
            else
            {
                _logger.LogWarning("The method failed : Incorrect credendials");
                return BadRequest("Incorrect credentials");
            }
        }

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Account registration",
            Description = "Creates a new user and returns jwt token",
            OperationId = "Registration"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Returns a json with user information and jwt token.", typeof(User))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect user, user with this email already exists.")]
        public async Task<ActionResult> Register(
            [SwaggerParameter("The user that will be created.")] [FromBody]  User user)
        {
            _logger.LogInformation("Method 'Register' called");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                _logger.LogWarning("The method failed : {errors}", errors);
                return BadRequest(errors);
            }

            var res = await _userService.CreateUserAsync(user);

            if (!res.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", res.ErrorMessage);
                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Method 'Register' completed successfully, new user : {user}", res.Data);
            var token = _authService.GenerateToken(res.Data);
            return CreatedAtAction("GetUser", "Users", new { id = res.Data.Id }, new { User = res.Data, Token = token });
        }
    }
}
