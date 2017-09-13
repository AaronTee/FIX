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
using FIX.Web.Utils;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private IUserService _userService;
        private IBankService _bankService;
        private IInvestmentService _investmentService;

        public AccountController(IUserService userService, IBankService bankService = null, IInvestmentService investmentService = null)
        {
            _userService = userService;
            _bankService = bankService;
            _investmentService = investmentService;
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

        public void CreateNewAccount(UserViewModel model, string ipaddress = "")
        {
            try
            {
                User user;

                if (model.RoleId == (int)DBCRole.Id.Admin)
                {
                    user = new User()
                    {
                        Username = model.Username,
                        Email = model.Email ?? "-",
                        Password = model.Password,
                        SecurityPassword = "",
                        IP = ipaddress,
                        HasAcceptedTerms = true,
                        HasEmailVerified = true,
                        IsFirstTimeLogIn = false,
                        CreatedTimestamp = DateTime.UtcNow,
                        TimeZoneId = DBConstant.DEFAULT_TIMEZONEID,
                        StatusId = (int)DBConstant.EStatus.Active,
                        UserProfile = new UserProfile
                        {
                            ReferralId = 0,
                            Name = model.Username,
                            ICNumber = "",
                            Address = "",
                            City = "",
                            State = "",
                            PostCode = "",
                            Country = "",
                            Gender = DBCGender.Other,
                            PhoneNo = "",
                            RoleId = (int)DBCRole.Id.Admin,
                            CreatedTimestamp = DateTime.UtcNow,
                        }
                    };
                }
                else
                {
                    user = new User()
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password,
                        SecurityPassword = model.SecurityPassword,
                        IP = ipaddress,
                        HasAcceptedTerms = true,
                        HasEmailVerified = false,
                        IsFirstTimeLogIn = true,
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
                                UBAId = Guid.NewGuid(),
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
                }

                _userService.InsertUser(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(UserViewModel model)
        {
            try
            {
                CreateNewAccount(model, Request.UserHostAddress);

                var success = _userService.SaveChanges(User.Identity.GetUserId<int>());
                if (!success) return View("Error");

                var curUser = _userService.GetUserBy(model.Username);
                return VerifyEmail(curUser.UserId);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return RedirectToAction("Error");
        }

        [AllowAnonymous]
        public ActionResult VerifyEmail()
        {
            try
            {
                int uid = 0;
                Int32.TryParse(TempData["ActivationEmailUserId"]?.ToString(), out uid);
                if (uid == 0)
                {
                    return HttpNotFound();
                }

                RequiredActionModel _raModel = new RequiredActionModel
                {
                    Title = "Email pending verification",
                    Controller = "Account",
                    Action = "VerifyEmail",
                    Content = "Your email has not been verified yet, please check your registered email to verify.",
                    FormMethod = FormMethod.Post,
                    SubmitButtonDescription = "Resend email verification",
                    Data = uid,
                    DataName = "uid"
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
        [HttpPost]
        public ActionResult VerifyEmail(int uid)
        {
            var reqUser = _userService.GetUserBy(uid);

            //Check once more if account is verified
            //Probably someone abused here. [Check Target: User without verified yet]
            if (reqUser.HasEmailVerified)
            {
                RequiredActionModel _raModel2 = new RequiredActionModel
                {
                    Title = "This user's email has already verified.",
                    Controller = "Account",
                    Action = "Login",
                    Content = "",
                    FormMethod = FormMethod.Get,
                    SubmitButtonDescription = "Back",
                };
                return View("RequiredAction", _raModel2);
            }

            SendActivationEmail(uid, Request);

            /* For registration through logged in user portal */
            if (User.Identity.IsAuthenticated)
            {
                Success("Successfully registered user " + reqUser.Username + ". Verification email has been sent to account email address (" + reqUser.Email + ").", false);
                return RedirectToAction("Index", "Home");
            }

            RequiredActionModel _raModel = new RequiredActionModel
            {
                Title = "Email verification sent",
                Controller = "Account",
                Action = "Login",
                Content = "Email verification sent, Please check your email '" + reqUser.Email + "' to verify your email address.",
                FormMethod = FormMethod.Get,
                SubmitButtonDescription = "Login",
            };

            return View("RequiredAction", _raModel);
        }

        [AllowAnonymous]
        public ActionResult Activation(string token)
        {
            try
            {
                if (!token.IsNullOrEmpty())
                {
                    //check if activation is exist
                    if (_userService.IsValidToken(token, EAccessTokenPurpose.VerifyEmail) != null)
                    {
                        _userService.ActivateUserAccount(token);

                        RequiredActionModel _raModel = new RequiredActionModel
                        {
                            Controller = "Account",
                            Action = "Login",
                            FormMethod = FormMethod.Get,
                            SubmitButtonDescription = "Login to your account",
                        };

                        _raModel.Title = "Email successfully verified.";

                        return View("RequiredAction", _raModel);
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
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return new HttpStatusCodeResult(500);
            }
            return new HttpNotFoundResult();
        }

        public ActionResult Setup()
        {
            //Check if user submitted or not.
            var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

            if (!curUser.IsFirstTimeLogIn.Value) return new HttpNotFoundResult();
            SetupViewModel model = new SetupViewModel
            {
                PackageList = _investmentService.GetAllPackage().ToList().Select(x => new SetupViewModel.PackageInfo
                {
                    PackageDescription = x.Description,
                    PackageThreshold = x.Threshold.toCurrencyFormat(),
                    ReturnRate = (x.Rate * 100).ToString("#0.00") + "%",
                    styleClass = x.Description
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Setup(SetupViewModel model, HttpPostedFileBase ReceiptFile)
        {
            var currentUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

            if (ModelState.IsValid)
            {
                InvestmentCreateModel investmentCreateModel = new InvestmentCreateModel
                {
                    Bank = model.Bank,
                    Date = model.Date,
                    Amount = model.Amount,
                    ReceiptFile = ReceiptFile,
                    ReferenceNo = model.ReferenceNo
                };

                var isSuccess = new InvestmentController(_investmentService, _userService).NewPackageSubscription(investmentCreateModel, ReceiptFile, currentUser.UserId);
                if (isSuccess)
                {
                    currentUser.IsFirstTimeLogIn = false;
                    _userService.UpdateUser(currentUser);
                    _userService.SaveChanges(User.Identity.GetUserId<int>());

                    return RedirectToAction("SetupComplete");
                }
                else return View("Error");
            }

            model.PackageList = _investmentService.GetAllPackage().ToList().Select(x => new SetupViewModel.PackageInfo
            {
                PackageDescription = x.Description,
                PackageThreshold = x.Threshold.toCurrencyFormat(),
                ReturnRate = (x.Rate * 100).ToString("#0.00") + "%",
                styleClass = x.Description
            }).ToList();
            
            return View(model);
        }

        public ActionResult SetupComplete()
        {
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userStatusId = _userService.GetUserBy(User.Identity.GetUserId<int>()).StatusId;
                    if (User.Identity.GetUserHasAcceptedTC() && userStatusId == (int)EStatus.Active)
                    {
                        if (User.IsInRole(DBCRole.Admin))
                        {
                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    return LogOff();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return LogOff();
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
                if (await _userService.IsValid(model.Username, model.Password))
                {
                    var currentUser = _userService.GetUserBy(model.Username);
                    var userRole = _userService.GetUserRoleBy(currentUser.UserProfile);
                    
                    //Verify email
                    if (!currentUser.HasEmailVerified)
                    {
                        TempData["ActivationEmailUserId"] = currentUser.UserId;
                        return RedirectToAction("VerifyEmail");
                    }

                    //Add Claims
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

                    var isFirstTimeLogin = (currentUser.IsFirstTimeLogIn != null) ? currentUser.IsFirstTimeLogIn.Value : false;

                    //Verify is new user
                    if (isFirstTimeLogin)
                    {
                        return RedirectToAction("Setup");
                    }

                    //Verify if has pending application
                    if(currentUser.StatusId == (int)EStatus.Pending)
                    {
                        RequiredActionModel _raModel = new RequiredActionModel
                        {
                            Title = "Account pending review",
                            Controller = "Account",
                            Action = "Login",
                            Content = "Seems like your application has been submitted or existed in our system and is pending to review. Please wait while we confirm your application.",
                            FormMethod = FormMethod.Get,
                            SubmitButtonDescription = "Back To Login",
                        };

                        return View("RequiredAction", _raModel);
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

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                //Get user by email
                if (ModelState.IsValid)
                {
                    var reqUser = _userService.GetUserByEmail(model.Email);

                    if (reqUser != null)
                    {
                        var token = "rs?token=" + _userService.CreateNewToken(reqUser, EAccessTokenPurpose.ResetPassword);
                        string body = "Hi " + reqUser.Username + ",";
                        body += "<br /><br />Please click the following link to reset your password, do not proceed if this action is not performed by you.";
                        body += "<br /><a href = '" + string.Format("{0}://{1}/Account/ResetPassword/{2}", Request.Url.Scheme, Request.Url.Authority, token) + "'>Click here to activate your account.</a>";
                        body += "<br /><br />*This is an automatic generated mail, please do not reply.";

                        var mailaddress = AppSettingsHelper.GetKeyValue("MailingAddress");
                        Emailer.Send(body, "FIX Account Activation", mailaddress, reqUser.Email);
                    }

                    return RedirectToAction("ForgotPasswordConfirmation");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string token)
        {
            if (!token.IsNullOrEmpty())
            {
                if (_userService.IsValidToken(token, EAccessTokenPurpose.ResetPassword) != null)
                {
                    ResetPasswordViewModel model = new ResetPasswordViewModel
                    {
                        Token = RouteData.Values["id"]?.ToString(),
                    };
                    return View(model);
                }
            }

            return new HttpNotFoundResult();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            //add password reset model here. add password field for new password.
            try
            {
                if (ModelState.IsValid) {

                    if (!model.Token.IsNullOrEmpty())
                    {
                        //check if activation is exist
                        if (_userService.IsValidToken(model.Token, EAccessTokenPurpose.ResetPassword) != null)
                        {
                            _userService.ResetPassword(model.Token, model.Password);

                            RequiredActionModel _raModel = new RequiredActionModel
                            {
                                Controller = "Account",
                                Action = "Login",
                                FormMethod = FormMethod.Get,
                                SubmitButtonDescription = "Login to your account",
                            };

                            _raModel.Title = "Your password has been reset.";

                            return RedirectToAction("ResetPasswordConfirmation");
                        }
                    }
                }
                else
                {
                    return View("ResetPassword", new
                    {
                        id = model.Token
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return new HttpStatusCodeResult(500);
            }

            return new HttpNotFoundResult();
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //public ActionResult Manage()
        //{
        //    var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

        //    //_userService.ResetPassword(curUser.UserId,)

        //}

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


        public void SendActivationEmail(int uid, HttpRequestBase request)
        {
            if (uid == 0)
            {
                return;
            }

            var user = _userService.GetUserBy(uid);
            var token = "rs?token=" + _userService.CreateNewToken(user, EAccessTokenPurpose.VerifyEmail);
            string body = "Hi " + user.Username + ",";
            body += "<br /><br />Please click the following link to register your account, please note that this link is only valid 24 hours from the request time.";
            body += "<br /><a href = '" + string.Format("{0}://{1}/Account/Activation/{2}", request.Url.Scheme, request.Url.Authority, token) + "'>Click here to activate your account.</a>";
            body += "<br /><br />*This is an automatic generated mail, please do not reply.";

            var mailaddress = AppSettingsHelper.GetKeyValue("MailingAddress");

            Emailer.Send(body, "FIX Account Activation", mailaddress, user.Email);
        }

    }
}