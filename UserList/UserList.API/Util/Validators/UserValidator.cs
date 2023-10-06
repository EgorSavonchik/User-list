using UserList.API.Services.UserService;
using UserList.Domain.Entities;

namespace UserList.API.Util.Validators
{
    public class UserValidator
    {
        IUserService _userService;
        public UserValidator(IUserService userService) 
        {
            _userService = userService;
        }

        public async Task<bool> Validate(User? user)
        {
            if(user == null)
            {
                return false;
            }

            if(user.Email != null && user.Age != null && user.Name != null) 
            {
                return false;
            }

            if(user.Age < 0)
            {
                return false;
            }

            if(!(await _userService.IsEmailUnique(user.Email))) 
            { 
                return false;
            }

            return true;
        }
    }
}
