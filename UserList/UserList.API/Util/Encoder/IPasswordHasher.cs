using Microsoft.AspNetCore.Identity;

namespace UserList.API.Util.Encoder
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
