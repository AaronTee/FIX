using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIX.Web.Utils
{
    public static class FileManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<FileInfo> GetMonthlyReportsPDFInfo()
        {
            try
            {
                string pdfAbsolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["PDF_MonthlyReportPath"]);
                if (String.IsNullOrEmpty(pdfAbsolutePath))
                    throw new Exception("PDF_MonthlyReportPath is not define in Web Config");

                List<FileInfo> files = new List<FileInfo>();
                DirectoryInfo d = new DirectoryInfo(pdfAbsolutePath);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.pdf"); //Getting PDF files

                foreach (FileInfo file in Files)
                {
                    files.Add(file);
                }

                return files;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);   
            }

            return new List<FileInfo>();
        }
    }
}