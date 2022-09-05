using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config) {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

    }
        public string CreateToken(AppUser user)
        {
             var tokenHandler = new JwtSecurityTokenHandler();
             var claims = new List<Claim>                               //How our claims look like
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
                };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);       //what are the credentials to be approved
            var tokenDescriptor = new SecurityTokenDescriptor               //Describe our token
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}