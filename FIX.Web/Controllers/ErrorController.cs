using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult UnauthorizedAccess()
        {
            Response.AddHeader("REFRESH", "7;URL=/Account/Login");
            Response.StatusCode = 401;
            return View();
        }

        public ActionResult NotFound()
        {
            TempData["BackUrl"] = Request.UrlReferrer?.ToString();
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}