using Microsoft.AspNetCore.Identity;

namespace UserList.API.Util.Encoder
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the password
        /// </summary>
        /// <param name="password">Password to be hashed</param>
        /// <returns>Hashed string</returns>
        public string HashPassword(string password);

        /// <summary>
        /// Compares hashed and unhashed passwords
        /// </summary>
        /// <param name="hashedPassword">Hashed password</param>
        /// <param name="providedPassword">Unhashed password</param>
        /// <returns>True if the passwords are equal, false if the passwords are not equal</returns>
        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
