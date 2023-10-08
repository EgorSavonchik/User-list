using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        // GET: api/Users
        [HttpGet("", Name = "GetUsersFirstPage")]
        [HttpGet("page{pageNo:int}", Name = "GetUsersPerPage")]
        public async Task<ActionResult<ResponseData<ListModel<User>>>> GetUsers([FromQuery] UserFilterParameters parameters, int pageNo = 1, int pageSize = 10)
        {
            return Ok(await _userService.GetUserListAsync(pageNo, pageSize, parameters));
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

            await _userService.UpdateUserAsync(id, user);

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
