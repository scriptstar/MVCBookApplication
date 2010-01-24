using MbUnit.Framework;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Tests
{
    internal static class ModelAssertions
    {
        public static void AssertModel(this RegisterModel model,
                                       string username, string email,
                                       string question, string answer,
                                       string password)
        {
            Assert.AreEqual(username, model.Username);
            Assert.AreEqual(email, model.Email);
            Assert.AreEqual(password, model.Password);
            Assert.AreEqual(question, model.Question);
            Assert.AreEqual(answer, model.Answer);
        }


        public static void AssertModel(this LoginModel model,
                                       LoginModel source)
        {
            Assert.AreEqual(source.Username, model.Username);
            Assert.AreEqual(source.Password, model.Password);
            Assert.AreEqual(source.RememberMe, model.RememberMe);
        }

        public static void AssertModel(this ResetPasswordModel model,
                                       ResetPasswordModel source)
        {
            Assert.AreEqual(source.Username, model.Username);
            Assert.AreEqual(source.Question, model.Question);
            Assert.AreEqual(source.Answer, model.Answer);
        }
    }
}