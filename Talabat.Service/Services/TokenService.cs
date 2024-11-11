using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var Roles = await userManager.GetRolesAsync(user); 
            foreach (var role in Roles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, role)); 
            }

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));
           
            var Token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"], 
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:Expiration"])), 
                claims: AuthClaims, 
                signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature) // Set signing credentials
            );

            Console.WriteLine(Token.ToString()); 
            Console.WriteLine(Token.SecurityKey); 

            string jwt = new JwtSecurityTokenHandler().WriteToken(Token);

            //var tokenParts = jwt.Split('.');
            //string signature = tokenParts[2];

            return jwt; 
        }
    }
}
