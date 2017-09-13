using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FIX.Web.Models;
using FIX.Service;
using Microsoft.AspNet.Identity;
using static FIX.Service.DBConstant;
using System.Threading.Tasks;
using FIX.Service.Entities;
using FIX.Web.Extensions;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class ManageAccountController : BaseController
    {

        private IUserService _userService;
        private IBankService _bankService;

        public ManageAccountController(IUserService userService, IBankService bankService)
        {
            _userService = userService;
            _bankService = bankService;
        }
        
        public ActionResult Index()
        {
            var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

            ManageAccountViewModel model = new ManageAccountViewModel
            {
                ManagePersonalDetailVM = new ManagePersonalDetailViewModels
                {
                    Name = curUser.UserProfile.Name,
                    Address = curUser.UserProfile.Address,
                    City = curUser.UserProfile.City,
                    Country = curUser.UserProfile.Country,
                    PhoneNo = curUser.UserProfile.PhoneNo,
                    PostCode = curUser.UserProfile.PostCode,
                    State = curUser.UserProfile.State,
                },
                ReferralName = _userService.GetReferralBy(curUser.UserId)?.Username,
                Username = curUser.Username,
                Status = curUser.Status.Description,
                Email = curUser.Email,
                Gender = curUser.UserProfile.Gender,
                CreditBalance = curUser.UserWallet.FirstOrDefault()?.Balance.toCurrencyFormat() ?? "-"
            };

            if (curUser.UserBankAccount.Count > 0) {
                model.ManageBankAccountVM = new ManageBankAccountViewModels
                {
                    BankId = curUser.UserBankAccount.FirstOrDefault()?.BankId ?? 0,
                    BankAccountNo = curUser.UserBankAccount.FirstOrDefault()?.BankAccountNo,
                    BankAccountHolder = curUser.UserBankAccount.FirstOrDefault()?.BankAccountHolder,
                    BankBranch = curUser.UserBankAccount.FirstOrDefault()?.BankBranch,
                };
            }
            else
            {
                model.ManageBankAccountVM = new ManageBankAccountViewModels
                {
                    BankId = 0
                };
            }

            model.CountryDDL = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "Malaysia",
                    Value = "MY"
                }
            }, "Value", "Text", model.ManagePersonalDetailVM.Country);

            model.BankDDL = new SelectList(_bankService.GetAllBank().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.BankId.ToString()
            }), "Value", "Text", model.ManageBankAccountVM.BankId);

            return View(model);
        }

        public async Task<ActionResult> UpdatePassword([Bind(Prefix = "ManagePasswordVM")]ManagePasswordViewModels model)
        {
            try
            {
                var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

                var validCredential = await _userService.IsValid(curUser.Username, model.CurrentPassword);

                if (!validCredential)
                {
                    return Json(new { result = "Current password is invalid.", alertClass = "warning" }, JsonRequestBehavior.AllowGet);
                }

                if (model.ConfirmNewPassword != model.NewPasword)
                {
                    return Json(new { result = "Confirm password does not match.", alertClass = "warning" }, JsonRequestBehavior.AllowGet);
                }

                _userService.UpdatePassword(curUser, model.NewPasword);
                var saveSuccess = _userService.SaveChanges(User.Identity.GetUserId<int>());

                if (saveSuccess)
                {
                    return Json(new { result = "Password has been updated.", alertClass = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return Json(new { result = "Failed to update password. Please contact our customer support if problem still persist.", alertClass = "danger" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUserProfile([Bind(Prefix = "ManagePersonalDetailVM")]ManagePersonalDetailViewModels model)
        {
            try
            {
                var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

                if (ModelState.IsValid)
                {
                    curUser.UserProfile.Address = model.Address;
                    curUser.UserProfile.City = model.City;
                    curUser.UserProfile.Country = model.Country;
                    curUser.UserProfile.Name = model.Name;
                    curUser.UserProfile.PhoneNo = model.PhoneNo;
                    curUser.UserProfile.PostCode = model.PostCode;
                    curUser.UserProfile.State = model.State;

                    _userService.UpdateUser(curUser);
                    var saveSuccess = _userService.SaveChanges(User.Identity.GetUserId<int>());

                    if (saveSuccess)
                    {
                        return Json(new { result = "Your Profile has been updated.", alertClass = "success" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { result = "Some fields are not fill in correctly. Please fill in and submit again.", alertClass = "warning" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return Json(new { result = "Failed to update password. Please contact our customer support if problem still persist.", alertClass = "danger" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateBankAccount([Bind(Prefix = "ManageBankAccountVM")]ManageBankAccountViewModels model)
        {
            try
            {
                //Check if bank combination already existed
                /* This project will not use this method because multiple account could possibly have same bank account (same owner) */
                //var isValidBankRecord = _userService.IsNotInUsedBankAccount(model.BankId, model.BankAccountNo);

                //if (!isValidBankRecord)
                //{
                //    return Json(new { result = "This bank account has already registered on another account.", alertClass = "warning" }, JsonRequestBehavior.AllowGet);
                //}

                var curUser = _userService.GetUserBy(User.Identity.GetUserId<int>());

                //No Bank Account
                if (curUser.UserBankAccount.Count < 1)
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

                    curUser.UserBankAccount.Add(uba);
                    _userService.UpdateUser(curUser);
                }
                else
                {
                    curUser.UserBankAccount.FirstOrDefault().BankId = model.BankId;
                    curUser.UserBankAccount.FirstOrDefault().BankAccountHolder = model.BankAccountHolder;
                    curUser.UserBankAccount.FirstOrDefault().BankAccountNo = model.BankAccountNo;
                    curUser.UserBankAccount.FirstOrDefault().BankBranch = model.BankBranch;

                    _userService.UpdateUser(curUser);
                }

                var saveSuccess = _userService.SaveChanges(User.Identity.GetUserId<int>());

                if (saveSuccess)
                {
                    return Json(new { result = "Your bank account has been updated.", alertClass = "success" }, JsonRequestBehavior.AllowGet);
                }

            }catch(Exception ex)
            {
                Log.Error(ex.Message, ex);
            }

            return Json(new { result = "Failed to update your bank account. Please contact our customer support if problem still persist.", alertClass = "danger" }, JsonRequestBehavior.AllowGet);
        }
    }
}