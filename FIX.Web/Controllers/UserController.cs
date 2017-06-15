using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FIX.Web.Models;
using FIX.Core.Data;
using FIX.Service.Interface;
using AutoMapper;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private IUserService _userService;

        public UserController(IUserService userService, IBaseService baseService) : base(baseService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<UserViewModel> users = _userService.GetAllUsers().Select(u => new UserViewModel
            {
                Username = u.Username,
                Email = u.Email,
                UserId = u.UserId,
                Roles = u.UserProfile.Roles
            }).ToList();
            return View(users);
        }
 
        [HttpGet]
        public ActionResult Create()
        {
            UserCreateEditViewModel model = new UserCreateEditViewModel();

            model.CountryDDL = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Malaysia", Value = "MY" }
            };

            model.RoleDDL = _userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.RoleName,
                Value = x.RoleId.ToString()
            });

            model.GenderDDL = _userService.GetAllGender().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.GenderId.ToString()
            });

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserCreateEditViewModel model)
        {
            List<Role> rolesAssigned = new List<Role>();

            foreach (var selectedRole in model.Roles)
            {
                rolesAssigned.Add(_userService.GetRoleById(selectedRole));
            }

            User userEntity = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                CreatedTimestamp = DateTime.UtcNow,
                ModifiedTimestamp = DateTime.UtcNow,
                IP = Request.UserHostAddress,
                UserProfile = new UserProfile
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Gender = _userService.GetGenderById(model.Gender),
                    PhoneNo = model.PhoneNo,
                    CreatedTimestamp = DateTime.UtcNow,
                    ModifiedTimestamp = DateTime.UtcNow,
                    UserBankAccount = new List<UserBankAccount>()
                    {
                        new UserBankAccount() {
                            BankAccountHolder = model.BankAccountHolder,
                            BankAccountNo = model.BankAccountNo,
                            BankBranch = model.BankBranch
                        }
                    },
                    Roles = rolesAssigned
                },
            };
            _userService.InsertUser(userEntity);

            Save();
            
            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            UserCreateEditViewModel model = new UserCreateEditViewModel();

            model.CountryDDL = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Malaysia", Value = "MY" }
            };

            model.RoleDDL = _userService.GetAllRoles().Select(x => new SelectListItem()
            {
                Text = x.RoleName,
                Value = x.RoleId.ToString()
            });

            model.GenderDDL = _userService.GetAllGender().Select(x => new SelectListItem()
            {
                Text = x.Description,
                Value = x.GenderId.ToString()
            });

            if (id.HasValue && id != 0)
            {
                User userEntity = _userService.GetUser(id.Value);

                model.Username = userEntity.Username;
                model.Email = userEntity.Email;
                model.Username = userEntity.Username;
                model.Password = userEntity.Password;

                model.FirstName = userEntity.UserProfile?.FirstName;
                model.LastName = userEntity.UserProfile?.LastName;
                model.Address = userEntity.UserProfile?.Address;
                model.Country = userEntity.UserProfile?.Country;
                model.PhoneNo = userEntity.UserProfile?.PhoneNo;
                model.RolesDescription = userEntity.UserProfile.Roles.Select(x => x.RoleName).ToList();
                model.GenderDescription = userEntity.UserProfile.Gender?.Description;

                if(userEntity.UserProfile.UserBankAccount.Count() > 0)
                {
                    var bankInfo = userEntity.UserProfile.UserBankAccount.First(); //We have one bank account for now.
                    model.BankAccountHolder = bankInfo.BankAccountHolder;
                    model.BankAccountNo = bankInfo.BankAccountNo;
                    model.BankBranch = bankInfo.BankBranch;
                }
                
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserCreateEditViewModel model)
        {
            List<Role> rolesAssigned = new List<Role>();

            foreach(var selectedRole in model.Roles)
            {
                rolesAssigned.Add(_userService.GetRoleById(selectedRole));
            }

            User userEntity = _userService.GetUser(model.Id);

            List<UserBankAccount> bankAccountAssigned = userEntity.UserProfile.UserBankAccount?.ToList();
            
            if (bankAccountAssigned.Count() < 1)
            {
                bankAccountAssigned.Add(new UserBankAccount()
                {
                    BankAccountHolder = model.BankAccountHolder,
                    BankAccountNo = model.BankAccountNo,
                    BankBranch = model.BankBranch,
                    CreatedTimestamp = DateTime.UtcNow
                });
            }
            else
            {
                bankAccountAssigned.First().BankAccountHolder = model.BankAccountHolder;
                bankAccountAssigned.First().BankAccountNo = model.BankAccountNo;
                bankAccountAssigned.First().BankBranch = model.BankBranch;
                bankAccountAssigned.First().ModifiedTimestamp = DateTime.UtcNow;
            }


            userEntity.Username = model.Username;
            userEntity.Email = model.Email;
            userEntity.Password = model.Password;
            userEntity.CreatedTimestamp = DateTime.UtcNow;
            userEntity.ModifiedTimestamp = DateTime.UtcNow;
            userEntity.IP = Request.UserHostAddress;
            userEntity.UserProfile.FirstName = model.FirstName;
            userEntity.UserProfile.LastName = model.LastName;
            userEntity.UserProfile.Address = model.Address;
            userEntity.UserProfile.Country = model.Country;
            userEntity.UserProfile.PhoneNo = model.PhoneNo;
            userEntity.UserProfile.Gender = _userService.GetGenderById(model.Gender);
            userEntity.UserProfile.ModifiedTimestamp = DateTime.UtcNow;
            userEntity.UserProfile.UserBankAccount = bankAccountAssigned;
            userEntity.UserProfile.Roles = rolesAssigned;

            _userService.UpdateUser(userEntity);

            Save();

            return RedirectToAction("Index");
        }

        public ActionResult Detail(int? id)
        {
            UserViewModel model = new UserViewModel();
            if (id.HasValue && id != 0)
            {
                User userEntity = _userService.GetUser(id.Value);

                model.FirstName = userEntity.UserProfile?.FirstName;
                model.LastName = userEntity.UserProfile?.LastName;
                model.Address = userEntity.UserProfile?.Address;
                model.Email = userEntity?.Email;
                model.CreatedTimestamp = userEntity.CreatedTimestamp;
                model.Username = userEntity.Username;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            UserViewModel model = new UserViewModel();
            if (id != 0)
            {
                User userEntity = _userService.GetUser(id);
                model.FirstName = userEntity.UserProfile.FirstName;
                model.LastName = userEntity.UserProfile.LastName;
                model.Address = userEntity.UserProfile.Address;
                model.Email = userEntity.Email;
                model.CreatedTimestamp = userEntity.CreatedTimestamp;
                model.Username = userEntity.Username;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                if (id != 0)
                {
                    User userEntity = _userService.GetUser(id);
                    _userService.DeleteUser(userEntity);
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}