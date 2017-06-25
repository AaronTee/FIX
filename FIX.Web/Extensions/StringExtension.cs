using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIX.Web.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this String str)
        {
            return (str == null || str == string.Empty);
        }

        public static bool IsNotNullOrEmpty(this String str)
        {
            return (str != null && str != string.Empty);
        }
    }
}