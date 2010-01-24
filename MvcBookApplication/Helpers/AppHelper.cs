using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Security;
using MvcBookApplication.Data.Models;
using Ninject.Core;

namespace MvcBookApplication
{
    public static class AppHelper
    {
      

        public static string GetSortDirection(this ViewDataDictionary ViewData,
            string field)
        {
            return (string)ViewData["sortby"] != field
                       ? "asc"
                       : ((string)ViewData["sortdir"] == "asc"
                              ? "desc"
                              : "asc");
        }

       
    }


}
