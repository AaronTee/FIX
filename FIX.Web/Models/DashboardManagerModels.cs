using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FIX.Service.DBConstant;

namespace FIX.Web.Models
{
    public class DashboardManagerViewModels
    {
        public DashboardManagerViewModels()
        {
            PostTypeDDL = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem() { Text = EPostType.Announcement.ToString(), Value = ((int)EPostType.Announcement).ToString() },
            }, "Value", "Text", PostType);
        }

        public int PostId { get; set; }

        [Display(Name = "Board")]
        public int PostType { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedTimestamp { get; set; }
        public string ModifiedTimestamp { get; set; }

        public SelectList PostTypeDDL { get; set; }
    }

    public class ArticlePostModels
    {
        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public int? PostType { get; set; }
    }

}