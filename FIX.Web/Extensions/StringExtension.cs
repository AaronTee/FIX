using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string ConvertStringToHex(this String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(this String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}