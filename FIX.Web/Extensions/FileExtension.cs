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

        public static readonly string[] _validExtensions = { "jpg", "png", "jpeg" }; //  etc
        public static bool IsImage(this HttpPostedFileBase file)
        {
            return file.ContentType.Contains(file.ContentType);
        }
    }
}