using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using FIX.Web.Extensions;
using FIX.Service;

namespace SyntrinoWeb.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class IdentityAuthorize : AuthorizeAttribute
    {
        //public DBConstant.DBCRole Role { get; set; } //To be implement

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool pass = true;

            pass &= httpContext.User.Identity.GetUserHasAcceptedTC();
            pass &= !httpContext.User.Identity.GetUserIsFirstTimeLogin();

            return pass;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            filterContext.HttpContext.Response.AddHeader("REFRESH", "5;URL=/Account/Login");
            filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "UnauthorizedAccess" },
                                       { "controller", "Error" }
                                   });

        }
    }
}