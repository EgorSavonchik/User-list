using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using UserList.API.Services.UserService;
using UserList.Domain.Entities;

namespace UserList.API.Util.Validators
{
    public class UserValidator
    {
        private readonly string emailFormatRegex = @"^[\w-]+@[\w-]+\.{1}[a-zA-Z]+$";

        public UserValidatorResponse Validate(User? user)
        {
            if(user == null)
            {
                return new UserValidatorResponse
                {
                    Success = false,
                    ErrorMessage = "User is null"
                };
            }

            if(user.Email == null || user.Age == null || user.Name == null) 
            {
                return new UserValidatorResponse
                {
                    Success = false,
                    ErrorMessage = "One or more required fields are empty"
                };
            }

            if(!Regex.IsMatch(user.Email, emailFormatRegex))
            {
                return new UserValidatorResponse
                {
                    Success = false,
                    ErrorMessage = "Incorrect email format"
                };
            }

            if(user.Age < 0)
            {
                return new UserValidatorResponse
                {
                    Success = false,
                    ErrorMessage = "Age is not a positive number"
                };
            }

            return new UserValidatorResponse
            {
                Success = true
            };
        }
    }
}
