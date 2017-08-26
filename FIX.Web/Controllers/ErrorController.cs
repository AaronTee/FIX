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
            return View();
        }

        public ActionResult NotFound()
        {
            TempData["BackUrl"] = Request.UrlReferrer?.ToString();
            return View();
        }

        public ActionResult ServerError()
        {
            return View();
        }
    }
}