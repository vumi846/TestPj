using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VNPTPM.Model.Validate
{
    public class CustomValidation
    {
        public static Boolean IsNumber(string value) {
            Boolean status = false;
            var match = Regex.Match(value, "^(([0-9]*)|(([0-9]*)\\.([0-9]*)))$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                status = true;
            }

            return status;
        }

        public static Boolean IsNumberInteger(string value)
        {
            Boolean status = false;
            var match = Regex.Match(value, "^[1-9][0-9]*$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                status = true;
            }

            return status;
        }

        public static Boolean maxLength(int max, string value)
        {
             Boolean status = false;
             if(value.Length <= max)
             {
                status = true;
            }

            return status;
        }

        public static Boolean hasSpace(string value)
        {
            Boolean status = false;
            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c))
                    return true;
            }

            return status;
        }

        //public static Boolean IsDateWithFormat(string value, string formats)
        //{
        //    Boolean status = false;
        //    DateTime Test;
        //    if (DateTime.TryParseExact(value, formats, null, DateTimeStyles.None, out Test) == true)
        //        status = true;

        //    return status;
        //}

        public static Boolean IsPhoneNumber(string phone, bool IsRequired)
        {
            if (string.IsNullOrEmpty(phone) & !IsRequired)
                return true;

            if (string.IsNullOrEmpty(phone) & IsRequired)
                return false;

            var cleaned = CustomValidation.RemoveNonNumeric(phone);
            if (IsRequired)
            {
                if (cleaned.Length == 10)
                    return true;
                else
                    return false;
            }
            else
            {
                if (cleaned.Length == 0)
                    return true;
                else if (cleaned.Length > 0 & cleaned.Length < 10)
                    return false;
                else if (cleaned.Length == 10)
                    return true;
                else
                    return false;
            }
        }

        public static string RemoveNonNumeric(string phone)
        {
            return Regex.Replace(phone, @"[^0-9]+", "");
        }
    }
}
