using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserList.API.Data;
using UserList.API.DTO;
using UserList.API.Services.RoleService;
using UserList.API.Services.UserService;
using UserList.API.Util.Validators;
using UserList.Domain.Entities;
using UserList.Domain.Models;

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
            _logger.LogInformation("Logger create");
        }

        // GET: api/Users
        [HttpGet("", Name = "GetUsersFirstPage")]
        [HttpGet("page{pageNo:int}", Name = "GetUsersPerPage")]
        public async Task<ActionResult<ResponseData<ListModel<User>>>> GetUsers([FromQuery] UserFilterParameters parameters, int pageNo = 1, int pageSize = 10)
        {
            _logger.LogInformation("Method 'GetUsers' called with parameters: {@Parameters}, PageNo: {PageNo}, PageSize: {PageSize}", 
                parameters, pageNo, pageSize);
            var res = await _userService.GetUserListAsync(pageNo, pageSize, parameters);

            if(!res.Success)
            {
                _logger.LogWarning("The method failed : {ErrorMessage}", res.ErrorMessage);
                return BadRequest(res.ErrorMessage);
            }

            _logger.LogInformation("Method 'GetUsers' returned page with totalPages : {TotalPages}, currentPages : {CurentPages}", 
                res.Data.TotalPages, res.Data.CurrentPage);
            return Ok(res);
        }

        // GET: api/Users/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            
            var user = await _userService.GetUserByIdAsync(id);

            if (user.Data == null)
            {
                return NotFound(user.ErrorMessage); 
            }

            _logger.LogInformation($"User [{id}]");
            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            // Добавлять отдельно проверку на нал не стоит, тк пустой пользователь и не пройдет валидацию
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(errors);
            }

            if (!(await _userService.DoesUserExistAsync(id)))
            {
                return BadRequest();
            }

            var res = await _userService.UpdateUserAsync(id, user);

            if(!res.Success)
            {
                return BadRequest(res.ErrorMessage);
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
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

            if(!res.Success)
            {
                return BadRequest(res.ErrorMessage);
            }

            return CreatedAtAction("GetUser", new { id = res.Data.Id }, res.Data);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!(await _userService.DoesUserExistAsync(id)))
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);

            return Ok("Пользователь с id = " + id + " успешно удален");
        }

        // PUT: api/users/add-role/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("add-role/{id:int}")]
        public async Task<ActionResult> AddRole(int id, Role role)
        {
            if(!(await _userService.DoesUserExistAsync(id)))
            {
                return NotFound();
            }

            if(!(await _roleService.DoesRoleExistAsync(role.Id)))
            {
                return BadRequest();
            }

            await _userService.AddRoleAsync(id, role);

            return NoContent();
        }
    }
}
