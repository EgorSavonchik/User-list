using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserList.API.Services.UserService;
using UserList.API.Util.Encoder;
using UserList.API.Util.Settings;
using UserList.Domain.Entities;

namespace UserList.API.Services.AuthService
{
    public class AuthService : IAuthService
    {
        IConfiguration _configuration;
        IUserService _userService;
        IPasswordHasher _passwordHasher;

        public AuthService(IConfiguration configuration, IUserService userService, IPasswordHasher passwordHasher)
        {
            _configuration = configuration;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };   
            
            foreach(var role in user.Roles)
            {
                claims.Add(new Claim("Role: ", role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings.ExpiryInMinutes)),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtSettings.Audience,
                Issuer = jwtSettings.Issuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> VerifyCredentialsAsync(string email, string password)
        {
            User? user = await _userService.GetUserByEmail(email);
            
            if(user != null && _passwordHasher.VerifyHashedPassword(user.Password, password) == PasswordVerificationResult.Success)
            {
                return true;
            }

            return false;
        }
    }
}
