using FIX.Service;
using FIX.Service.Entities;
using FIX.Service.Models.Repositories;
using FIX.Web.Helpers;
using FIX.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private IUnitOfWork uow;

        public BaseController()
        {
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

        public void Success(string message, bool autoDisappear = false, bool dismissable = false)
        {
            AddAlert(AlertModel.AlertStyles.Success, autoDisappear, message, dismissable);
        }

        public void Information(string message, bool autoDisappear = false, bool dismissable = false)
        {
            AddAlert(AlertModel.AlertStyles.Information, autoDisappear, message, dismissable);
        }

        public void Warning(string message, bool autoDisappear = false, bool dismissable = false)
        {
            AddAlert(AlertModel.AlertStyles.Warning, autoDisappear, message, dismissable);
        }

        public void Danger(string message, bool autoDisappear = false, bool dismissable = false)
        {
            AddAlert(AlertModel.AlertStyles.Danger, autoDisappear, message, dismissable);
        }

        private void AddAlert(string alertStyle, bool autoDisappear, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(AlertModel.TempDataKey)
                ? (List<AlertModel>)TempData[AlertModel.TempDataKey]
                : new List<AlertModel>();

            alerts.Add(new AlertModel
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable,
                AutoDisappear = autoDisappear
            });

            TempData[AlertModel.TempDataKey] = alerts;
        }
    }
}