using System.Reflection;
using System.Web.Mvc;
using MbUnit.Framework;

namespace MvcBookApplication.Tests
{
    internal static class ResultAssertions
    {
        public static void AssertRedirectToRouteResult(this ActionResult result,
                                                       string actionName,
                                                       string controllerName)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(RedirectToRouteResult), result);
            var routeResult = (RedirectToRouteResult)result;
            Assert.GreaterThanOrEqualTo(routeResult.RouteValues.Count, 2);
            
            Assert.AreEqual(actionName.ToLower(),
                            routeResult.RouteValues["action"].ToString().ToLower());
            Assert.AreEqual(controllerName.ToLower(),
                            routeResult.RouteValues["controller"].ToString().ToLower());

        }

        public static void AssertRedirectToRouteResult(this ActionResult result,
                                               string actionName)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(RedirectToRouteResult), result);
            var routeResult = (RedirectToRouteResult)result;
            Assert.GreaterThanOrEqualTo(routeResult.RouteValues.Count, 1);
            Assert.AreEqual(actionName.ToLower(),
                            routeResult.RouteValues["action"].ToString().ToLower());
        }

        public static void AssertRedirectToRouteResultValues(this ActionResult result,
                                              object model)
        {
            var routeResult = (RedirectToRouteResult)result;
            var propertyInfos = model.GetType().GetProperties();
            foreach (var prop in propertyInfos)
            {
                Assert.AreEqual(prop.GetValue(model, null),
                    routeResult.RouteValues[prop.Name]);
            }
        }

        public static void AssertViewResult(this ActionResult result,
                                            Controller controller,
                                            string title)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            Assert.IsEmpty(((ViewResult)result).ViewName);
            Assert.AreEqual(title, controller.ViewData["Title"], "Page title is wrong");
        }

        public static void AssertViewResult(this ActionResult result,
                                         Controller controller,
                                         string title, string viewname)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            Assert.AreEqual(viewname, ((ViewResult)result).ViewName);
            Assert.AreEqual(title, controller.ViewData["Title"], "Page title is wrong");
        }
    }
}