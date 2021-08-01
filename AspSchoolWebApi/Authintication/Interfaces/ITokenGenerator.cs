using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspSchoolWebApi.Authintication.Interfaces
{
  public  interface ITokenGenerator
    {
       
        
        

        
        public string GenereteToken(List<Claim> claims);
        public ClaimsPrincipal Decode(string Token);


    }
}
