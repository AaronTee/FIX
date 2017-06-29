using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;
using static FIX.Service.DBConstant;

namespace FIX.Web.Extensions
{
    public static class IdentityExtension
    {
        public static string GetUserTimeZone(this IIdentity identity)
        {
            return ((ClaimsIdentity)identity).FindFirst("TimeZone").Value;
        }
    }
}