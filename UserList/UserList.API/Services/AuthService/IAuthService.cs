using UserList.Domain.Entities;

namespace UserList.API.Services.AuthService
{
    public interface IAuthService
    {
        public string GenerateToken(User user);

        public Task<bool> VerifyCredentialsAsync(string email, string password);
    }
}
