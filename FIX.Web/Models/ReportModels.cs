using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace FIX.Web.Models
{
    public class MonthlyReportModels
    {
        public string Date { get; set; }
        public string PDFName { get; set; }
    }

    public class MonthlyReportManageModels
    {
        [Display(Name = "Month")]
        public string Date { get; set; }

        [Display(Name = "PDF File")]
        public HttpPostedFileBase PDFFile { get; set; }
    }

    public class MonthlyReportFileListView : ActionsLink
    {
        public string Date { get; set; }
    }
}