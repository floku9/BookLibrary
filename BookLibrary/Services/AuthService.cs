using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookLibrary.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace BookLibrary.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;

        public AuthService(IBooksLibraryDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _users = db.GetCollection<User>(settings.UsersCollectionName);
        }

        public string GetToken(ClaimsIdentity identity)
        {
            var now = DateTime.Now;
            var token = new JwtSecurityToken(
                    issuer: AuthSettings.ISSUER,
                    audience: AuthSettings.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthSettings.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<ClaimsIdentity> GetIdentityAsync(string username, string password)
        {
            User user = await _users.Find(u => u.Name == username).FirstOrDefaultAsync();
            if (user != null)
            {
                if (IsPasswordValid(user, password))
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
                };

                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                return null;
            }
            return null;
        }
        
        public async Task AddUserAsync(string username, string password)
        {
            User user = new User()
            {
                Name = username,
                Salt = Guid.NewGuid().ToString(),
                Role = Models.Enums.Roles.User
            };

            user.Password = EncodePassword(password, user.Salt);
            await _users.InsertOneAsync(user);
        }

        private static bool IsPasswordValid(User user, string password) => user.Password == EncodePassword(password, user.Salt);

        public async Task<bool> UserAlreadyExistAsync(string username) => await _users.Find(u => u.Name == username).AnyAsync();

        private static string EncodePassword(string password, string salt)
        {
            var hMACMD5 = new HMACMD5(Encoding.UTF8.GetBytes(salt));
            var saltedHash = hMACMD5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(saltedHash);
        }
    }
}
