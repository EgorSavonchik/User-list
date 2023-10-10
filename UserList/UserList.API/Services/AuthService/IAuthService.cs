using UserList.Domain.Entities;

namespace UserList.API.Services.AuthService
{
    public interface IAuthService
    {
        /// <summary>
        /// Creating a jwt token with parameters set in the configuration file for a user
        /// </summary>
        /// <param name="user">User for whom the token will be created</param>
        /// <returns>Jwt token</returns>
        public string GenerateToken(User user);

        /// <summary>
        /// Verify user credentials
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="password">User password</param>
        /// <returns>True if these are valid credentials, false if invalid</returns>
        public Task<bool> VerifyCredentialsAsync(string email, string password);
    }
}
