using FIX.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

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
    }
}