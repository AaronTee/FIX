using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using static FIX.Service.DBConstant;

namespace FIX.Web.Extensions
{
    public static class IntegerExtension
    {
        public static bool IsNullOrEmpty(this int? val)
        {
            return (val == null || val == 0);
        }
    }
}