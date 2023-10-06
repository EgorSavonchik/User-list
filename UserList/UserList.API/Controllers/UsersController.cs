using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserList.API.Data;
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
        private UserValidator _userValidator;

        public UsersController(IUserService userService, IRoleService roleService, UserValidator userValidator)
        {
            _userService = userService;
            _roleService = roleService;
            _userValidator = userValidator;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<ResponseData<ListModel<User>>>> GetUsers(int pageNo = 1, int pageSize = 10)
        {
            return Ok(await _userService.GetUserListAsync(pageNo, pageSize));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user.Data == null)
            {
                return NotFound(id); // спросить, стоит ли так возращать или только NotFound()
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (!(await _userValidator.Validate(user)))
            {
                return BadRequest();
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
            if(!(await _userValidator.Validate(user)))
            {
                return BadRequest();
            }

            await _userService.CreateUserAsync(user);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!(await _userService.DoesUserExistAsync(id)))
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);

            return Ok("Пользователь с id = " + id + " успешно удален");
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("add-role/{id}")]
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
