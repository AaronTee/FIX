using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FIX.Web.Models;
using FIX.Service.Entities;
using FIX.Service;

namespace FIX.Web.Controllers
{
    [Authorize]
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
 
        [HttpGet]
        public ActionResult Create()
        {
            UserViewModel model = new UserViewModel();

            model.RoleDDL = _userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.RoleId.ToString()
            });

            model.BankDDL = _bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            });

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
                CreatedTimestamp = DateTime.UtcNow,
                UserProfile = new UserProfile
                {
                    Address = model.Address,
                    Country = model.Country,
                    FirstName = model.FirstName,
                    Gender = model.Gender,
                    LastName = model.LastName,
                    PhoneNo = model.PhoneNo,
                    RoleId = model.RoleId,
                    CreatedTimestamp = DateTime.UtcNow,
                },
                UserBankAccount = new List<UserBankAccount>
                {
                    new UserBankAccount() {
                        BankAcconutHolder = model.BankAccountHolder,
                        BankAccountNo = model.BankAccountNo,
                        BankBranch = model.BankBranch,
                        CreatedTimestamp = DateTime.UtcNow,
                        IsPrimary = true,
                        BankId = model.BankId
                    }
                }
            };
            _userService.InsertUser(user);

            _userService.SaveChanges();
            
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

            if (userInfo.UserBankAccount.Count > 0)
            {
                var userBank = userInfo.UserBankAccount.First();
                model.BankId = userBank.BankId;
                model.BankAccountHolder = userBank.BankAcconutHolder;
                model.BankAccountNo = userBank.BankAccountNo;
                model.BankBranch = userBank.BankBranch;
            }

            model.RoleDDL = _userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.RoleId.ToString()
            });

            model.BankDDL = _bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {

            var userInfo = _userService.GetUserBy(model.Id);

            userInfo.Username = model.Username;
            userInfo.Email = model.Email;

            userInfo.UserProfile.FirstName = model.FirstName;
            userInfo.UserProfile.LastName = model.LastName;
            userInfo.UserProfile.Gender = model.Gender;
            userInfo.UserProfile.Address = model.Address;
            userInfo.UserProfile.Country = model.Country;
            userInfo.UserProfile.PhoneNo = model.PhoneNo;

            if (userInfo.UserBankAccount.Count > 0)
            {
                foreach (var bankAccount in userInfo.UserBankAccount)
                {
                    bankAccount.BankId = model.BankId;
                    bankAccount.BankAcconutHolder = model.BankAccountHolder;
                    bankAccount.BankAccountNo = model.BankAccountNo;
                    bankAccount.BankBranch = model.BankBranch;
                    break; //support only one account for now hence break.
                }
            }

            _userService.UpdateUser(userInfo);

            _userService.SaveChanges();

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

            if (userInfo.UserBankAccount.Count > 0)
            {
                var userBank = userInfo.UserBankAccount.First();
                model.BankName = userBank.Bank.Name;
                model.BankAccountHolder = userBank.BankAcconutHolder;
                model.BankAccountNo = userBank.BankAccountNo;
                model.BankBranch = userBank.BankBranch;
            }

            return View(model);
        }

        public JsonResult ValidateUsername(string input)
        {
            return Json(_userService.GetUserBy(input) == null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateEmail(string input)
        {
            return Json(_userService.IsValidEmailAddress(input), JsonRequestBehavior.AllowGet);
        }

    }
}