using System;
using System.ComponentModel.DataAnnotations;

namespace MvcBookApplication.Validation
{
    public class DateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            if (value.GetType() == typeof(DateTime))
            {
                return true;
            }
            if (value.GetType() == typeof(string))
            {
                DateTime result;
                var svalue = value as string;
                return string.IsNullOrEmpty(svalue) || DateTime.TryParse(svalue, out result);
            }
            return false;
        }
    }
}