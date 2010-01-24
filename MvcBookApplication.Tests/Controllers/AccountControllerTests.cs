using System;
using System.Web.Mvc;
using System.Web.Security;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    internal class AccountControllerTests
    {
        private const string username = "test";
        private const string email = "test@test.com";
        private const string question = "what is your favorite movie?";
        private const string answer = "Godfather";
        private const string password = "P@ssword1";
        private MembershipCreateStatus createstatus;

        private AccountController controller;
        private Mocks mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new Mocks();
            controller = new AccountController(mocks.FormsAuthentication.Object,
                                       mocks.MembershipProvider.Object);
        }


        [Test]
        public void Logout_Can_User_Logout()
        {
            mocks.FormsAuthentication.Expect(f => f.SignOut());

            var result = controller.Logout();
            result.AssertRedirectToRouteResult("Index", "Home");
            mocks.MembershipProvider.VerifyAll();
        }

        #region Login Tests

        [Test]
        public void Login_Should_Return_View_For_Get_Requests()
        {
            var result = controller.Login();
            result.AssertViewResult(controller, "Login");
        }

        [Test]
        public void Login_Should_Return_Error_If_Username_Is_Missing()
        {
            var inModel = new LoginModel
                              {
                                  Password = password,
                                  RememberMe = true,
                                  Username = string.Empty
                              };
            var result = controller.Login(inModel);
            AssertLoginViewResultOnError(result,
                                         "username", "Username is required",
                                         inModel);
        }


        [Test]
        public void Login_Should_Return_Error_For_Wrong_Credentials()
        {
            var aUsername = "blah_blah_1234_432";
            var aPassword = "wrong_password!!!";

            mocks.MembershipProvider
                .Expect(p => p.ValidateUser(aUsername, aPassword))
                .Returns(false);
            var inModel = new LoginModel
                              {
                                  Password = aPassword,
                                  RememberMe = true,
                                  Username = aUsername
                              };
            var result = controller.Login(inModel);
            AssertLoginViewResultOnError(result,
                                         "provider",
                                         "The username or password provided is incorrect.",
                                         inModel);
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void Login_Redirects_To_Index_Home_On_Success()
        {
            mocks.MembershipProvider
                .Expect(p => p.ValidateUser(username, password))
                .Returns(true);
            var inModel = new LoginModel
                              {
                                  Password = password,
                                  RememberMe = true,
                                  Username = username
                              };
            var result = controller.Login(inModel);
            result.AssertRedirectToRouteResult("Index", "Home");
            mocks.MembershipProvider.VerifyAll();
        }

        //[Test]
        //public void Login_User_Can_Login()
        //{
        //    var controller = TestControllerFactory.GetControllerWithFakeContext<AccountController>("POST");

        //    for (var i = 0; i < 20; i++)
        //    {
        //        var aUsername = "user" + i;
        //        var aPassword = "password" + i;
        //        var aUserGuid = Guid.NewGuid();
        //        var aEmail = string.Format("{0}@dotnetfactory.com", aUsername);
        //        var rememberme = true;

        //        controller.SetMockMembershipUser(aUsername, aUserGuid, aEmail);
        //        controller.SetMockCurrentUser(mocks.MembershipUser.Object);

        //        mocks.FormsAuthentication
        //            .Expect(f => f.SetAuthCookie(aUsername, rememberme));
        //        mocks.MembershipProvider
        //            .Expect(p => p.ValidateUser(aUsername, aPassword))
        //            .Returns(true);
        //        mocks.MembershipProvider
        //            .Expect(p => p.GetUser(aUsername, true))
        //            .Returns(mocks.MembershipUser.Object);

        //        //todo we need to mock the call to CreateUserFromMembershipUser
        //        //mocks.AccountService
        //        //    .Expect(a => a.CreateUserFromMembershipUser(mocks.MembershipUser.Object))
        //        //    .Returns(mocks.CurrentUser.Object);

        //        //mocks.Session
        //        //    .Expect(s => s.Add("currentuser", mocks.CurrentUser.Object))
        //        //    ;
        //        //mocks.Session.Expect(s => s["currentuser"])
        //        //    .Callback(sc => Assert.AreEqual(mocks.CurrentUser.Object, sc));
        //        //mocks.Session.ExpectSet(s => s["currentuser"], mocks.CurrentUser.Object);


        //        var result = controller.Login(aUsername, aPassword, rememberme);
        //        MvcAssert.AssertRedirectToRouteResult(result, "Index", "Home");

        //        mocks.FormsAuthentication.VerifyAll();
        //        mocks.MembershipProvider.VerifyAll();
        //        mocks.MembershipUser.VerifyAll();
        //        //mocks.Session.VerifyAll();
        //        Assert.Inconclusive("Session is not verified");
        //        //todo figure out a way to mock and verify that the session is set
        //    }
        //}

        private void AssertLoginViewResultOnError(ActionResult result,
                                                  string errorKey,
                                                  string errorMessage,
                                                  LoginModel model)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof (ViewResult), result);
            controller.ViewData.ModelState.AssertErrorMessage(errorKey, errorMessage);
            Assert.IsInstanceOfType(typeof (LoginModel),
                                    ((ViewResult) result).ViewData.Model);
            var outModel = (((ViewResult) result).ViewData.Model as LoginModel);
            outModel.AssertModel(model);
            Assert.AreEqual("Login", controller.ViewData["Title"], "Page title is wrong");
        }

        #endregion

        #region Register Tests

        [Test]
        public void Register_Can_Get_To_Register_View()
        {
            var result = controller.Register();
            result.AssertViewResult(controller, "Register");
        }

        [Test]
        public void Register_Redirects_To_Home_Index_On_Success()
        {
            //set expectations
            mocks.MembershipProvider
                .Expect(p => p.CreateUser(username, password,
                                          email, question, answer, true,
                                          null, out createstatus))
                .Returns(new Mock<MembershipUser>().Object);
            //run tests
            var model = new RegisterModel
                            {
                                Username = username,
                                Email = email,
                                Question = question,
                                Answer = answer,
                                Password = password
                            };
            var result = controller.Register(model);
            //assert result
            result.AssertRedirectToRouteResult("Index", "home");
        }


        [Test]
        public void Register_Should_Return_Error_If_Username_Is_Missing()
        {
            var model = new RegisterModel
            {
                Username = string.Empty,
                Email = email,
                Question = question,
                Answer = answer,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "username",
                                            "Username is required",
                                            string.Empty, email,
                                            question, answer, password);
        }


        [Test]
        public void Register_Should_Return_Error_If_Email_Is_Missing()
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = string.Empty,
                Question = question,
                Answer = answer,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "email",
                                            "Email is required",
                                            username, string.Empty,
                                            question, answer, password);
        }

        [Test]
        public void Register_Should_Return_Error_If_Password_Is_Missing()
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = email,
                Question = question,
                Answer = answer,
                Password = string.Empty
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "password",
                                            "Password is required",
                                            username, email, question,
                                            answer, string.Empty);
        }

        [Test]
        public void Register_Should_Return_Error_If_Question_Is_Missing()
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = email,
                Question = string.Empty,
                Answer = answer,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "question",
                                            "Question is required",
                                            username, email, string.Empty,
                                            answer, password);
        }

        [Test]
        public void Register_Should_Return_Error_If_Answer_Is_Missing()
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = email,
                Question = question,
                Answer = string.Empty,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "answer",
                                            "Answer is required",
                                            username, email, question,
                                            string.Empty, password);
        }

        [Test]
        public void Register_Should_Return_Error_If_Email_Is_Invalid()
        {
            var invalidEmail = "bad @ email .#.com";
            var model = new RegisterModel
            {
                Username = username,
                Email = invalidEmail,
                Question = question,
                Answer = answer,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "email",
                                            "Email is invalid",
                                            username, invalidEmail, question,
                                            answer, password);
        }

        [Test]
        public void Register_Should_Return_Error_If_Username_Is_Invalid()
        {
            var invalidUsername = "123";
            var model = new RegisterModel
            {
                Username = invalidUsername,
                Email = email,
                Question = question,
                Answer = answer,
                Password = password
            };
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "username",
                                            "Username is invalid",
                                            invalidUsername, email, question,
                                            answer, password);
        }

        [Test]
        [Row(MembershipCreateStatus.DuplicateUserName,
            "Username already exists. " +
            "Please enter a different user name.",
            "Failed duplicate user name test")]
        [Row(MembershipCreateStatus.DuplicateEmail,
            "A username for that e-mail address already exists. " +
            "Please enter a different e-mail address.",
            "Failed duplicate email test")]
        [Row(MembershipCreateStatus.InvalidPassword,
            "The password provided is invalid. " +
            "Please enter a valid password value.",
            "Failed invalid password test")]
        [Row(MembershipCreateStatus.InvalidEmail,
            "The e-mail address provided is invalid. " +
            "Please check the value and try again.",
            "Failed invalid email test"
            )]
        [Row(MembershipCreateStatus.InvalidAnswer,
            "The password retrieval answer provided is invalid. " +
            "Please check the value and try again.",
            "Failed invalid answer test")]
        [Row(MembershipCreateStatus.InvalidQuestion,
            "The password retrieval question provided is invalid. " +
            "Please check the value and try again.",
            "Failed invalid question test")]
        [Row(MembershipCreateStatus.InvalidUserName,
            "The user name provided is invalid. " +
            "Please check the value and try again.",
            "Failed invalid username test")]
        [Row(MembershipCreateStatus.ProviderError,
            "The authentication provider returned an error. " +
            "Please verify your entry and try again. " +
            "If the problem persists, please contact your system administrator."
            , "Failed provider error test")]
        [Row(MembershipCreateStatus.UserRejected,
            "The user creation request has been canceled. " +
            "Please verify your entry and try again. " +
            "If the problem persists, please contact your system administrator."
            , "Failed user rejected test")]
        [Row(MembershipCreateStatus.DuplicateProviderUserKey,
            "An unknown error occurred. Please verify your entry and try again. " +
            "If the problem persists, please contact your system administrator."
            , "Failed default test")]
        public void Register_Should_Fail_If_CreateUser_Fails(MembershipCreateStatus status, string errorMessage)
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = email,
                Question = question,
                Answer = answer,
                Password = password
            };
            //create fake membership provider & mocks
            var fakeProvider = new FakeMembershipProvider();
            //tell the fake provider what status to return when CreateUser is called
            fakeProvider.SetFakeStatus(status);
            //run tests
            controller = new AccountController(mocks.FormsAuthentication.Object, fakeProvider);
            var result = controller.Register(model);
            AssertRegisterViewResultOnError(controller, result, "provider",
                                            errorMessage,
                                            username, email, question,
                                            answer, password);
        }


        [Test]
        public void Register_Should_Call_SetAuthCookie_On_Success()
        {
            var model = new RegisterModel
            {
                Username = username,
                Email = email,
                Question = question,
                Answer = answer,
                Password = password
            };
            //create mocks and set expectations
            mocks.MembershipProvider
                .Expect(p => p.CreateUser(username, password,
                                          email, question, answer, true,
                                          null, out createstatus))
                .Returns(new Mock<MembershipUser>().Object);
            mocks.FormsAuthentication
                .Expect(a => a.SetAuthCookie(username, false));

            //run tests
            var result = controller.Register(model);
            //assert result
            Assert.IsNotNull(result);
            result.AssertRedirectToRouteResult("Index", "home");
            //verify mocks expectations have been met
            mocks.MembershipProvider.VerifyAll();
            mocks.FormsAuthentication.VerifyAll();
        }

        #region Private methods

        private static void AssertRegisterViewResultOnError(AccountController ac,
                                                            ActionResult result,
                                                            string errorKey,
                                                            string errorMessage,
                                                            string username,
                                                            string email,
                                                            string question,
                                                            string answer,
                                                            string password)
        {
            //todo refactor register method to take in the registermodel
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof (ViewResult), result);
            ac.ViewData.ModelState.AssertErrorMessage(errorKey, errorMessage);
            Assert.IsInstanceOfType(typeof (RegisterModel),
                                    ((ViewResult) result).ViewData.Model);
            var model = (((ViewResult) result).ViewData.Model as RegisterModel);
            model.AssertModel(username, email, question, answer, password);
        }

        #endregion

        #endregion

        #region ResetPassword Tests

        [Test]
        public void ResetPassword_User_Can_View_Reset_Password_Form()
        {
            var result = controller.ResetPassword();
            result.AssertViewResult(controller, "Reset Password");
        }

        [Test]
        public void ResetPassword_Should_Return_Error_If_Username_Is_Missing()
        {
            var result = controller.ResetPassword(string.Empty);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof (ViewResult), result);
            controller.ViewData.ModelState.AssertErrorMessage("username",
                                                      "Username or email are required");
            result.AssertViewResult(controller, "Reset Password");
        }

        [Test]
        public void ResetPassword_User_Can_Go_To_Question()
        {
            mocks.MembershipProvider
                .Expect(p => p.GetUser(username, false))
                .Returns(mocks.MembershipUser.Object);
            mocks.MembershipUser
                .Expect(u => u.PasswordQuestion)
                .Returns(question);

            var result = controller.ResetPassword(username);
            result.AssertRedirectToRouteResult("ResetPasswordQuestion");
            result.AssertRedirectToRouteResultValues(new ResetPasswordModel
                                                         {
                                                             Username = username,
                                                             Question = question
                                                         });
            mocks.MembershipUser.VerifyAll();
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void ResetPassword_Can_Reset_With_Email()
        {
            var usernameFromProvider = "user2";
            //this should return null meaning username was not found
            //which is true since we are sending an email
            mocks.MembershipProvider
                .Expect(p => p.GetUser(email, false))
                .Returns((MembershipUser) null);
            mocks.MembershipProvider
                .Expect(p => p.GetUserNameByEmail(email))
                .Returns(usernameFromProvider);
            mocks.MembershipProvider
                .Expect(p => p.GetUser(usernameFromProvider, false))
                .Returns(mocks.MembershipUser.Object);
            mocks.MembershipUser
                .Expect(u => u.PasswordQuestion)
                .Returns(question);

            var result = controller.ResetPassword(email);
            result.AssertRedirectToRouteResult("ResetPasswordQuestion");
            result.AssertRedirectToRouteResultValues(new ResetPasswordModel
                                                         {
                                                             Username = usernameFromProvider,
                                                             Question = question
                                                         });

            mocks.MembershipUser.VerifyAll();
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void ResetPassword_Should_Return_Error_For_Non_Existing_User()
        {
            var badusernameoremail = "badUsernameOrEmail";
            //return null i.e. username was not found
            mocks.MembershipProvider
                .Expect(p => p.GetUser(badusernameoremail, false))
                .Returns((MembershipUser) null);
            //return null i.e. email was not fount
            mocks.MembershipProvider
                .Expect(p => p.GetUserNameByEmail(badusernameoremail))
                .Returns((string) null);

            var result = controller.ResetPassword(badusernameoremail);
            result.AssertViewResult(controller, "Reset Password");
            controller.ViewData.ModelState.AssertErrorMessage("username",
                                                      "This username/email doesn't exist.");
            controller.ViewData.AssertItem("usernameoremail", badusernameoremail);
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void ResetPasswordQuestion_Can_View_Question_Form()
        {
            var result = controller.ResetPasswordQuestion(new ResetPasswordModel
                                                      {
                                                          Question = question,
                                                          Username = username
                                                      });
            result.AssertViewResult(controller, "Reset Password");
        }

        [Test]
        public void ResetPasswordQuestion_Return_Error_If_Username_Does_Not_Exist()
        {
            var result = controller.ResetPasswordQuestion(new ResetPasswordModel
                                                      {
                                                          Question = question
                                                      });
            result.AssertRedirectToRouteResult("ResetPassword");
        }

        [Test]
        public void ResetPasswordQuestion_Return_Error_If_Question_Does_Not_Exist()
        {
            var result = controller.ResetPasswordQuestion(new ResetPasswordModel
                                                      {
                                                          Username = username
                                                      });
            result.AssertRedirectToRouteResult("ResetPassword");
        }

        [Test]
        public void ResetPasswordQuestion_Returns_Error_If_Answer_Is_Empty()
        {
            var inModel = new ResetPasswordModel
                              {
                                  Question = question,
                                  Username = username
                              };
            var result = controller.ResetPasswordQuestionSubmit(inModel);
            AssertResetPasswordOnError(result, inModel, "answer", "Answer is required");
        }

        [Test]
        public void ResetPasswordQuestion_Returns_Error_If_Answer_Is_Wrong()
        {
            var wronganswer = "wrong answer";
            mocks.MembershipProvider
                .Expect(p => p.ResetPassword(username, wronganswer))
                .Throws<MembershipPasswordException>();

            var inModel = new ResetPasswordModel
                              {
                                  Question = question,
                                  Username = username,
                                  Answer = wronganswer
                              };
            var result = controller.ResetPasswordQuestionSubmit(inModel);
            AssertResetPasswordOnError(result, inModel, "answer", "You entered the wrong answer.");
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void ResetPasswordQuestion_Returns_Error_If_Provider_Throws_Exception()
        {
            var wronganswer = "wrong answer";
            mocks.MembershipProvider
                .Expect(p => p.ResetPassword(username, wronganswer))
                .Throws<Exception>();

            var inModel = new ResetPasswordModel
                              {
                                  Question = question,
                                  Username = username,
                                  Answer = wronganswer
                              };
            var result = controller.ResetPasswordQuestionSubmit(inModel);
            AssertResetPasswordOnError(result, inModel, "error",
                                       "There was an error resetting the password.");
            mocks.MembershipProvider.VerifyAll();
        }

        [Test]
        public void ResetPasswordQuestion_Should_Send_Email_On_Success()
        {
            //var newpassword = "newpassword";
            //mocks.MembershipProvider.Expect(p => p.ResetPassword(username, answer)).Returns(newpassword);
            //MyMocks.EmailService.Expect(m => m.SendPasswordReset(username, newpassword));

            //var controller = TestControllerFactory.GetControllerWithFakeContext<AccountController>("POST");

            //var result = controller.ResetPasswordQuestion(username, question, answer);
            //MvcAssert.AssertRedirectToRouteResult(result, "ResetPasswordSuccess");

            //MyMocks.MembershipProvider.VerifyAll();
            //MyMocks.EmailService.VerifyAll();
            Assert.Inconclusive();
        }

        [Test]
        public void ResetPasswordQuestion_Should_Fail_If_Email_Send_Fails()
        {
            //var newpassword = "newpassword";
            //MyMocks.MembershipProvider.Expect(p => p.ResetPassword(username, answer)).Returns(newpassword);
            //MyMocks.EmailService.Expect(m => m.SendPasswordReset(username, newpassword)).Throws<SendEmailException>();

            //var result = controller.ResetPasswordQuestion(username, question, answer);
            //AssertFailedResetPasswordQuestion(controller, username, question,
            //                                  answer, "Password was reset but there was an error sending the email.", result);

            //MyMocks.MembershipProvider.VerifyAll();
            //MyMocks.EmailService.VerifyAll();
            Assert.Inconclusive();
        }

        private void AssertResetPasswordOnError(ActionResult result,
                                                ResetPasswordModel model, string errorkey, string errormessage)
        {
            controller.ViewData.ModelState.AssertErrorMessage(errorkey, errormessage);
            result.AssertViewResult(controller, "Reset Password");
            model.AssertModel((ResetPasswordModel) controller.ViewData.Model);
        }

        #endregion
    }
}