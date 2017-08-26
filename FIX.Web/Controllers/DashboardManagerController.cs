using FIX.Service;
using FIX.Service.Entities;
using FIX.Web.Extensions;
using FIX.Web.Models;
using Microsoft.AspNet.Identity;
using SyntrinoWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FIX.Service.DBConstant;

namespace FIX.Web.Controllers
{
    [Authorize(Roles = DBCRole.Admin)]
    public class DashboardManagerController : Controller
    {
        private IArticleService _articleService;

        public DashboardManagerController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public ActionResult Index()
        {
            DashboardManagerViewModels model = new DashboardManagerViewModels();

            return View(model);
        }

        public JsonResult GetPostContent(int postType)
        {
            var status = EJState.Unknown;
            var post = _articleService.GetPost(postType);
            if (post != null)
            {
                status = EJState.Success;
                return Json(new
                {
                    status = status,
                    title = post.Title,
                    content = post.Content,
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult SetPostContent(ArticlePostModels article)
        {
            var status = EJState.Failed;

            if (article != null)
            {
                if (article.PostType != null)
                {
                    var post = _articleService.GetPost(article.PostType.Value);

                    if (post != null)
                    {
                        post.Title = article.Title;
                        post.Content = article.Content;

                        _articleService.UpdatePost(post);
                    }
                    else
                    {
                        Post newPost = new Post
                        {
                            Title = article.Title,
                            Content = article.Content,
                            PostType = article.PostType.Value,
                            CreatedBy = User.Identity.Name,
                            CreatedTimestamp = DateTime.UtcNow
                        };
                        _articleService.InsertPost(newPost);
                    }

                    if (_articleService.SaveChange(User.Identity.GetUserId<int>()))
                    {
                        status = EJState.Success;
                    }
                }
            }

            return Json(new
            {
                status = status
            }, JsonRequestBehavior.AllowGet);
        }
    }
}