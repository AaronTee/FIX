using FIX.Data;
using FIX.Service.Interface;
using FIX.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FIX.Web.Controllers
{
    public class BaseController : Controller
    {
        protected static readonly HttpClient client = new HttpClient();
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IBaseService _baseService;

        public BaseController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string cookie_cultureName = AppSettingsHelper.GetKeyValue("Cookie_CultureName");

            if (RouteData.Values["lang"] != null &&
                !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
            {
                // set the culture from the route data (url)
                var lang = RouteData.Values["lang"].ToString();
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
            }
            else
            {
                // load the culture info from the cookie
                var cookie = HttpContext.Request.Cookies[cookie_cultureName];
                var langHeader = string.Empty;
                if (cookie != null)
                {
                    // set the culture by the cookie content
                    langHeader = cookie.Value;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                else
                {
                    // set the culture by the location if not speicified
                    langHeader = HttpContext.Request.UserLanguages[0];
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }
                // set the lang value into route data
                RouteData.Values["lang"] = langHeader;
            }

            // save the location into cookie
            HttpCookie _cookie = new HttpCookie(cookie_cultureName, Thread.CurrentThread.CurrentUICulture.Name);
            _cookie.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.SetCookie(_cookie);

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Save changes in Dbcontext. Return true if succeed.
        /// </summary>
        /// <returns></returns>
        protected bool Save()
        {
            try
            {
                _baseService.Save();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }
    }
}