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
using System.Web.Security;
using System.Configuration;
using static FIX.Service.DBConstant;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IUserService _userService;
        private IBankService _bankService;

        public AccountController(IUserService userService, IBankService bankService)
        {
            _userService = userService;
            _bankService = bankService;
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            UserViewModel model = new UserViewModel();

            model.BankDDL = new SelectList(_bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            }), "Value", "Text");

            model.ReferralDDL = new SelectList(_userService.GetAllUsersWithoutAdmin().ToList()
                .Select(x => new SelectListItem()
                {
                    Text = x.UserId + " - " + x.Username,
                    Value = x.UserId.ToString()
                }), "Value", "Text");

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(UserViewModel model)
        {
            try
            {
                User user = new User()
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    SecurityPassword = model.SecurityPassword,
                    IP = Request.UserHostAddress,
                    HasAcceptedTerms = false,
                    HasEmailVerified = false,
                    IsFirstTimeLogIn = false,
                    CreatedTimestamp = DateTime.UtcNow,
                    TimeZoneId = DBConstant.DEFAULT_TIMEZONEID,
                    StatusId = (int)DBConstant.EStatus.Pending,
                    UserProfile = new UserProfile
                    {
                        ReferralId = model.ReferralId,
                        Name = model.Name,
                        ICNumber = model.ICNumber,
                        Address = model.Address,
                        City = model.City,
                        State = model.State,
                        PostCode = model.PostCode,
                        Country = model.Country,
                        Gender = model.Gender,
                        PhoneNo = model.PhoneNo,
                        RoleId = _userService.GetAllRoles().Where(x => x.RoleId == (int)DBCRole.Id.User).FirstOrDefault().RoleId,
                        CreatedTimestamp = DateTime.UtcNow,
                    },
                    UserBankAccount = new List<UserBankAccount>
                    {
                        new UserBankAccount()
                        {
                            BankAccountHolder = model.BankAccountHolder,
                            BankAccountNo = model.BankAccountNo,
                            BankBranch = model.BankBranch,
                            CreatedTimestamp = DateTime.UtcNow,
                            IsPrimary = true,
                            BankId = model.BankId
                        }
                    },
                    UserWallet = new List<UserWallet>
                    {
                        new UserWallet()
                        {
                            WalletId = Guid.NewGuid(),
                            Currency = DBCCurrency.USD,
                            Balance = decimal.Zero,
                        }
                    }
                };

                _userService.InsertUser(user);
                var success = _userService.SaveChanges(User.Identity.GetUserId<int>());

                if (!success) return View("Error");

                var curUser = _userService.GetUserBy(user.Username);

                TempData["ActivationEmailUserId"] = curUser.UserId;
                return RedirectToAction("ActivationEmail");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return RedirectToAction("Error");
        }

        [AllowAnonymous]
        public ActionResult ActivationEmail()
        {
            try
            {
                var uid = (int)TempData["ActivationEmailUserId"];

                if (uid == 0)
                {
                    return HttpNotFound();
                }

                SendActivationEmail(uid);

                RequiredActionModel _raModel = new RequiredActionModel
                {
                    Title = "Your account need to be verified by your email.",
                    Controller = "Account",
                    Action = "Login",
                    Content = "Please check your email to activate your account.",
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

        [AllowAnonymous]
        public ActionResult Activation()
        {
            try
            {
                var encrypedActivationCode = RouteData.Values["id"]?.ToString();
                if (!encrypedActivationCode.IsNullOrEmpty())
                {
                    if (new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$").IsMatch(encrypedActivationCode))
                    {
                        var decryptedActivationCode = EncryptionHelper.Base64Decode(encrypedActivationCode);
                        Guid activationCode = Guid.ParseExact(decryptedActivationCode, "N");

                        //check if activation is exist
                        if (_userService.IsValidActivationCode(activationCode))
                        {
                            _userService.ValidateActivationCode(activationCode);
                            return RedirectToAction("Setup");
                        }
                        else
                        {
                            //RequiredActionModel _raModel = new RequiredActionModel
                            //{
                            //    Controller = "Account",
                            //    Action = "Login",
                            //    FormMethod = FormMethod.Get,
                            //    SubmitButtonDescription = "Login",
                            //};

                            //_raModel.Title = "Invalid Verification Request";
                            //_raModel.Content = "Verification link or code is not valid, either has been removed or expired. Please log in to your account to request another email verification code.";

                            //return View("RequiredAction", _raModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return new HttpStatusCodeResult(500);
            }
            return new HttpNotFoundResult();
        }

        public ActionResult Setup()
        {
            if (!User.Identity.GetUserIsFirstTimeLogin()) return new HttpNotFoundResult();
            SetupViewModel model = new SetupViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Setup(SetupViewModel model)
        {
            return View("Setup");
        }

        public ActionResult SetupComplete()
        {
            Response.AddHeader("REFRESH", "5;URL=/Account/Login");

            return View();
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, bool rememberMe)
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
                if (await _userService.IsValid(model.Username, model.Password, ConfigurationManager.AppSettings["CipherKeyPhrase"]))
                {
                    var currentUser = _userService.GetUserBy(model.Username);
                    var userRole = _userService.GetUserRoleBy(currentUser.UserProfile);
                    
                    //Verify email
                    if (!currentUser.HasEmailVerified)
                    {
                        TempData["ActivationEmailUserId"] = currentUser.UserId;
                        return RedirectToAction("ActivationEmail");
                    }

                    var ident = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, currentUser.UserId.ToString()),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim("TimeZone", currentUser.TimeZoneId),
                    new Claim("AcceptedTC", currentUser.HasAcceptedTerms.ToString()),
                    new Claim("IsFirstTimeLogin", currentUser.IsFirstTimeLogIn.ToString()),
                    new Claim(ClaimTypes.Role, userRole.Description)
                    },
                    DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    }, ident);

                    //if (!returnUrl.IsNullOrEmpty())
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //else
                    //{
                    //    return (userRole.Description == DBConstant.DBCRole.Admin) ? RedirectToAction("Index", "User") : RedirectToAction("Index", "Home");
                    //}

                    if (currentUser.IsFirstTimeLogIn.Value)
                    {
                        return RedirectToAction("Setup");
                    }

                    return (userRole.Description == DBConstant.DBCRole.Admin) ? RedirectToAction("Index", "User") : RedirectToAction("Index", "Home");

                }

                // invalid username or password
                ModelState.AddModelError("", "Invalid username or password");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return View();
        }

        //[AllowAnonymous]
        //public ActionResult EmailVerification()
        //{
        //    var requestUserId = (int)TempData["EmailActivationResendUserId"];

        //    if(requestUserId == 0)
        //    {
        //        return HttpNotFound();
        //    }

        //    RequiredActionModel _raModel = new RequiredActionModel
        //    {
        //        Title = "Pending Email Verification",
        //        Controller = "Account",
        //        Action = "ActivationEmail",
        //        Content = @"Your email has not verified yet, please check your email to activate this account before continue.",
        //        SubmitButtonDescription = "Resend Verification Email",
        //        FormMethod = FormMethod.Post,
        //        DataName = "uid",
        //        Data = requestUserId
        //    };

        //    return View("RequiredAction", _raModel);
        //}

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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
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
                    body += "<br /><br />Please click the following link to register your account";
                    body += "<br /><a href = '" + string.Format("{0}://{1}/Account/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, encryptedActivationCode) + "'>Click here to activate your account.</a>";
                    body += "<br /><br />*This is an automatic generated mail, please do not reply.";

                    var mailaddress = AppSettingsHelper.GetKeyValue("MailingAddress");
                    var mailaddresspassword = AppSettingsHelper.GetKeyValue("MailingAddressPassword");

                    SmtpClient smtp = new SmtpClient();

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