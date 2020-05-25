using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;


namespace cw5.Services
{
    public class AuthenticationResult
    {
        public Claim[] Claims { get; set; }
        public string RefreshToken { get; set; }

    }
}
