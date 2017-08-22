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
            var tzTime = d.ToUserLocalDateTimeAsDateTime(TimeZoneId);

            return tzTime.ConvertToDateString();
        }

        public static string ToUserLocalDateTime(this DateTime d, string TimeZoneId = null)
        {
            if (d == null) return string.Empty;
            var tzTime = d.ToUserLocalDateTimeAsDateTime(TimeZoneId);

            return tzTime.ConvertToDateTimeString();
        }

        public static DateTime ToUserLocalDateTimeAsDateTime(this DateTime d, string TimeZoneId = null)
        {
            if (TimeZoneId == null) TimeZoneId = "UTC";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(d, tz);
        }

        public static string ConvertToDateYearMonthString(this DateTime d, string TimeZoneId = null)
        {
            return d.ToUserLocalDateTimeAsDateTime(TimeZoneId).ToString(DBCDateFormat.MMMyyyy);
        }

        public static string ConvertToDateString(this DateTime d, string TimeZoneId = null)
        {
            return d.ToUserLocalDateTimeAsDateTime(TimeZoneId).ToString(DBCDateFormat.ddMMMyyyy);
        }

        public static string ConvertToDateTimeString(this DateTime d, string TimeZoneId = null)
        {
            return d.ToUserLocalDateTimeAsDateTime(TimeZoneId).ToString(DBCDateFormat.ddMMMyyyyHHmmsstt);
        }

        public static string ConvertToPlainDateTimeString(this DateTime d, string TimeZoneId = null)
        {
            return d.ToUserLocalDateTimeAsDateTime(TimeZoneId).ToString(DBCDateFormat.PS_yyyyMMddhhmmss);
        }
    }
}