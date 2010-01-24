using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace MvcBookApplication.Tests
{
    [TestFixture]
    class AppHelperTest
    {

        [Test]
        [Row("test@test_test.com")]
        [Row("sdfdf dsfsdf")]
        [Row("sdfdf@.com")]
        [Row("sdfdf@dfdfdf")]
        [Row("sdfdf@fdfd. com")]
        [Row("s&#dfdf@fdfd.com")]
        [Row("sd@fdf@fdfd.com")]
        public void IsValidEmail_Invalid_Emails_Should_ReturnFalse(string invalidEmail)
        {
            Assert.IsFalse(MvcBookApplication.Services.AppHelper.IsValidEmail(invalidEmail),
                           "Email validation failed for " + invalidEmail);
        }

        [Test]
        [Row("test@test.com")]
        [Row("123@test.com")]
        [Row("first_last@test.com")]
        [Row("international@test.com.eg")]
        [Row("someorg@test.org")]
        [Row("somenet@test.net")]
        public void IsValidEmail_Valid_Emails_Should_Return_True(string invalidEmail)
        {
            Assert.IsTrue(MvcBookApplication.Services.AppHelper.IsValidEmail(invalidEmail),
                          "Email validation failed for " + invalidEmail);
        }


        #region IsValidUsername Tests
        [Test]
        [Row("user")]
        [Row("user1")]
        [Row("user_1")]
        [Row("user_2_3")]
        public void Valid_Usernames_Should_Return_True(string username)
        {
            Assert.IsTrue(MvcBookApplication.Services.AppHelper.IsValidUsername(username),
                          string.Format("Username validation failed for {0}",
                                        username));
        }

        [Test]
        [Row("")]
        [Row("a")]
        [Row("ab")]
        [Row("abc")]
        public void Short_Username_Should_Return_False(string username)
        {
            Assert.IsFalse(MvcBookApplication.Services.AppHelper.IsValidUsername(username),
                           string.Format("username failed validation for {0}",
                                         username));
        }

        [Test]
        [Row("user!")]
        [Row("user@")]
        [Row("user#")]
        [Row("user$")]
        public void Username_With_Invalid_Characters_Should_Return_False(
            string username)
        {
            Assert.IsFalse(MvcBookApplication.Services.AppHelper.IsValidUsername(username),
                           string.Format("username failed validation for {0}",
                                         username));
        }

        [Test]
        [Row("1user")]
        [Row(" user")]
        [Row("_user")]
        public void Username_Starting_With_Non_Alpha_Should_Return_False(
            string username)
        {
            Assert.IsFalse(MvcBookApplication.Services.AppHelper.IsValidUsername(username),
                           string.Format("username failed validation for {0}",
                                         username));
        }
        #endregion

    }
}