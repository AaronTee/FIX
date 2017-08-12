using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Extensions
{
    public static class FileExtension
    {
        public static bool IsPDF(this HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("pdf"))
            {
                return true;
            }

            string[] formats = new string[] { ".pdf" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}