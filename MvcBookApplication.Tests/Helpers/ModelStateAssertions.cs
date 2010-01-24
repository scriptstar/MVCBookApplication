using System.Web.Mvc;
using MbUnit.Framework;

namespace MvcBookApplication.Tests
{
    public static class ModelStateAssertions
    {
        public static void AssertErrorMessage(this ModelStateDictionary modelState,
                                              string key, string errormessage)
        {
            Assert.IsTrue(modelState.ContainsKey(key));
            Assert.AreEqual(errormessage,
                            modelState[key].Errors[0].ErrorMessage);
            Assert.IsFalse(modelState.IsValid);
        }
    }
}