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
        public static string ToUserLocalDate(this DateTime d, string TimeZoneId = null)
        {
            if (d == null) return string.Empty;
            if (TimeZoneId == null) TimeZoneId = "UTC";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var tzTime = TimeZoneInfo.ConvertTimeFromUtc(d, tz);

            return tzTime.ConvertToDateString();
        }

        public static string ToUserLocalDateTime(this DateTime d, string TimeZoneId = null)
        {
            if (d == null) return string.Empty;
            if (TimeZoneId == null) TimeZoneId = "UTC";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            var tzTime = TimeZoneInfo.ConvertTimeFromUtc(d, tz);

            return tzTime.ConvertToDateTimeString();
        }

        public static string ConvertToDateYearMonthString(this DateTime d)
        {
            return d.ToString(DBCDateFormat.MMMyyyy);
        }

        public static string ConvertToDateString(this DateTime d)
        {
            return d.ToString(DBCDateFormat.ddMMMyyyy);
        }

        public static string ConvertToDateTimeString(this DateTime d)
        {
            return d.ToString(DBCDateFormat.ddMMMyyyyHHmmsstt);
        }
    }
}