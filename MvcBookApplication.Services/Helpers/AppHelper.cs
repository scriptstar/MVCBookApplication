using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Security;
using MvcBookApplication.Data.Models;
using Ninject.Core;

namespace MvcBookApplication.Services
{
    public static class AppHelper
    {
        public static bool IsValidEmail(string email)
        {
            var EmailExpression =
                new Regex(
                    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|" +
                    @"(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            return EmailExpression.IsMatch(email);
        }
public static bool IsValidUsername(string username)
{
    if (string.IsNullOrEmpty(username))
        return false;

    if (username.Length < 4)
        return false;

    var UsernameExpression =
        new Regex(@"^[a-zA-Z][a-zA-Z0-9_]+$",
                  RegexOptions.Compiled |
                  RegexOptions.IgnoreCase |
                  RegexOptions.Singleline);
    var valid = UsernameExpression.IsMatch(username);

    return valid;
}

     

        public static string ToShortDateString(this DateTime? date)
        {
            return date == null ? "" : ((DateTime)date).ToShortDateString();
        }
    }
}