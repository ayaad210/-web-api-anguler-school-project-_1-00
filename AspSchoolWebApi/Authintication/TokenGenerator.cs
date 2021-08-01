using AspSchoolWebApi.Authintication.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Authintication
{
    public class TokenGenerator:ITokenGenerator
    {

        protected readonly SymmetricSecurityKey _key;

        protected readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT").GetSection("mysecret").Value.ToString()));

        }




        public string GenereteToken(List<Claim> claims)
        {
           // var claims = new List<Claim>
           // {
           //     new Claim(ClaimTypes.Name,Username)

           //};
            var signingInCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            var descripption = new SecurityTokenDescriptor
            {
                Issuer = _configuration.GetSection("JWT").GetSection("ValidIssuer").Value.ToString(),
                Audience = _configuration.GetSection("JWT").GetSection("ValidAudience").Value.ToString(),
                SigningCredentials = signingInCredentials,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(20)

            };
            var handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descripption);
            return handler.WriteToken(token);





        }

        public ClaimsPrincipal Decode(string Token)
        {
            SecurityToken token;

            var handlerResult = new JwtSecurityTokenHandler().ValidateToken(Token, new TokenValidationParameters
            {
                IssuerSigningKey = _key,
                ValidIssuer = "http://localhost:1444/api",
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero,//متذودش وقت فوق عمرة
                ValidateLifetime = true,


            },out token);
            return handlerResult;




        }

    }
}
