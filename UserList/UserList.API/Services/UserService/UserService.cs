using UserList.Domain.Entities;
using UserList.Domain.Models;

namespace UserList.API.Services.UserService
{
    public class UserService : IUserService
    {
        public Task<ResponseData<User>> CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<User>> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<ListModel<User>>> GetUserListAsync(int pageNo = 1, int pageSize = 3)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(int id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
