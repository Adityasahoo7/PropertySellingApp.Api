using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace PropertySellingApp.Services.Security
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config) { _config = config; }


        public (string token, DateTime expires) CreateToken(User user)
        {
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60"));


            var claims = new List<Claim>
{
new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
new(JwtRegisteredClaimNames.Email, user.Email),
new(ClaimTypes.Name, user.FullName),
new(ClaimTypes.Role, user.Role.ToString())
};


            var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}
