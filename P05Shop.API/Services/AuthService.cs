using Microsoft.EntityFrameworkCore;
using P05Shop.API.Models;
using P06Shop.Shared;
using P06Shop.Shared.Auth;
using P06Shop.Shared.Services.AuthService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace P05Shop.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        public readonly IConfiguration _config;

        public AuthService(DataContext dataContext, IConfiguration config)
        {
            _config = config;
            _context = dataContext;
        }

        public Task<ServiceResponse<bool>> ChangePassword(string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<string>> Login(UserLoginDto userLoginDto)
        {
           
            var user=  await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(userLoginDto.Email.ToLower()));

            if (user == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            else if (!VerifyPasswordHash(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Wrong password."
                };
            }
            else
            {
                return new ServiceResponse<string>
                {
                    Message = "Login successful.",
                    Data = CreateToken(user),
                    Success = true
                };
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("DateCreated", user.DateCreated.ToString())
            };

            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value));

            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha512Signature);


            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public Task<ServiceResponse<int>> Register(UserRegisterDto userRegisterDto)
        {
            throw new NotImplementedException();
        }
    }
}
