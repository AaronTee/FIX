using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Controllers
{
    public class HtmlLoaderController : Controller
    {
        // GET: HtmlLoader
        public IHtmlString Modal()
        {
            var path = HttpContext.Server.MapPath("~/Views/Shared/Html/Modal.html");
            var html = System.IO.File.ReadAllText(path);
            return new MvcHtmlString(html);
        }
    }
}