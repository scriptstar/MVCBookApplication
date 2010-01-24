using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class TemplateControllerTests
    {
        [Test]
        public void list_returns_system_templates()
        {
            var mockSer = new Mock<ITemplateService>();
            mockSer.Expect(s => s.Get()).Returns(new List<Template>
                                             {
                                                 new Template(),
                                                 new Template()
                                             });
            var con = new TemplateController(mockSer.Object);
            var result = con.List(null);
            Assert.IsNotNull(result);
            result.AssertViewResult(con, null, "list");
            Assert.IsInstanceOfType(typeof(IList<Template>), con.ViewData.Model);
            Assert.AreEqual(2, (con.ViewData.Model as IList<Template>).Count);
        }

        [Test]
        public void save_saves_user_template()
        {
            var template = new Template();
            var username = "test";
            var mockSer = new Mock<ITemplateService>();
            mockSer.Expect(s => s.Save(username, "name", "content")).Returns(1);
            var con = new TemplateController(mockSer.Object);
            con.SetFakeControllerContext();

            var result = con.Save("name", "content");

            mockSer.VerifyAll();
            Assert.IsInstanceOfType(typeof(JsonResult), result);
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(typeof(JsonData), jsonResult.Data);
            Assert.IsTrue(((JsonData)jsonResult.Data).success);
            Assert.AreEqual(1, ((JsonData)jsonResult.Data).id);
        }

        [Test]
        public void list_returns_user_templates()
        {
            var username = "test";
            var mockSer = new Mock<ITemplateService>();
            mockSer.Expect(s => s.Get(username)).Returns(new List<Template>
                                     {
                                         new Template(),
                                         new Template(),
                                         new Template()
                                     });
            var con = new TemplateController(mockSer.Object);
            var result = con.List(username);
            Assert.IsNotNull(result);
            result.AssertViewResult(con, null, "list");
            Assert.IsInstanceOfType(typeof(IList<Template>), con.ViewData.Model);
            Assert.AreEqual(3, (con.ViewData.Model as IList<Template>).Count);
        }
    }
}