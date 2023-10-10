using Microsoft.EntityFrameworkCore;
using UserList.API.Data;

namespace UserList.API.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext appDbContext, IConfiguration configuration)
        {
            _context = appDbContext;
        }

        public async Task<bool> DoesRoleExistAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(u => u.Id == roleId);
        }
    }
}
