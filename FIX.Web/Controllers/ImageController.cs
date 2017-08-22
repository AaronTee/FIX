using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FIX.Web.Controllers
{
    [Authorize]
    public class ImageController : BaseController
    {
        public ActionResult Image(string ImageName)
        {
            string photorootpath = ConfigurationManager.AppSettings["PhotoUploadPath"];
            if (String.IsNullOrEmpty(photorootpath))
                throw new Exception("PhotoRootPath is not define in Web Config"); //
            string photofullpath = photorootpath + "\\" + ImageName.Trim();
            return base.File(photofullpath, "image/jpeg", ImageName);
        }

        public ActionResult ImageUploadReceipt(string ImageName)
        {
            string photorootpath = ConfigurationManager.AppSettings["UploadReceiptPhotoPath"];
            if (String.IsNullOrEmpty(photorootpath))
                throw new Exception("PhotoRootPath is not define in Web Config"); //
            string photofullpath = photorootpath + "\\" + ImageName.Trim();
            return base.File(photofullpath, "image/jpeg", ImageName);
        }

        internal bool Upload(string relativePath, HttpPostedFileBase file, string saveAsName)
        {
            try
            {
                string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                string fileName = saveAsName;
                string targetPath = Path.Combine(targetFolder, fileName);
                file.SaveAs(targetPath);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return false;
            }
        }

        public ActionResult ImageWithSubFolder(string imagename, string subfolder)
        {
            string photorootpath = ConfigurationManager.AppSettings["PhotoUploadPath"];
            if (String.IsNullOrEmpty(photorootpath))
                throw new Exception("PhotoRootPath is not define in Web Config"); //
            string photofullpath = photorootpath + "\\" + subfolder + "\\" + imagename;
            return base.File(photofullpath, "image/jpeg", imagename);

        }

        public ActionResult ImageDefaultPath(string imagename)
        {
            string photorootpath = ConfigurationManager.AppSettings["PhotoUploadPath"];
            if (String.IsNullOrEmpty(photorootpath))
                throw new Exception("PhotoRootPath is not define in Web Config"); //
            string photofullpath = photorootpath + "\\" + imagename;
            return base.File(photofullpath, "image/jpeg", imagename);

        }

        public ActionResult ImageWithThumnbail(string DistriCode, string SalesmanCode, string imagename, string subfolder, int width, int height)
        {
            Image img = null;
            try
            {
                string photorootpath = ConfigurationManager.AppSettings["PhotoUploadPath"];
                if (String.IsNullOrEmpty(photorootpath))
                    throw new Exception("PhotoRootPath is not define in Web Config"); //
                string imagepath = photorootpath + "\\" + DistriCode + "\\" + SalesmanCode + "\\" + imagename.Trim();
                img = System.Drawing.Image.FromFile(imagepath);
                if (img == null)
                {
                    throw new ArgumentNullException("Image");
                }
                var ms = GetMemoryStream(img, width, height, ImageFormat.Jpeg);

                // output
                Response.Clear();
                Response.ContentType = "image/jpeg";

                //var a = System.Drawing.Image.Save(ms, ImageFormat.Jpeg);
                return File(ms, Response.ContentType);
            }
            catch (Exception ex)
            {
                //img = new Bitmap(1, 1);
                return null;
            }
            finally
            {
                if (img != null) img.Dispose();
            }
        }

        public ActionResult ImageWithSubFolderThumnbail(string imagename, string subfolder, int width, int height)
        {
            Image img = null;
            try
            {
                string photorootpath = ConfigurationManager.AppSettings["PhotoUploadPath"];
                if (String.IsNullOrEmpty(photorootpath))
                    throw new Exception("PhotoRootPath is not define in Web Config"); //
                string imagepath = photorootpath + "\\" + subfolder + "\\" + imagename;
                img = System.Drawing.Image.FromFile(imagepath);
                if (img == null)
                {
                    throw new ArgumentNullException("Image");
                }
                var ms = GetMemoryStream(img, width, height, ImageFormat.Jpeg);

                // output
                Response.Clear();
                Response.ContentType = "image/jpeg";

                //var a = System.Drawing.Image.Save(ms, ImageFormat.Jpeg);
                return File(ms, Response.ContentType);
            }
            catch (Exception ex)
            {
                //img = new Bitmap(1, 1);
                return null;
            }
            finally
            {
                if (img != null) img.Dispose();
            }
        }


        private static MemoryStream GetMemoryStream(Image input, int width, int height, ImageFormat fmt)
        {
            // maintain aspect ratio
            if (input.Width > input.Height)
                height = input.Height * width / input.Width;
            else
                width = input.Width * height / input.Height;

            var bmp = new Bitmap(input, width, height);
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            return ms;
        }


	}
}