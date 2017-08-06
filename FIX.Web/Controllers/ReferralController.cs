using FIX.Service;
using FIX.Service.Entities;
using FIX.Web.Extensions;
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
    [Authorize]
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

            var parentUser = _userService.GetUserBy(uid);

            if (uid != 0 || uid != null)
            {
                childrenInfo = _userService.GetReferralChildren(uid).ToList();
                var treeChildren = new Func<List<ReferralTreeNode>>(() => {
                    List<ReferralTreeNode> value = new List<ReferralTreeNode>();
                    foreach (var c in childrenInfo)
                    {
                        var hasChildren = (curLevel < DBConstant.MAX_REFERRAL_TREE_LEVEL) ? _userService.GetReferralChildren(c.UserId).Count() > 0 : false;

                        var _text = c.Username + " ";
                        var userPackages = c.UserPackage.OrderByDescending(x => x.Package.Threshold).ToList();

                        //Get first three package order by descending amount of package.
                        for (int i = 0; (i < DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE && i < userPackages.Count()); i++)
                        {
                            _text += "[" + userPackages[i].Date.ToUserLocalDate(User.Identity.GetUserTimeZone()) + "|" + userPackages[i].Package.Description + "]" + ((userPackages.Count - 1 != i) ? ", " : "");
                        }
                        if(userPackages.Count > DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE)
                        {
                            _text += "and " + (userPackages.Count() - DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE) + " more.";
                        }

                        value.Add(new ReferralTreeNode
                        {
                            id = c.UserId,
                            text = _text,
                            children = hasChildren,
                            icon = "glyphicon glyphicon-user " + ((userPackages.Count > 0) ? userPackages.First().Package.Description.ToLower() : "")
                        });
                    }

                    return value;
                })();

                var _parentText = parentUser.Username + " ";
                var _parentUserPackages = parentUser.UserPackage.OrderByDescending(x => x.Package.Threshold).ToList();

                //Get first three package order by descending amount of package.
                for (int i = 0; (i < DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE && i < _parentUserPackages.Count()); i++)
                {
                    _parentText += "[" + _parentUserPackages[i].Date.ToUserLocalDate() + "|" + _parentUserPackages[i].Package.Description + "]" + ((_parentUserPackages.Count-1 != i) ? ", " : "");
                }
                if (_parentUserPackages.Count > DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE)
                {
                    _parentText += "and " + (_parentUserPackages.Count() - DBConstant.MAX_REFERRAL_TREE_SHOW_PACKAGE) + " more.";
                }

                treeNode = new ReferralTreeNode
                {
                    id = uid.Value,
                    text = _parentText,
                    children = treeChildren,
                    icon = "glyphicon glyphicon-user " + ((_parentUserPackages.Count > 0) ? _parentUserPackages.First().Package.Description.ToLower() : "")
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

            if (searchUser.UserId == User.Identity.GetUserId<int>())
            {
                return Json(new
                {
                    isSelf = true
                }, JsonRequestBehavior.AllowGet);
            }
            
            if (searchUser != null)
            {
                searchUserParentId = searchUser.UserProfile.ReferralId;
                while (iterationCount < DBConstant.MAX_REFERRAL_TREE_SEARCH_LEVEL && !searchUserParentId.IsNullOrEmpty())
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