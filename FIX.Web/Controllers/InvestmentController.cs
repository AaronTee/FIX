using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using FIX.Web.Extensions;
using FIX.Service;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class InvestmentController : BaseController
    {
        IUserService _userService;

        public InvestmentController(IUserService userService)
        {
            _userService = userService;
        }

        //public JsonResult UserPackage()
        //{

        //    return Json();
        //}

        //public JsonResult UserPackageProgress()
        //{

        //    return Json();
        //}



        public ActionResult Index()
        {
            return View();
        }
    }
}