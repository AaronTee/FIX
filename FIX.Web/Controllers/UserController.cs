using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FIX.Web.Models;
using FIX.Service.Entities;
using FIX.Service;
using FIX.Web.Extensions;
using static FIX.Service.DBConstant;
using Microsoft.AspNet.Identity;
using SyntrinoWeb.Attributes;

namespace FIX.Web.Controllers
{
    [Authorize(Roles = DBCRole.Admin)]
    [IdentityAuthorize]
    public class UserController : BaseController
    {

        private IUserService _userService;
        private IBankService _bankService;

        public UserController(IUserService userService, IBankService bankService)
        {
            _userService = userService;
            _bankService = bankService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<UserViewModel> users = _userService.GetAllUsers().Select(u => new UserViewModel
            {
                Id = u.UserId,
                Username = u.Username,
                Email = u.Email,
                RoleName = u.UserProfile.Role.Description
            }).ToList();
            return View(users);
        }

        public JsonResult GetListViewData(int offset, int limit, string search, string sort, string order)
        {
            var queryableList = _userService.GetAllUsers();
            var allRowCount = queryableList.Count();

            if (search.IsNotNullOrEmpty())
            {
                queryableList = queryableList.Where(x => x.Username.Contains(search));
            }

            //Limit

            if (sort == "RoleName") sort = "UserProfile.Role.Description";
            if (sort == "Status") sort = "Status.Description";
            //Sort
            queryableList = queryableList.PaginateList(x => x.Username, sort, order, offset, limit);

            var rowsResult = queryableList.ToList()
                .Select(x => new UserListViewModel() {
                    UserId = x.UserId,
                    Username = x.Username,
                    Email = x.Email,
                    RoleName = x.UserProfile.Role.Description,
                    Status = x.Status.Description,
                    ActionLinks = new Func<List<ActionLink>>(() =>
                    {
                        List<ActionLink> links = new List<ActionLink>();
                        if (x.UserProfile.RoleId != (int)DBCRole.Id.Admin)
                        {
                            links.Add(new ActionLink()
                            {
                                Name = "Edit",
                                Url = Url.RouteUrl(new
                                {
                                    id = x.UserId,
                                    controller = "User",
                                    action = "Edit"
                                })
                            });

                            links.Add(new ActionLink()
                            {
                                Name = "Detail",
                                Url = Url.RouteUrl(new
                                {
                                    id = x.UserId,
                                    controller = "User",
                                    action = "Detail"
                                })
                            });
                        }
                        return links;
                    })()

                });

            var model = new
            {
                total = allRowCount,
                rows = rowsResult
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
 
        [HttpGet]
        public ActionResult Create()
        {
            UserViewModel model = new UserViewModel();

            model.RoleDDL = new SelectList(_userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.RoleId.ToString()
            }), "Value", "Text");

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

        [HttpPost]
        public ActionResult Create(UserViewModel model)
        {
            try
            {
                AccountController accController = new AccountController(_userService, _bankService);
                accController.CreateNewAccount(model, Request.UserHostAddress);
                _userService.SaveChanges(User.Identity.GetUserId<int>());

                //Query the new userid
                var newUser = _userService.GetUserBy(model.Username);
                accController.SendActivationEmail(newUser.UserId, Request);

                if (model.RoleId == (int)DBCRole.Id.Admin)
                {
                    Success("Successfully created admin " + model.Username + ".");
                }
                else
                {
                    Success("Successfully registered user " + model.Username + ". Verification email has been sent to account email address (" + model.Email + ").", false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                Danger("Failed to create user " + model.Username + ", you can check logs files regarding the error detail.");
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var userInfo = _userService.GetUserBy(id);

            UserViewModel model = new UserViewModel
            {
                Username = userInfo.Username,
                Email = userInfo.Email,
                RoleId = userInfo.UserProfile.RoleId,
                Name = userInfo.UserProfile.Name,
                ICNumber = userInfo.UserProfile.ICNumber,
                Address = userInfo.UserProfile.Address,
                Gender = userInfo.UserProfile.Gender,
                State = userInfo.UserProfile.State,
                City = userInfo.UserProfile.City,
                PostCode = userInfo.UserProfile.PostCode,
                Country = userInfo.UserProfile.Country,
                PhoneNo = userInfo.UserProfile.PhoneNo,
                ReferralName = _userService.GetUserBy(userInfo.UserProfile.ReferralId)?.Username,
                CreditBalance = userInfo.UserWallet.FirstOrDefault()?.Balance.toCurrencyFormat()
            };

            if (userInfo.UserBankAccount.Count > 0)
            {
                var userBank = userInfo.UserBankAccount.First();
                model.BankId = userBank.BankId;
                model.BankAccountHolder = userBank.BankAccountHolder;
                model.BankAccountNo = userBank.BankAccountNo;
                model.BankBranch = userBank.BankBranch;
            }

            model.RoleDDL = new SelectList(_userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.RoleId.ToString()
            }), "Value", "Text", model.RoleId);

            model.BankDDL = new SelectList(_bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            }), "Value", "Text", model.BankId);

            model.GenderDDL = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Male", Value = DBConstant.DBCGender.Male },
                new SelectListItem() { Text = "Female", Value = DBConstant.DBCGender.Female },
                new SelectListItem() { Text = "Other", Value = DBConstant.DBCGender.Other }
            }, "Value", "Text", model.Gender);

