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
            User user = await _users.Find(u => u.Name == username && u.Password == password).FirstOrDefaultAsync();
            if (user != null)
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
    }
}
