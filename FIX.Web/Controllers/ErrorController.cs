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
            return View();
        }

        public ActionResult NotFound()
        {
            TempData["BackUrl"] = Request.UrlReferrer?.ToString();
            return View();
        }
    }
}