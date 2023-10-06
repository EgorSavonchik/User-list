using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using UserList.API.Data;
using UserList.Domain.Entities;
using UserList.Domain.Models;

namespace UserList.API.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly int _maxPageSize;
        private readonly AppDbContext _context;


        public UserService(AppDbContext appDbContext, IConfiguration configuration)
        {
            _context = appDbContext;
            _maxPageSize = Convert.ToInt32(configuration.GetSection("ItemsPerPage").Value);
        }

        public async Task<ResponseData<User>> CreateUserAsync(User user) // добавить валидацию
        {

            var roles = user.Roles.ToList();
            user.Roles.Clear();

            foreach(var role in roles)
            {
                user.Roles.Add(_context.Roles.Find(role.Id));
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ResponseData<User>
            {
                Data = user,
                Success = true,
                ErrorMessage = null
            };
        }

        public async Task DeleteUserAsync(int id) // млжет быть переделать чтобы возращало бул
        {
            User? user = await _context.Users.FindAsync(id);

            if(user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ResponseData<User>> GetUserByIdAsync(int id)
        {
            var res = await _context.Users.FindAsync(id);
            
            if (res == null)
            {
                return new ResponseData<User>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "No element with id = " + id
                };
            }

            _context.Entry(res).Collection(u => u.Roles).Load();
            return new ResponseData<User>
            {
                Data = res,
            };
        }

        public async Task<ResponseData<ListModel<User>>> GetUserListAsync(int pageNo = 1, int pageSize = 10)
        {
            if (pageSize > _maxPageSize)
            {
                pageSize = _maxPageSize;
            }

            var query = _context.Users.AsQueryable().Include(u => u.Roles);
            var dataList = new ListModel<User>();

            // количество элементов в списке
            var count = query.Count();
            if (count == 0)
            {
                return new ResponseData<ListModel<User>>
                {
                    Data = dataList
                };
            }

            // количество страниц
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNo > totalPages)
            {
                return new ResponseData<ListModel<User>>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "No such page"
                };
            }

            dataList.Items = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;

            var response = new ResponseData<ListModel<User>>
            {
                Data = dataList
            };

            return response;
        }

        public async Task UpdateUserAsync(int id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            user.Id = id;

            if (existingUser != null)
            {
                _context.Entry(existingUser).Collection(u => u.Roles).Load();
                _context.Entry(existingUser).CurrentValues.SetValues(user);

                existingUser.Roles.Clear();

                // Добавляем новые роли
                foreach (var role in user.Roles)
                {
                    var newRole = await _context.Roles.FindAsync(role.Id);
                    if (newRole != null)
                    {
                        existingUser.Roles.Add(newRole);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRoleAsync(int id, Role role)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                _context.Entry(user).Collection(u => u.Roles).Load();

                var newRole = await _context.Roles.FindAsync(role.Id);
                if(newRole != null && !user.Roles.Contains(newRole))
                {
                    user.Roles.Add(newRole);

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> DoesUserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return !(await _context.Users.AnyAsync(u => u.Email == email));
        }
    }
}
