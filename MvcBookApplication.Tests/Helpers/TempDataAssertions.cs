using System.Web.Mvc;
using MbUnit.Framework;

namespace MvcBookApplication.Tests
{
    internal static class TempDataAssertions
    {
        public static void AssertItem(this TempDataDictionary TempData, string key, object value)
        {
            Assert.AreEqual(value, TempData[key], "TempData is invalid");
            
        }
    }
}