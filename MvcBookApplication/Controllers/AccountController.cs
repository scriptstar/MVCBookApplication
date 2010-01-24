using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Models;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    [HandleError]
    [OutputCache(Location = OutputCacheLocation.None)]
    public class AccountController : Controller
    {
        public AccountController()
            : this(null, null)
        {
        }

        [Inject]
        public AccountController(IFormsAuthentication formsAuth,
            MembershipProvider provider)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationWrapper();
            Provider = provider ?? Membership.Provider;
        }

        public IFormsAuthentication FormsAuth { get; private set; }

        public MembershipProvider Provider { get; private set; }


        //[Authorize]
        //public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        //{

        //    ViewData["Title"] = "Change Password";
        //    ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

        //    // Non-POST requests should just display the ChangePassword form 
        //    if (Request.HttpMethod != "POST")
        //    {
        //        return View();
        //    }

        //    // Basic parameter validation
        //    List<string> errors = new List<string>();

        //    if (String.IsNullOrEmpty(currentPassword))
        //    {
        //        errors.Add("You must specify a current password.");
        //    }
        //    if (newPassword == null || newPassword.Length < Provider.MinRequiredPasswordLength)
        //    {
        //        errors.Add(String.Format(CultureInfo.InvariantCulture,
        //                 "You must specify a new password of {0} or more characters.",
        //                 Provider.MinRequiredPasswordLength));
        //    }
        //    if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
        //    {
        //        errors.Add("The new password and confirmation password do not match.");
        //    }

        //    if (errors.Count == 0)
        //    {

        //        // Attempt to change password
        //        MembershipUser currentUser = Provider.GetUser(User.Identity.Name, true /* userIsOnline */);
        //        bool changeSuccessful = false;
        //        try
        //        {
        //            changeSuccessful = currentUser.ChangePassword(currentPassword, newPassword);
        //        }
        //        catch
        //        {
        //            // An exception is thrown if the new password does not meet the provider's requirements
        //        }

        //        if (changeSuccessful)
        //        {
        //            return RedirectToAction("ChangePasswordSuccess");
        //        }
        //        else
        //        {
        //            errors.Add("The current password is incorrect or the new password is invalid.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewData["errors"] = errors;
        //    return View();
        //}

        //public ActionResult ChangePasswordSuccess()
        //{

        //    ViewData["Title"] = "Change Password";

        //    return View();
        //}

        //public ActionResult Login(string username, string password, bool? rememberMe)
        //{

        //    ViewData["Title"] = "Login";

        //    // Non-POST requests should just display the Login form 
        //    if (Request.HttpMethod != "POST")
        //    {
        //        return View();
        //    }

        //    // Basic parameter validation
        //    List<string> errors = new List<string>();

        //    if (String.IsNullOrEmpty(username))
        //    {
        //        errors.Add("You must specify a username.");
        //    }

        //    if (errors.Count == 0)
        //    {

        //        // Attempt to login
        //        bool loginSuccessful = Provider.ValidateUser(username, password);

        //        if (loginSuccessful)
        //        {

        //            FormsAuth.SetAuthCookie(username, rememberMe ?? false);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            errors.Add("The username or password provided is incorrect.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    ViewData["errors"] = errors;
        //    ViewData["username"] = username;
        //    return View();
        //}

        public ActionResult Logout()
        {
            FormsAuth.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        [AcceptVerbs("GET")]
        public ActionResult Register()
        {
            ViewData["Title"] = "Register";
            return View();
        }

        [AcceptVerbs("POST")]
        public ActionResult Register([Bind(Prefix = "")] RegisterModel model)
        {
            //todo refactor register method to take in the registermodel
            //validate input
            if (string.IsNullOrEmpty(model.Username))
                ViewData.ModelState.AddModelError("username",
                                                  "Username is required");

            else if (!Services.AppHelper.IsValidUsername(model.Username))
            {
                ViewData.ModelState.AddModelError("username",
                                                  "Username is invalid");
            }
            if (string.IsNullOrEmpty(model.Email))
                ViewData.ModelState.AddModelError("email",
                                                  "Email is required");
            else if (!Services.AppHelper.IsValidEmail(model.Email))
                ViewData.ModelState.AddModelError("email",
                                                  "Email is invalid");

            if (string.IsNullOrEmpty(model.Password))
                ViewData.ModelState.AddModelError("password",
                                                  "Password is required");
            if (string.IsNullOrEmpty(model.Question))
                ViewData.ModelState.AddModelError("question",
                                                  "Question is required");
            if (string.IsNullOrEmpty(model.Answer))
                ViewData.ModelState.AddModelError("answer",
                                                  "Answer is required");


            //var model = new RegisterModel
            //                {
            //                    Username = username,
            //                    Email = email,
            //                    Password = password,
            //                    Question = question,
            //                    Answer = answer
            //                };

            //if validation fails then return the view
            if (!ViewData.ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to register the user
            MembershipCreateStatus createStatus;
            var newUser = Provider.CreateUser(model.Username, model.Password, model.Email,
                                              model.Question, model.Answer, true,
                                              null, out createStatus);
            if (newUser != null)
            {
                FormsAuth.SetAuthCookie(model.Username, false);
                return RedirectToAction("Index", "Home");
            }

            //if we get this far, there was an error
            ViewData.ModelState.AddModelError("provider",
                                              ErrorCodeToString(createStatus));
            return View(model);
        }


public static string ErrorCodeToString(MembershipCreateStatus createStatus)
{
    // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
    // a full list of status codes.
    switch (createStatus)
    {
        case MembershipCreateStatus.DuplicateUserName:
            return "Username already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
            return "A username for that e-mail address already exists. " + 
                "Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
            return "The password provided is invalid. " + 
                "Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
            return "The e-mail address provided is invalid. " + 
                "Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
            return "The password retrieval answer provided is invalid. " + 
                "Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
            return "The password retrieval question provided is invalid. " + 
                "Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
            return "The user name provided is invalid. " + 
                "Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
            return
                "The authentication provider returned an error. " + 
                "Please verify your entry and try again. " + 
                "If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
            return
                "The user creation request has been canceled. " + 
                "Please verify your entry and try again. " + 
                "If the problem persists, please contact your system administrator.";

        default:
            return
                "An unknown error occurred. Please verify your entry and try again. " + 
                "If the problem persists, please contact your system administrator.";
    }
}


        [AcceptVerbs("get")]
        public ActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        [AcceptVerbs("POST")]
        public ActionResult Login([Bind(Prefix = "")] LoginModel model)
        {
            ViewData["Title"] = "Login";
            // Basic parameter validation
            if (string.IsNullOrEmpty(model.Username))
                ViewData.ModelState.AddModelError("username",
                                                  "Username is required");
            if (!ViewData.ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to login
            var loginSuccessful = Provider.ValidateUser(model.Username, model.Password);

            if (loginSuccessful)
            {
                FormsAuth.SetAuthCookie(model.Username, model.RememberMe);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData.ModelState.AddModelError("provider",
                                                  "The username or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AcceptVerbs("get")]
        public ActionResult ResetPassword()
        {
            ViewData["Title"] = "Reset Password";
            return View();
        }

        [AcceptVerbs("POST")]
        public ActionResult ResetPassword(string usernameOrEmail)
        {
            ViewData["Title"] = "Reset Password";
            ViewData["usernameOrEmail"] = usernameOrEmail;
            if (string.IsNullOrEmpty(usernameOrEmail))
                ViewData.ModelState.AddModelError("username",
                                                  "Username or email are required");
            if (!ViewData.ModelState.IsValid)
            {
                return View();
            }

            var user = Provider.GetUser(usernameOrEmail, false);
            var username = usernameOrEmail;
            if (user == null)
            {
                //try getting by email
                username = Provider.GetUserNameByEmail(usernameOrEmail);
                if (!string.IsNullOrEmpty(username))
                    user = Provider.GetUser(username, false);
            }

            if (user == null)
            {
                ViewData.ModelState.AddModelError("username",
                                                  "This username/email doesn't exist.");
                return View();
            }

            //user is found
            return RedirectToAction("ResetPasswordQuestion",
                                            new ResetPasswordModel
                                                {
                                                    Username = username,
                                                    Question = user.PasswordQuestion
                                                });


        }

        [AcceptVerbs("get")]
        public ActionResult ResetPasswordQuestion([Bind(Prefix = "")] ResetPasswordModel model)
        {
            ViewData["Title"] = "Reset Password";
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Question))
            {
                return RedirectToAction("ResetPassword");
            }
            return View(model);
        }

        [AcceptVerbs("post")]
        [ActionName("ResetPasswordQuestion")]
        public ActionResult ResetPasswordQuestionSubmit([Bind(Prefix = "")] ResetPasswordModel model)
        {
            ViewData["Title"] = "Reset Password";
            if (string.IsNullOrEmpty(model.Username))
            {
                ViewData.ModelState.AddModelError("username",
                                                  "Username is required");
            }

            if (string.IsNullOrEmpty(model.Question))
            {
                ViewData.ModelState.AddModelError("question",
                                                  "Question is required");
            }

            if (string.IsNullOrEmpty(model.Answer))
            {
                ViewData.ModelState.AddModelError("answer",
                                                  "Answer is required");
            }
            if (!ViewData.ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var password = Provider.ResetPassword(model.Username, model.Answer);
                //todo should we send an email or does ResetPassword send an email?
                //try
                //{
                //    Emailer.SendPasswordReset(username, password);
                //    return RedirectToAction("ResetPasswordSuccess");
                //}
                //catch (SendEmailException ex)
                //{
                //    //todo log the error
                //    ViewData.ModelState.AddModelError("error", model.Answer,
                //                                  "Password was reset but there was an error sending the email.");
                //}
            }
            catch (MembershipPasswordException)
            {
                ViewData.ModelState.AddModelError("answer",
                                             "You entered the wrong answer.");
            }
            catch (Exception ex)
            {
                //todo log the error
                ViewData.ModelState.AddModelError("error",
                                                  "There was an error resetting the password.");
            }
            return View(model);
        }
    }
}