using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Swashbuckle.AspNetCore.Annotations;
using UserList.API.DTO;
using UserList.API.Services.RoleService;
using UserList.API.Services.UserService;
using UserList.Domain.Entities;
using UserList.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UserList.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IRoleService roleService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet("", Name = "GetUsersFirstPage")]
        [HttpGet("page{pageNo:int}", Name = "GetUsersPerPage")]
        [SwaggerOperation(
            Summary = "Get a users",
            Description = "Obtaining a list of users based on specified parameters.",
            OperationId = "GetUsers"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a returns json with user information.", typeof(User))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Incorrect sorted field, incorrect filter field, Incorrect role parameter, no such page.")]
        public async Task<ActionResult<ResponseData<ListModel<User>>>> GetUsers(
            [FromQuery] UserFilterParameters parameters,
            [SwaggerParameter("Page number.")]  int pageNo = 1,
            [SwaggerParameter("Page size.")]  int pageSize = 10)
        {
            _logger.LogInformation("Method 'GetUsers' called with parameters: {@Parameters}, PageNo: {PageNo}, PageSize: {PageSize}", 
                parameters, pageNo, pageSize);
            var res = await _userService.GetUserListAsync(pageNo, pageSize, parameters);

            if(!res.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", res.ErrorMessage);
                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Method 'GetUsers' completed successfully, return page with totalPages : {TotalPages}, currentPages : {CurentPages}", 
                res.Data.TotalPages, res.Data.CurrentPage);
            return Ok(res);
        }

        // GET: api/Users/5
        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get a user",
            Description = "Get user information with a given id.",
            OperationId = "GetUser"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a returns json with user information.", typeof(User))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "There is no user with this Id.")]
        public async Task<ActionResult<User>> GetUser(
            [SwaggerParameter("The unique Id of the user.", Required = true)] int id)
        {
            _logger.LogInformation("Method 'GetUser' called with id : {Id}", id);

            if (!(await _userService.DoesUserExistAsync(id)))
            {
                _logger.LogWarning("The method failed : There is no user with this Id");
                return NotFound("There is no user with this Id");
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (!user.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", user.ErrorMessage);
                return BadRequest(user.ErrorMessage); 
            }

            _logger.LogInformation("Method 'GetUser' completed successfully, return user with id : {Id}", user.Data.Id);
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Updating a user",
            Description = "Updates user information with the one passed in.",
            OperationId = "UpdateUser"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a string with information about the completion of the operation.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The user is not validated, there is no user with this Id, a user with this email already exists.")]
        public async Task<IActionResult> PutUser(
            [SwaggerParameter("The unique Id of the user.", Required = true)] int id,
            [SwaggerParameter("Updated user.")] [FromBody] User user)
        {
            _logger.LogInformation("Method 'PutUser' called with id : {id}", id);
            // Добавлять отдельно проверку на нал не стоит, тк пустой пользователь и не пройдет валидацию
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                _logger.LogWarning("The method failed : {ErrorMessage}", errors);
                return BadRequest(errors);
            }

            if (!(await _userService.DoesUserExistAsync(id)))
            {
                _logger.LogWarning("The method failed : There is no user with this Id");
                return BadRequest("There is no user with this Id");
            }

            var res = await _userService.UpdateUserAsync(id, user);

            if(!res.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", res.ErrorMessage);
                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Method 'PutUser' completed successfully");
            return Ok($"User with id = {res.Data.Id} successfully updated");
        }

        // POST: api/Users
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creating a user",
            Description = "Creates a user with the given fields.",
            OperationId = "CreateUser"
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Returns a json with user information", typeof(User))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The user is not validated, a user with this email already exists.")]
        public async Task<ActionResult<User>> PostUser(
            [SwaggerParameter("information about the user that will be created.")] [FromBody] User user)
        {
            _logger.LogInformation("Method 'PostUser' called");
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                _logger.LogWarning("The method failed : {ErrorMessage}", errors);
                return BadRequest(errors);
            }

            var res = await _userService.CreateUserAsync(user);

            if(!res.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", res.ErrorMessage);
                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Method 'PostUser' completed successfully, new user : {@user}", res.Data);
            return CreatedAtAction("GetUser", new { id = res.Data.Id }, res.Data);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Deleting a user",
            Description = "Deleting a user with a given Id.",
            OperationId = "DeleteUser"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a string with information about the completion of the operation.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with specified Id not found.")]
        public async Task<IActionResult> DeleteUser(
            [SwaggerParameter("The unique Id of the user.", Required = true)] int id)
        {
            _logger.LogInformation("Method 'DeleteUser' called with id : {id}", id);
            if (!(await _userService.DoesUserExistAsync(id)))
            {
                _logger.LogWarning("The method failed : There is no user with this Id");
                return NotFound("There is no user with this Id");
            }

            await _userService.DeleteUserAsync(id);

            _logger.LogInformation("Method 'Delete' completed successfully");
            return Ok($"User with Id = {id} successfully deleted");
        }

        // PUT: api/users/add-role/5
        [HttpPut("add-role/{id:int}")]
        [SwaggerOperation(
            Summary = "Adding a user role",
            Description = "The method allows to add a role to a user by his Id.",
            OperationId = "AddRoleToUser"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a string with information about the completion of the operation.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with specified Id not found, role with specified Id not found.")]
        public async Task<ActionResult> AddRole(
            [SwaggerParameter("The unique Id of the user.", Required = true)]  int id,
            [SwaggerParameter("The role to add to the user.")] [FromBody] Role role)
        {
            _logger.LogInformation("Method 'AddRole' called with id : {id}, role : {role}", id, role);
            if (!(await _userService.DoesUserExistAsync(id)))
            {
                _logger.LogWarning("The method failed : There is no user with this Id");
                return NotFound("There is no user with this Id");
            }

            if(!(await _roleService.DoesRoleExistAsync(role.Id)))
            {
                _logger.LogWarning("The method failed : There is no role with this Id");
                return NotFound("There is no role with this Id");
            }

            await _userService.AddRoleAsync(id, role);

            _logger.LogInformation("Method 'AddRole' completed successfully");
            return Ok($"User with id = {id} get role {role.Name}");
        }
    }
}
