using FIX.Web.Extensions;
using FIX.Web.Models;
using FIX.Web.Utils;
using SyntrinoWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static FIX.Service.DBConstant;

namespace FIX.Web.Controllers
{
    [Authorize]
    [IdentityAuthorize]
    public class MonthlyReportController : BaseController
    {
        // GET: MonthlyReport
        public ActionResult Index()
        {
            MonthlyReportModels model = new MonthlyReportModels();
            model.Date = DateTime.UtcNow.ConvertToDateYearMonthString(User.Identity.GetUserTimeZone());
            var pdfs = FileManager.GetMonthlyReportsPDFInfo();
            var searchTerm = model.Date.Replace("/", "").ToLower();
            model.PDFName = pdfs.Where(x => x.Name.ToLower().Contains(searchTerm)).FirstOrDefault()?.Name;
            
            return View(model);
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Manage()
        {
            MonthlyReportManageModels model = new MonthlyReportManageModels
            {
                Date = DateTime.UtcNow.ConvertToDateYearMonthString(User.Identity.GetUserTimeZone()),
            };

            return View(model);
        }

        [Authorize(Roles = DBCRole.Admin)]
        [HttpPost]
        public ActionResult New(MonthlyReportManageModels model, HttpPostedFileBase PDFFile)
        {

            if (PDFFile == null) ModelState.AddModelError("", "Upload file cannot be empty.");

            if (ModelState.IsValid)
            {
                if (PDFFile.IsPDF())
                {
                    try
                    {
                        //PDF Format : [MM-yyyy_pdfname.pdf]
                        string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PDF_MonthlyReportPath"]);
                        string fileName = model.Date.Replace("/", "") + ".pdf";
                        string targetPath = Path.Combine(targetFolder, fileName);
                        PDFFile.SaveAs(targetPath);

                        Success("Successfully uploaded report " + PDFFile.FileName + ". (Renamed to " + fileName + ")");
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.Message, ex);
                        Warning("Something went wrong when server trying to upload this file, if problem still persist please contact administrator.");
                    }
                }
                else
                {
                    Warning("Upload File is not a valid PDF file. Please choose a valid PDF file to upload.");
                }
                
            }
            else
            {
                Warning("Please make sure input field is correctly filled in.");
            }

            return RedirectToAction("Manage");
        }

        [Authorize(Roles = DBCRole.Admin)]
        public JsonResult MonthlyReportList(int offset, int limit, string search, string sort, string order)
        {
            var pdfs = FileManager.GetMonthlyReportsPDFInfo();
            var data = pdfs.Select(x => new MonthlyReportFileListView
            {
                Date = x.Name,
                ActionLinks = new Func<List<ActionLink>>(() =>
                {
                    List<ActionLink> actions = new List<ActionLink>();

                    actions.Add(new ActionLink
                    {
                        Name = "Download",
                        Url = Url.Action("Download", new
                        {
                            PDFFilename = x.Name
                        })
                    });

                    actions.Add(new ActionLink
                    {
                        Name = "Delete",
                        Url = Url.Action("Delete", new
                        {
                            PDFFilename = x.Name
                        })
                    });

                    return actions;
                })()
            }).ToList();

            var model = new
            {
                total = data.Count,
                rows = data
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Download(string PDFFilename)
        {
            string fullPDFFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PDF_MonthlyReportPath"]) + PDFFilename;
            var documentData = System.IO.File.ReadAllBytes(fullPDFFilePath);

            return File(documentData, "application/pdf");
        }

        [Authorize(Roles = DBCRole.Admin)]
        public ActionResult Delete(string PDFFilename)
        {
            try
            {
                string fullPDFFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PDF_MonthlyReportPath"]) + PDFFilename;
                if ((System.IO.File.Exists(fullPDFFilePath)))
                {
                    System.IO.File.Delete(fullPDFFilePath);
                    Success("Successfully deleted report " + PDFFilename + ".");
                }
                else
                {
                    Warning("This report has either been removed or not found in server, please contact administrator if you believe it is an error.");

                }
            }
            catch (Exception ex)
            {
                Danger("Seomthing wrong while we removing the report, please contact administrator if problem persist.");
                Log.Error(ex.Message, ex);
            }

            return RedirectToAction("Manage");
        }
    }
}