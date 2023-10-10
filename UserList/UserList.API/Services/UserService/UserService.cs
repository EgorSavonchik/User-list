using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using UserList.API.Data;
using UserList.API.DTO;
using UserList.API.Util.Encoder;
using UserList.API.Util.Validators;
using UserList.Domain.Entities;
using UserList.Domain.Models;

namespace UserList.API.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly int _maxPageSize;
        private readonly AppDbContext _context;
        private UserValidator _userValidator;
        private IPasswordHasher _passwordHasher;


        public UserService(AppDbContext appDbContext, IConfiguration configuration, UserValidator userValidator, IPasswordHasher passwordHasher)
        {
            _context = appDbContext;
            _maxPageSize = Convert.ToInt32(configuration.GetSection("ItemsPerPage").Value);
            _userValidator = userValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseData<User>> CreateUserAsync(User user) 
        {
            var valResponse = _userValidator.Validate(user);

            if (!valResponse.Success) 
            {
                return new ResponseData<User>
                {
                    Success = false,
                    ErrorMessage = valResponse.ErrorMessage
                };
            }

            if (!(await IsEmailUnique(user.Email)))
            {
                return new ResponseData<User>
                {
                    Success = false,
                    ErrorMessage = "A user with this email already exists"
                };
            }

            var roles = user.Roles.ToList();
            user.Roles.Clear();

            foreach(var role in roles)
            {
                user.Roles.Add(_context.Roles.Find(role.Id));
            };

            user.Password = _passwordHasher.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ResponseData<User>
            {
                Data = user,
                Success = true,
                ErrorMessage = null
            };
        }

        public async Task DeleteUserAsync(int id)
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

        public async Task<ResponseData<ListModel<User>>> GetUserListAsync(int pageNo = 1, int pageSize = 10, UserFilterParameters? parameters = null)
        {
            if (pageSize > _maxPageSize)
            {
                pageSize = _maxPageSize;
            }

            var query = _context.Users.AsQueryable();
            var dataList = new ListModel<User>();

            // количество элементов в списке
            var count = query.Count();
            if (query.Count() == 0)
            {
                return new ResponseData<ListModel<User>>
                {
                    Data = dataList
                };
            }

            // количество страниц
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (pageNo > totalPages || pageNo <= 0)
            {
                return new ResponseData<ListModel<User>>
                {
                    Data = null,
                    Success = false,
                    ErrorMessage = "No such page"
                };
            }

            // фильтрация
            if (parameters != null &&  parameters.FilterBy != null && parameters.FilterValue != null)
            {
                var property = typeof(User).GetProperty(parameters.FilterBy);

                if (property == null || property.Name == "Roles")
                {
                    return new ResponseData<ListModel<User>>
                    {
                        Data = null,
                        Success = false,
                        ErrorMessage = "Incorrect filter field"
                    };
                }

                query = query
                    .Where(user => EF.Property<string>(user, parameters.FilterBy) != null
                                && EF.Property<string>(user, parameters.FilterBy) == parameters.FilterValue).AsQueryable();

                //если после выборки остался пустой список
                totalPages = (int)Math.Ceiling(query.Count() / (double)pageSize);
                if (totalPages == 0)
                {
                    return new ResponseData<ListModel<User>>
                    {
                        Data = dataList
                    };
                }
            }

            // сортировка
            if (parameters != null && parameters.SortBy != null)
            {
                PropertyInfo? property = typeof(User).GetProperty(parameters.SortBy);

                if (property == null)
                {
                    return new ResponseData<ListModel<User>>
                    {
                        Data = null,
                        Success = false,
                        ErrorMessage = "Incorrect sorted field"
                    };
                }

                if (parameters.Ascending != null ? parameters.Ascending.Value : true)
                {
                    if (property.Name == "Roles")
                    {
                        query = query.Include(u => u.Roles)
                            .OrderBy(user => user.Roles.Max(role => role.Id))
                            .AsQueryable();
                    }
                    else
                    {
                        query =  query.OrderBy(u => EF.Property<string>(u, parameters.SortBy)).AsQueryable();
                    }
                }
                else
                {
                    if (property.Name == "Roles")
                    {
                        query = query.Include(u => u.Roles)
                            .OrderByDescending(user => user.Roles.Max(role => role.Id))
                            .AsQueryable();
                    }
                    else
                    {
                        query = query.OrderByDescending(u => EF.Property<string>(u, parameters.SortBy)).AsQueryable();
                    }
                }
            }

            //выборка по ролям
            if(parameters != null && (parameters.RoleId != null|| parameters.RoleName != null)) 
            {
                if(parameters.RoleId != null && parameters.RoleName != null) 
                {
                    if(_context.Roles.Where(r => r.Id == parameters.RoleId).First().Name != parameters.RoleName)
                    {
                        return new ResponseData<ListModel<User>>
                        {
                            Success = false,
                            ErrorMessage = "Incorrect role parameter"
                        };
                    }
                }

                query = query.Include(u => u.Roles).AsQueryable();

                if (parameters.RoleId != null)
                {
                    query = query.Include(u => u.Roles)
                        .Where(u => u.Roles.Any(r => r.Id == parameters.RoleId))
                        .AsQueryable();
                }
                else if(parameters.RoleName != null) 
                {
                    query = query.Include(u => u.Roles)
                        .Where(u => u.Roles.Any(r => r.Name == parameters.RoleName))
                        .AsQueryable();
                }
            }

            // чтобы избежать избыточной загрузки ролей через Include
            if (parameters != null && (parameters.RoleId != null || parameters.RoleName != null || parameters.SortBy == "Roles"))
            {
                dataList.Items = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                dataList.Items = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).Include(u => u.Roles).ToListAsync();
            }

            dataList.CurrentPage = pageNo;
            dataList.TotalPages = totalPages;

            var response = new ResponseData<ListModel<User>>
            {
                Data = dataList
            };

            return response;
        }

        public async Task<ResponseData<User>> UpdateUserAsync(int id, User user)
        {
            var valResponse = _userValidator.Validate(user);

            if (!valResponse.Success)
            {
                return new ResponseData<User>
                {
                    Success = false,
                    ErrorMessage = valResponse.ErrorMessage
                };
            }

            var existingUser = await _context.Users.FindAsync(id);
            user.Id = id;

            if (existingUser != null)
            {
                if (!(await IsEmailUnique(user.Email)) && user.Email != existingUser.Email)
                {
                    return new ResponseData<User>
                    {
                        Success = false,
                        ErrorMessage = "A user with this email already exists"
                    };
                }

                user.Password = _passwordHasher.HashPassword(user.Password);

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

            return new ResponseData<User>
            {
                Data = user,
                Success = true
            };
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

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
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
