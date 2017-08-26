using FIX.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace FIX.Web.Utils
{
    public static class Emailer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Send(string bodyContent, string title, string fromEmail, string toEmail)
        {
            using (var mm = new MailMessage())
            {
                try
                {
                    SmtpClient smtp = new SmtpClient();

                    mm.From = new MailAddress(fromEmail);
                    mm.To.Add(toEmail);
                    mm.Subject = title;
                    mm.Body = bodyContent;
                    mm.IsBodyHtml = true;
                    mm.BodyEncoding = System.Text.Encoding.UTF8;
                    mm.SubjectEncoding = System.Text.Encoding.Default;

                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
                finally
                {
                    mm.Dispose();
                }
            }
        }
    }
}