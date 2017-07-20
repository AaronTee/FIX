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
using FIX.Web.Extensions;

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

                    //Verify email
                    if (!currentUser.HasEmailVerified)
                    {
                        RequiredActionModel _raModel = new RequiredActionModel
                        {
                            Title = "Pending Email Verification",
                            Controller = "Account",
                            Action = "ActivationEmail",
                            Content = @"Your email has not verified yet, please check your email to activate this account before continue.",
                            SubmitButtonDescription = "Resend Verification Email",
                            FormMethod = FormMethod.Post,
                            DataName = "uid",
                            Data = currentUser.UserId
                        };

                        return View("RequiredAction", _raModel);
                    }
                    var ident = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, currentUser.UserId.ToString()),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim("TimeZone", currentUser.TimeZoneId),
                    new Claim(ClaimTypes.Role, userRole.Description)
                    },
                    DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);

                    return RedirectToAction("Index", "Home");
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

            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));

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
            try
            {
                RequiredActionModel _raModel = new RequiredActionModel
                {
                    Controller = "Account",
                    Action = "Login",
                    FormMethod = FormMethod.Get,
                    SubmitButtonDescription = "Login",
                };

                if (RouteData.Values["id"] != null)
                {
                    var decryptedActivationCode = EncryptionHelper.Base64Decode(RouteData.Values["id"].ToString());
                    Guid activationCode = Guid.ParseExact(decryptedActivationCode, "N");
                    if (_userService.ValidateActivationCode(activationCode))
                    {
                        _raModel.Title = "Successfully verified";
                        _raModel.Content = "Email has been successfully verified. You can now proceed to login.";
                    }
                    else
                    {
                        _raModel.Title = "Invalid Verification Request";
                        _raModel.Content = "Verification link or code is not valid, either has been removed or expired. Please log in to your account to request another email verification code.";
                    }
                    ViewBag.Url = "/Account/Login";
                    ViewBag.UrlTitle = "Login to your account";
                }

                return View("RequiredAction", _raModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return View("Error");
            }
        }

        [AllowAnonymous]
        public ActionResult TermsAndConditions()
        {
            try
            {
                var key = Convert.ToInt32(EncryptionHelper.DecryptText(TempData["uid"].ToString()));

                if (key != 0)
                {
                    _userService.GetUserBy(key);

                    TermsAndContitionsViewModel taModel = new TermsAndContitionsViewModel
                    {
                        Terms = "Some terms here"
                    };
                    return View(taModel);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return View("Error");
        }


        //[HttpPost]
        //public ActionResult TermsAndConditions(TermsAndContitionsViewModel model)
        //{
        //    //var user = _userService.GetUserBy(key);

        //    //LoginViewModel lgModel = new LoginViewModel
        //    //{
        //    //    Username = user.Username,
        //    //    Password = user.Password
        //    //};

        //    return View("Login", lgModel); 
        //} 

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ActivationEmail(int? uid)
        {
            try
            {
                SendActivationEmail(uid);

                RequiredActionModel _raModel = new RequiredActionModel
                {
                    Title = "Email Verification Sent",
                    Controller = "Account",
                    Action = "Login",
                    Content = "Email Verification sent. Please check your email to activate your account.",
                    FormMethod = FormMethod.Get,
                    SubmitButtonDescription = "Login",
                };

                return View("RequiredAction", _raModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return View("Error");
            }
        }

        public void SendActivationEmail(int? uid)
        {
            if (uid == 0 || uid == null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    uid = _userService.GetUserBy(User.Identity.Name).UserId;
                }
            }

            var user = _userService.GetUserBy(uid);

            var activationCode = _userService.AssignNewValidationCode(user);
            var encryptedActivationCode = EncryptionHelper.Base64Encode(activationCode.ToString("N"));

            _userService.SaveChanges(uid.Value);

            using (var mm = new MailMessage())
            {
                try
                {
                    string body = "Hi " + user.Username + ",";
                    body += "<br /><br />Please click the following link to activate your account";
                    body += "<br /><a href = '" + string.Format("{0}://{1}/Account/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, encryptedActivationCode) + "'>Click here to activate your account.</a>";
                    body += "<br /><br />*This is an automatic generated mail, please do not reply.";

                    var mailaddress = AppSettingsHelper.GetKeyValue("MailingAddress");
                    var mailaddresspassword = AppSettingsHelper.GetKeyValue("MailingAddressPassword");

                    SmtpClient smtp = new SmtpClient();
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                    //        System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                    //        System.Security.Cryptography.X509Certificates.X509Chain chain,
                    //        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    //{
                    //    return true;
                    //};

                    mm.From = new MailAddress(mailaddress);
                    mm.To.Add(user.Email);
                    mm.Subject = "Account Activation";
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    mm.BodyEncoding = System.Text.Encoding.UTF8;
                    mm.SubjectEncoding = System.Text.Encoding.Default;

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