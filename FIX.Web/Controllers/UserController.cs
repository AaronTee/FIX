using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FIX.Web.Models;
using FIX.Core.Data;
using FIX.Service.Interface;

namespace FIX.Web.Controllers
{
    public class UserController : BaseController
    {
        private IUserService userService;

        public UserController(IUserService userService, IBaseService baseService) : base(baseService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<UserModel> users = userService.GetUsers().Select(u => new UserModel
            {
                FirstName = u.UserProfile.FirstName,
                LastName = u.UserProfile.LastName,
                Email = u.Email,
                Address = u.UserProfile.Address,
                ID = u.ID
            });
            return View(users);
        }

        [HttpGet]
        public ActionResult CreateEditUser(int? id)
        {
            UserModel model = new UserModel();
            if (id.HasValue && id != 0)
            {
                User userEntity = userService.GetUser(id.Value);
                model.FirstName = userEntity.UserProfile.FirstName;
                model.LastName = userEntity.UserProfile.LastName;
                model.Address = userEntity.UserProfile.Address;
                model.Email = userEntity.Email;
                model.UserName = userEntity.Username;
                model.Password = userEntity.Password;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateEditUser(UserModel model)
        {
            if (model.ID == 0)
            {
                User userEntity = new User
                {
                    Username = model.UserName,
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
                    }
                };
                userService.InsertUser(userEntity);
                if (userEntity.ID > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                User userEntity = userService.GetUser(model.ID);
                userEntity.Username = model.UserName;
                userEntity.Email = model.Email;
                userEntity.Password = model.Password;
                userEntity.ModifiedTimestamp = DateTime.UtcNow;
                userEntity.IP = Request.UserHostAddress;
                userEntity.UserProfile.FirstName = model.FirstName;
                userEntity.UserProfile.LastName = model.LastName;
                userEntity.UserProfile.Address = model.Address;
                userEntity.UserProfile.ModifiedTimestamp = DateTime.UtcNow;
                userService.UpdateUser(userEntity);
                if (userEntity.ID > 0)
                {
                    return RedirectToAction("Index");
                }

            }
            return View(model);
        }

        public ActionResult Detail(int? id)
        {
            UserModel model = new UserModel();
            if (id.HasValue && id != 0)
            {
                User userEntity = userService.GetUser(id.Value);

                model.FirstName = userEntity.UserProfile.FirstName;
                model.LastName = userEntity.UserProfile.LastName;
                model.Address = userEntity.UserProfile.Address;
                model.Email = userEntity.Email;
                model.CreatedTimestamp = userEntity.CreatedTimestamp;
                model.UserName = userEntity.Username;
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            UserModel model = new UserModel();
            if (id != 0)
            {
                User userEntity = userService.GetUser(id);
                model.FirstName = userEntity.UserProfile.FirstName;
                model.LastName = userEntity.UserProfile.LastName;
                model.Address = userEntity.UserProfile.Address;
                model.Email = userEntity.Email;
                model.CreatedTimestamp = userEntity.CreatedTimestamp;
                model.UserName = userEntity.Username;
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
                    User userEntity = userService.GetUser(id);
                    userService.DeleteUser(userEntity);
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