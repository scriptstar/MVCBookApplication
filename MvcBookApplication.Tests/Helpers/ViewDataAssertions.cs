using System.Web.Mvc;
using MbUnit.Framework;

namespace MvcBookApplication.Tests
{
    internal static class ViewDataAssertions
    {
        public static void AssertItem(this ViewDataDictionary viewdata, string key, object value)
        {
            Assert.AreEqual(value, viewdata[key], "ViewData is invalid");
        }
    }
}