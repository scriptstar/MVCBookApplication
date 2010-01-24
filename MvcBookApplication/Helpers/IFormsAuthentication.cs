using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Models;
using Ninject.Core;

namespace MvcBookApplication
{
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
    }
}
