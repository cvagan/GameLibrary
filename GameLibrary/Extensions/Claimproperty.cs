using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GameLibrary.Extensions
{
    public static class Claimproperty
    {
        public static string DisplayName(this IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = user.Identity as ClaimsIdentity;
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == "DisplayName")
                    {
                        return claim.Value;
                    }
                }
                return "";
            }
            return "";
        }
    }
}
