using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using UserList.API.Util.Encoder;
using UserList.Domain.Entities;

namespace UserList.API.Util.Hasher
{
    public class PasswordHasherSHA : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            string hashedProvidedPassword = HashPassword(providedPassword);

            if (hashedProvidedPassword == hashedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
