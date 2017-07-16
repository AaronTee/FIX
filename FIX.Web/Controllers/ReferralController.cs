using FIX.Service;
using FIX.Service.Entities;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Controllers
{
    public class ReferralController : BaseController
    {
        private IUserService _userService;

        public ReferralController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Referral
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Follower(int? uid, int curLevel)
        {
            List<User> childrenInfo = new List<User>();
            ReferralTreeNode treeNode = new ReferralTreeNode();

            var curUsername = _userService.GetUserBy(uid).Username;

            if (uid != 0 || uid != null)
            {
                childrenInfo = _userService.GetReferralChildren(uid).ToList();
                var treeChildren = new Func<List<ReferralTreeNode>>(() => {
                    List<ReferralTreeNode> value = new List<ReferralTreeNode>();
                    foreach (var c in childrenInfo)
                    {
                        var hasChildren = (curLevel < DBConstant.MAX_REFERRAL_TREE_LEVEL) ? _userService.GetReferralChildren(c.UserId).Count() > 0 : false;

                        value.Add(new ReferralTreeNode
                        {
                            id = c.UserId,
                            text = c.Username,
                            children = hasChildren,
                            //icon = (hasChildren) ? "glyphicon glyphicon-plus" : null
                        });
                    }

                    return value;
                })();

                treeNode = new ReferralTreeNode
                {
                    id = uid.Value,
                    text = curUsername,
                    //icon = (treeChildren.Count > 0) ? "glyphicon glyphicon-plus" : null,
                    children = treeChildren
                };

                string output = JsonConvert.SerializeObject(treeNode);
            }

            return Json(treeNode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchFollower(string username)
        {
            Int32 iterationCount = 1;
            bool isFollower = false;
            Int32? searchUserParentId = 0;

            var searchUser = _userService.GetUserBy(username);
            
            if (searchUser != null)
            {
                searchUserParentId = searchUser.UserProfile.ReferralId;
                while (iterationCount < DBConstant.MAX_REFERRAL_TREE_SEARCH_LEVEL && searchUserParentId != null)
                {
                    if (searchUserParentId == User.Identity.GetUserId<int>())
                    {
                        isFollower = true;
                        break;
                    }

                    //find next
                    searchUserParentId = _userService.GetUserBy(searchUserParentId).UserProfile.ReferralId;
                    iterationCount++;
                }
            }

            return Json(new
            {
                atLevel = iterationCount,
                isFollower = isFollower
                //childData = (isFollower) ? JsonConvert.SerializeObject(childUser) : null
            }, JsonRequestBehavior.AllowGet);

        }
    }
}