using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using FIX.Web.Models;
using Newtonsoft.Json;
using FIX.Web.Helpers;
using System.Net;
using System.Threading;
using FIX.Service;
using System.Net.Mail;
using FIX.Service.Entities;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
#if !DEBUG
                if (!IsRecapchaValidate())
                {
                    ModelState.AddModelError("", "Captcha is not valid. Please try again.");
                    return View(model);
                }
#endif
                if (await _userService.IsValid(model.Username, model.Password))
                {
                    var currentUser = _userService.GetUserBy(model.Username);
                    var userRole = _userService.GetUserRoleBy(currentUser.UserProfile);

                    var ident = new ClaimsIdentity(new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, model.Username),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, userRole.Description)

                },
                    DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);

                    return RedirectToAction("Index", "Home"); // auth succeed 
                }

                // invalid username or password
                ModelState.AddModelError("", "invalid username or password");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return View();
        }

        private bool IsRecapchaValidate()
        {
            var response = Request["g-recaptcha-response"];
            string secretKey = AppSettingsHelper.GetKeyValue("GoogleRecaptchaSecretKey");

            Log.Info("Response : " + response);
            Log.Info("Key : " + secretKey);

            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));

            Log.Info("Result : " + result);

            var JResponse = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(result);

            return JResponse.success;
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //Clear cookie
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Activation()
        {
            ViewBag.Title = "Email Verification";
            ViewBag.Message = "Verification Code is not valid, either has been removed or expired. Please log in to your account and request another email verification code.";

            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                if (_userService.ValidateActivationCode(activationCode)) ViewBag.Message = "Email has been successfully verified.";
                ViewBag.Url = "/Account/Login";
                ViewBag.UrlTitle = "Login to your account";
            }

            return View("RequiredAction");
        }

        public void SendActivationEmail(int userId)
        {
            var user = _userService.GetUserBy(userId);

            var activationCode = _userService.AssignNewValidationCode(user);
            _userService.SaveChanges();

            using (var mm = new MailMessage())
            {
                string body = "Hello " + user.Username + ",";
                body += "<br /><br />Please click the following link to activate your account";
                body += "<br /><a href = '" + string.Format("{0}://{1}/Home/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />Thanks";
                
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("aarontee.tech@gmail.com", "asd123ASD123");
                smtp.UseDefaultCredentials = true;
                smtp.Port = 587;
                smtp.Timeout = 20000;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                mm.From = new MailAddress("aarontee.tech@gmail.com");
                mm.To.Add(user.Email);
                mm.Subject = "Account Activation";
                mm.Body = body;
                mm.IsBodyHtml = true;
                mm.BodyEncoding = System.Text.Encoding.UTF8;
                mm.SubjectEncoding = System.Text.Encoding.Default;
                try
                {
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
                finally
                {
                    mm.Dispose();
                }
            }
        }

    }
}