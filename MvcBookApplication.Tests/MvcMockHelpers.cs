using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Moq;
using MvcBookApplication.Data;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;

namespace MvcBookApplication.Tests
{
    internal static class MvcMockHelpers
    {
        private static void SetFakeHttpContext()
        {
            MyMocks.HttpContext.Expect(ctx => ctx.Request).Returns(MyMocks.Request.Object);
            MyMocks.HttpContext.Expect(ctx => ctx.Response).Returns(MyMocks.Response.Object);
            MyMocks.HttpContext.Expect(ctx => ctx.Session).Returns(MyMocks.Session.Object);
            MyMocks.HttpContext.Expect(ctx => ctx.Server).Returns(MyMocks.Server.Object);
            MyMocks.HttpContext.Expect(ctx => ctx.User).Returns(MyMocks.User.Object);
            MyMocks.User.Expect(ctx => ctx.Identity).Returns(MyMocks.Identity.Object);

            var userid = Guid.NewGuid();
            MyMocks.Identity.Expect(i => i.Name).Returns("test");
            MyMocks.MembershipProvider.Expect(m => m.GetUser("test", false))
                .Returns(MyMocks.MembershipUser.Object);
            MyMocks.MembershipUser.Expect(u => u.ProviderUserKey).Returns(userid);
            return;
        }

        public static void SetFakeControllerContext(this Controller controller)
        {
            SetFakeHttpContext();
            var route = new Route("{controller}/{action}/{id}", new MvcRouteHandler());
            MyMocks.RouteData.Object.Route = route;
            MyMocks.RouteData.Object.RouteHandler = new MvcRouteHandler();
            controller.ControllerContext = new ControllerContext(
                new RequestContext(MyMocks.HttpContext.Object,
                                   MyMocks.RouteData.Object),
                controller);
        }


        //public T GetController<T>()
        //{
        //    kernel = GetIoCKernel();
        //    var contactController = (T) kernel
        //                                    .Get(typeof (ContactController));
        //    contactController.ControllerContext = new ControllerContext(
        //        httpContext.Object,
        //        new RouteData(),
        //        controllerbase.Object);
        //    return contactController;
        //}

        //private static Mock<MembershipProvider> MockMembership =
        //    new Mock<MembershipProvider>();

        //private static StandardKernel GetIoCKernel()
        //{
        //    var modules = new IModule[]
        //                      {
        //                          new InlineModule(
        //                              new Action<InlineModule>[]
        //                                  {
        //                                      m => m.Bind<MembershipProvider>()
        //                                               .ToConstant(MockMembership.Object),
        //                                      m => m.Bind<IContactService>()
        //                                               .To<InMemoryContactService>(),
        //                                      m => m.Bind<IContactRepository>()
        //                                               .To<InMemoryContactRepository>()
        //                                               .Using<SingletonBehavior>(),
        //                                      m => m.Bind<IValidationRunner>()
        //                                               .To<ValidationRunner>()
        //                                  })
        //                      };
        //    return new StandardKernel(modules);
        //}
    }
}