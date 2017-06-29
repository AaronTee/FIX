using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;
using static FIX.Service.DBConstant;

namespace FIX.Web.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToUserLocalDate(this DateTime d, string TimeZoneId)
        {
            if (d == null) return string.Empty;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var tzTime = TimeZoneInfo.ConvertTimeFromUtc(d, tz);

            return tzTime.ConvertToDateString();
        }


        public static string ConvertToDateString(this DateTime d)
        {
            return d.ToString(DBCDateFormat.ddMMyyyy);
        }
    }
}