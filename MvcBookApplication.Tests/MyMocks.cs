using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Moq;
using MvcBookApplication.Controllers;

namespace MvcBookApplication.Tests
{
    internal class Mocks
    {
        private Mock<IFormsAuthentication> formsAuthentication;
        private  Mock<MembershipProvider> membershipProvider;
        private Mock<MembershipUser> membershipUser;

        public Mock<IFormsAuthentication> FormsAuthentication
        {
            get
            {
                formsAuthentication = formsAuthentication ??
                                      new Mock<IFormsAuthentication>();
                return formsAuthentication;
            }
        }

        public  Mock<MembershipProvider> MembershipProvider
        {
            get
            {
                membershipProvider = membershipProvider ??
                                     new Mock<MembershipProvider>();
                return membershipProvider;
            }
        }

        public Mock<MembershipUser> MembershipUser
        {
            get
            {
                membershipUser = membershipUser ?? new Mock<MembershipUser>();
                return membershipUser;
            }
        }
    }
    internal class MyMocks
    {

        #region Fields (18)

        //private static Mock<ControllerContext> controllerContext;

        private static Mock<IFormsAuthentication> formsAuthentication
            = new Mock<IFormsAuthentication>();

        private static Mock<MembershipProvider> membershipProvider
            = new Mock<MembershipProvider>();

        private static Mock<MembershipUser> membershipUser
            = new Mock<MembershipUser>();

        private static readonly Mock<HttpContextBase> httpContext
            = new Mock<HttpContextBase>();

        private static readonly Mock<IIdentity> identity
            = new Mock<IIdentity>();

        private static readonly Mock<HttpRequestBase> request
            = new Mock<HttpRequestBase>();

        private static readonly Mock<HttpResponseBase> response
            = new Mock<HttpResponseBase>();

        private static Mock<RouteData> routeData;

        private static readonly Mock<HttpServerUtilityBase> server
            = new Mock<HttpServerUtilityBase>();

        private static readonly Mock<HttpSessionStateBase> session
            = new Mock<HttpSessionStateBase>();

        private static readonly Mock<IPrincipal> user
            = new Mock<IPrincipal>();

        #endregion Fields

        #region Properties (18)

        //public static Mock<ControllerContext> ControllerContext
        //{
        //    get
        //    {
        //        controllerContext = controllerContext ?? new Mock<ControllerContext>();
        //        return controllerContext;
        //    }
        //    set { controllerContext = value; }
        //}

        public Mock<IFormsAuthentication> FormsAuthentication
        {
            get
            {
                return formsAuthentication;
            }
        }

        public static Mock<MembershipProvider> MembershipProvider
        {
            get
            {
                return membershipProvider;
            }
        }

        //public static Mock<MembershipUser> MembershipUser
        //{
        //    get
        //    {
        //        membershipUser = membershipUser ?? new Mock<MembershipUser>();
        //        return membershipUser;
        //    }
        //}

        public static Mock<HttpContextBase> HttpContext
        {
            get { return httpContext; }
        }

        public static Mock<IIdentity> Identity
        {
            get { return identity; }
        }



        public static Mock<HttpRequestBase> Request
        {
            get { return request; }
        }

        public static Mock<HttpResponseBase> Response
        {
            get { return response; }
        }

        public static Mock<RouteData> RouteData
        {
            get
            {
                routeData = routeData ?? new Mock<RouteData>();
                return routeData;
            }
        }

        public static Mock<HttpServerUtilityBase> Server
        {
            get { return server; }
        }

        public static Mock<HttpSessionStateBase> Session
        {
            get { return session; }
        }

        public static Mock<IPrincipal> User
        {
            get { return user; }
        }

        public static Mock<MembershipUser> MembershipUser
        {
            get { return membershipUser; }
        }

        #endregion Properties
    }


}