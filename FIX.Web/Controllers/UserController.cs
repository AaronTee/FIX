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
                Roles = u.Roles
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
                    CreatedTimestamp = DateTime.UtcNow,
                    ModifiedTimestamp = DateTime.UtcNow
                },
                Roles = rolesAssigned
            };
            _userService.InsertUser(userEntity);
            
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
                UserBankAccount userBankAccountEntity = _userService.GetPrimaryBankAccount(userEntity);

                model.Username = userEntity.Username;
                model.Email = userEntity.Email;
                model.Username = userEntity.Username;
                model.Password = userEntity.Password;

                model.FirstName = userEntity.UserProfile?.FirstName;
                model.LastName = userEntity.UserProfile?.LastName;
                model.Address = userEntity.UserProfile?.Address;

                model.BankAccountHolder = userBankAccountEntity?.BankAccountHolder;
                model.BankAccountNo = userBankAccountEntity?.BankAccountNo;
                model.BankBranch = userBankAccountEntity?.BankBranch;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserCreateEditViewModel model)
        {
            List<Role> rolesAssigned = new List<Role>();

            foreach(var selectedRole in model.Roles)
            {
                rolesAssigned.Add(new Role() {
                    RoleId = selectedRole
                });
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
                    Gender = model.Gender,
                    CreatedTimestamp = DateTime.UtcNow,
                    ModifiedTimestamp = DateTime.UtcNow
                },
                Roles = rolesAssigned
            };
            return View(model);

            _userService.UpdateUser(userEntity);

            return View(model);
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