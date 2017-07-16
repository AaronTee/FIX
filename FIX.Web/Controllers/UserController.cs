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

namespace FIX.Web.Controllers
{
    [Authorize(Roles = DBCRole.Admin)]
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

            queryableList = queryableList.PaginateList("Username", sort, order, offset, limit);

            var rowsResult = queryableList.ToList()
                .Select(x => new UserListViewModel() {
                    UserId = x.UserId,
                    Username = x.Username,
                    Email = x.Email,
                    RoleName = x.UserProfile.Role.Description,
                    Status = x.Status.Description,
                    ActionLinks = new List<ActionLink>()
                    {
                        new ActionLink() {
                            Name = "Edit",
                            Url = Url.RouteUrl(new {
                                id = x.UserId,
                                controller = "User",
                                action = "Edit"
                            })
                        },
                        new ActionLink() {
                            Name = "Detail",
                            Url = Url.RouteUrl(new {
                                id = x.UserId,
                                controller = "User",
                                action = "Detail"
                            })
                        }
                    }

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

            model.ReferralDDL = new SelectList(_userService.GetAllUsers().ToList().Where(x => x.UserProfile.Role.Description != DBCRole.Admin)
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
            User user = new User()
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                IP = Request.UserHostAddress,
                HasAcceptedTerms = false,
                HasEmailVerified = false,
                IsFirstTimeLogIn = false,
                CreatedTimestamp = DateTime.UtcNow,
                TimeZoneId = DBConstant.DEFAULT_TIMEZONEID,
                StatusId = (int)DBConstant.EStatus.Active,
                UserProfile = new UserProfile
                {
                    Address = model.Address,
                    Country = model.Country,
                    FirstName = model.FirstName,
                    Gender = model.Gender,
                    LastName = model.LastName,
                    PhoneNo = model.PhoneNo,
                    RoleId = model.RoleId,
                    ReferralId = model.ReferralId,
                    CreatedTimestamp = DateTime.UtcNow,
                },
                UserBankAccount = new List<UserBankAccount>
                {
                    new UserBankAccount() {
                        BankAccountHolder = model.BankAccountHolder,
                        BankAccountNo = model.BankAccountNo,
                        BankBranch = model.BankBranch,
                        CreatedTimestamp = DateTime.UtcNow,
                        IsPrimary = true,
                        BankId = model.BankId
                    }
                }
            };
            _userService.InsertUser(user);

            _userService.SaveChanges(User.Identity.GetUserId<int>());

            var curUser = _userService.GetUserBy(user.Username);

            AccountController ac = new AccountController(_userService);
            ac.SendActivationEmail(curUser.UserId);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            UserViewModel model = new UserViewModel();

            var userInfo = _userService.GetUserBy(id);

            model.Username = userInfo.Username;
            model.Email = userInfo.Email;
            model.RoleId = userInfo.UserProfile.RoleId;

            model.FirstName = userInfo.UserProfile.FirstName;
            model.LastName = userInfo.UserProfile.LastName;
            model.Gender = userInfo.UserProfile.Gender;
            model.Address = userInfo.UserProfile.Address;
            model.Country = userInfo.UserProfile.Country;
            model.PhoneNo = userInfo.UserProfile.PhoneNo;
            model.ReferralName = (userInfo.UserProfile.ReferralId == (int?)null) ? "" : _userService.GetUserBy(userInfo.UserProfile.ReferralId).Username;

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
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {

            var userInfo = _userService.GetUserBy(model.Id);
            userInfo.UserProfile.RoleId = model.RoleId;
            userInfo.UserProfile.FirstName = model.FirstName;
            userInfo.UserProfile.LastName = model.LastName;
            userInfo.UserProfile.Gender = model.Gender;
            userInfo.UserProfile.Address = model.Address;
            userInfo.UserProfile.Country = model.Country;
            userInfo.UserProfile.PhoneNo = model.PhoneNo;
            userInfo.UserProfile.ModifiedTimestamp = DateTime.UtcNow;

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

            _userService.UpdateUser(userInfo);

            _userService.SaveChanges(User.Identity.GetUserId<int>());

            return RedirectToAction("Index");
        }

        public ActionResult Detail(int? id)
        {
            UserViewModel model = new UserViewModel();

            var userInfo = _userService.GetUserBy(id);

            model.Username = userInfo.Username;
            model.Email = userInfo.Email;
            model.RoleName = userInfo.UserProfile.Role.Description;

            model.FirstName = userInfo.UserProfile.FirstName;
            model.LastName = userInfo.UserProfile.LastName;
            model.Gender = userInfo.UserProfile.Gender;
            model.Address = userInfo.UserProfile.Address;
            model.Country = userInfo.UserProfile.Country;
            model.PhoneNo = userInfo.UserProfile.PhoneNo;
            model.ReferralName = (userInfo.UserProfile.ReferralId == (int?)null) ? "" : _userService.GetUserBy(userInfo.UserProfile.ReferralId).Username;

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

        [HttpPost]
        public JsonResult ValidateUsername(string username)
        {
            return Json(_userService.GetUserBy(username) == null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateEmail(string email)
        {
            return Json(_userService.IsValidEmailAddress(email), JsonRequestBehavior.AllowGet);
        }

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