            model.CountryDDL = new SelectList(
                new List<SelectListItem>()
                {
                    new SelectListItem
                    {
                        Text = "Malaysia",
                        Value = "MY"
                    }
                }, "Value", "Text", model.Country);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            var userInfo = _userService.GetUserBy(model.Id);
            try
            {
                userInfo.UserProfile.RoleId = model.RoleId;
                userInfo.UserProfile.City = model.City;
                userInfo.UserProfile.State = model.State;
                userInfo.UserProfile.PostCode = model.PostCode;
                userInfo.UserProfile.Gender = model.Gender;
                userInfo.UserProfile.Address = model.Address;
                userInfo.UserProfile.Country = model.Country;
                userInfo.UserProfile.PhoneNo = model.PhoneNo;
                userInfo.UserProfile.ModifiedTimestamp = DateTime.UtcNow;

                //has at least one bank account
                if (userInfo.UserBankAccount.Count > 0)
                {
                    foreach (var bankAccount in userInfo.UserBankAccount)
                    {
                        bankAccount.BankId = model.BankId;
                        bankAccount.BankAccountHolder = model.BankAccountHolder;
                        bankAccount.BankAccountNo = model.BankAccountNo;
                        bankAccount.BankBranch = model.BankBranch;
                        break; //support only one account for now hence break.
                    }
                }
                else
                {
                    UserBankAccount uba = new UserBankAccount
                    {
                        UBAId = Guid.NewGuid(),
                        IsPrimary = true,
                        CreatedTimestamp = DateTime.UtcNow,
                        BankId = model.BankId,
                        BankAccountHolder = model.BankAccountHolder,
                        BankAccountNo = model.BankAccountNo,
                        BankBranch = model.BankBranch,
                    };

                    userInfo.UserBankAccount.Add(uba);
                }

                //create wallet if empty
                if(userInfo.UserWallet.Count < 1)
                {
                    userInfo.UserWallet = new List<UserWallet>
                    {
                        new UserWallet()
                        {
                            WalletId = Guid.NewGuid(),
                            Balance = decimal.Zero,
                            Currency = DBCCurrency.USD,
                            UserId = userInfo.UserId
                        }
                    };
                }

                _userService.UpdateUser(userInfo);

                _userService.SaveChanges(User.Identity.GetUserId<int>());

                Success("Successfully modified user " + userInfo.Username + ".");

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                Danger("Failed to modify user " + userInfo.Username + ", you can check audit log regarding the error detail.");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Detail(int? id)
        {
            var userInfo = _userService.GetUserBy(id);
            UserViewModel model = new UserViewModel
            {
                Username = userInfo.Username,
                Email = userInfo.Email,
                hasEmailVerified = userInfo.HasEmailVerified,
                hasAcceptedTerms = userInfo.HasAcceptedTerms,
                RoleName = userInfo.UserProfile.Role.Description,
                Name = userInfo.UserProfile.Name,
                ICNumber = userInfo.UserProfile.ICNumber,
                Gender = userInfo.UserProfile.Gender,
                PhoneNo = userInfo.UserProfile.PhoneNo,
                Address = userInfo.UserProfile.Address,
                City = userInfo.UserProfile.City,
                State = userInfo.UserProfile.State,
                PostCode = userInfo.UserProfile.PostCode,
                Country = userInfo.UserProfile.Country,
                CreditBalance = userInfo.UserWallet.FirstOrDefault()?.Balance.toCurrencyFormat(),
                ReferralName = _userService.GetUserBy(userInfo.UserProfile.ReferralId)?.Username
            };

            if (userInfo.UserBankAccount.Count > 0)
            {
                var userBank = userInfo.UserBankAccount.First();
                model.BankName = userBank.Bank.Name;
                model.BankAccountHolder = userBank.BankAccountHolder;
                model.BankAccountNo = userBank.BankAccountNo;
                model.BankBranch = userBank.BankBranch;
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ValidateUsername(string username)
        {
            return Json(_userService.GetUserBy(username) == null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ValidateEmail(string email)
        {
            return Json(_userService.IsValidEmailAddress(email), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult Search(string input)
        {
            object data = new { };

            if (!input.IsNullOrEmpty())
            {
                data = _userService.GetUsersWithoutAdmin().Where(x => x.Username.Contains(input)).Select(x => new
                {
                    text = x.UserId.ToString() + " - " + x.Username,
                    id = x.UserId.ToString()
                });
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}