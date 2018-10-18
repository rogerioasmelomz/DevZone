using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Helpers
{
    public static class Extensions
    {

        #region STRING EXTENSIONS

        public static string ToSqlFormat(this DateTime value) { return (value.ToString("yyyy-MM-dd")); }

        public static bool IsEmpty(this string value) { return (string.IsNullOrWhiteSpace(value)); }

        public static string Left(this string value, int length) {

            string strResult= string.Empty;
            try
            {
                if (!value.IsEmpty())
                {
                    if (value.Length > length)
                    {
                        strResult = value.Substring(0, length);
                    }
                    else
                    {
                        strResult = value;
                    }
                }
            }
            catch (Exception)
            {
            }

            return (strResult);
        }

        public static string Right(this string value, int length)
        {

            string strResult = string.Empty;
            try
            {
                if (!value.IsEmpty())
                {

                    strResult =  value.Length <= length ? value : value.Substring(value.Length - length);
                }
            }
            catch (Exception)
            {
            }

            return (strResult);
        }

        public static void Format(this string value) { }

        #endregion
    }
}
