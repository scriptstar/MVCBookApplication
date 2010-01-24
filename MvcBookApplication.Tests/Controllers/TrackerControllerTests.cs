using System;
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
    public class TrackerControllerTests
    {
        private const string CONTENT_PATH =
            @"L:\projects\MvcBookApplication\MvcBookApplication\Content\";

        [Test]
        public void dynamicimage_should_return_image_when_no_parameters_defined()
        {
            var con = new TrackerController();
            con.SetFakeControllerContext();
            MyMocks.Server.Expect(s => s.MapPath("~/content/tracker.jpg"))
                .Returns(CONTENT_PATH + "tracker.jpg");

            var result = con.DynamicImage(null);

            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(typeof(FileContentResult), result,
                                    "Wrong type returned");
            var fileContent = result as FileContentResult;
            Assert.AreEqual("tracker.jpg", fileContent.FileDownloadName,
                            "Wrong file name");
            Assert.AreEqual("image/jpeg", fileContent.ContentType,
                            "Wrong content type");
            MyMocks.Server.VerifyAll();
        }

        [Test]
        public void dynamicimage_should_record_access_of_image()
        {
            var messageAudit = new MessageAudit
                                   {
                                       Action = "View",
                                       CreatedOn = DateTime.Now,
                                       Email = "test@test.com",
                                       MessageId = 2
                                   };
            var mockService = new Mock<IMessageAuditService>();
            mockService.Expect(s => s.Add(messageAudit)).Returns(1);

            var con = new TrackerController(mockService.Object);
            con.SetFakeControllerContext();
            MyMocks.Server.Expect(s => s.MapPath("~/content/tracker.jpg"))
                .Returns(CONTENT_PATH + "tracker.jpg");

            var result = con.DynamicImage(messageAudit);
            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(typeof(FileContentResult), result,
                                    "Wrong type returned");
            var fileContent = result as FileContentResult;
            Assert.AreEqual("tracker.jpg", fileContent.FileDownloadName,
                            "Wrong file name");
            Assert.AreEqual("image/jpeg", fileContent.ContentType,
                            "Wrong content type");
            MyMocks.Server.VerifyAll();
            mockService.VerifyAll();
        }

        [Test]
        public void dynamicimage_should_record_access_only_if_email_is_present()
        {
            var messageAudit = new MessageAudit
                                  {
                                      Action = "View",
                                      CreatedOn = DateTime.Now,
                                      Email = null,
                                      MessageId = 2
                                  };

            var con = new TrackerController(null);
            con.SetFakeControllerContext();
            MyMocks.Server.Expect(s => s.MapPath("~/content/tracker.jpg"))
                .Returns(CONTENT_PATH + "tracker.jpg");

            var result = con.DynamicImage(messageAudit);

            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(typeof(FileContentResult), result,
                                    "Wrong type returned");
            var fileContent = result as FileContentResult;
            Assert.AreEqual("tracker.jpg", fileContent.FileDownloadName,
                            "Wrong file name");
            Assert.AreEqual("image/jpeg", fileContent.ContentType,
                            "Wrong content type");
            MyMocks.Server.VerifyAll();
        }

        [Test]
        public void dynamicimage_should_record_access_only_if_message_id_is_present()
        {
            var messageAudit = new MessageAudit
            {
                Action = "View",
                CreatedOn = DateTime.Now,
                Email = "test@test.com"
            };

            var con = new TrackerController(null);
            con.SetFakeControllerContext();
            MyMocks.Server.Expect(s => s.MapPath("~/content/tracker.jpg"))
                .Returns(CONTENT_PATH + "tracker.jpg");

            var result = con.DynamicImage(messageAudit);

            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(typeof(FileContentResult), result,
                                    "Wrong type returned");
            var fileContent = result as FileContentResult;
            Assert.AreEqual("tracker.jpg", fileContent.FileDownloadName,
                            "Wrong file name");
            Assert.AreEqual("image/jpeg", fileContent.ContentType,
                            "Wrong content type");
            MyMocks.Server.VerifyAll();
        }

        [Test]
        public void dynamicimage_should_set_action_view()
        {
            var messageAudit = new MessageAudit
            {
                CreatedOn = DateTime.Now,
                Email = "test@test.com",
                MessageId = 2
            };
            var mockService = new Mock<IMessageAuditService>();
            mockService.Expect(s => s.Add(messageAudit)).Returns(1);

            var con = new TrackerController(mockService.Object);
            con.SetFakeControllerContext();
            MyMocks.Server.Expect(s => s.MapPath("~/content/tracker.jpg"))
                .Returns(CONTENT_PATH + "tracker.jpg");

            var result = con.DynamicImage(messageAudit);

            Assert.AreEqual("View", messageAudit.Action);
            MyMocks.Server.VerifyAll();
            mockService.VerifyAll();
        }

        [Test]
        public void link_should_redirect_to_url()
        {
            var messageAudit = new MessageAudit
            {
                Url = "http://test.com"
            };

            var con = new TrackerController();
            con.SetFakeControllerContext();

            var result = con.Link(messageAudit);
            Assert.IsNotNull(result, "Result is null");
            Assert.IsInstanceOfType(typeof(RedirectResult), result,
                                    "Wrong type returned");
            var redirectResult = result as RedirectResult;
            Assert.AreEqual(messageAudit.Url, redirectResult.Url);
        }

        [Test]
        public void link_should_record_redirect()
        {
            var messageAudit = new MessageAudit
            {
                Url = "http://test.com",
                Email = "test@test.com",
                MessageId = 2
            };
            var mockService = new Mock<IMessageAuditService>();
            mockService.Expect(s => s.Add(messageAudit)).Returns(1);

            var con = new TrackerController(mockService.Object);

            var result = con.Link(messageAudit);

            var redirectResult = result as RedirectResult;
            Assert.AreEqual(messageAudit.Url, redirectResult.Url);
            mockService.VerifyAll();
        }

        [Test]
        public void link_should_record_redirect_only_if_messageid_is_present()
        {
            var messageAudit = new MessageAudit
            {
                Url = "http://test.com",
                Email = "test@test.com"
            };

            var con = new TrackerController();

            var result = con.Link(messageAudit);

            var redirectResult = result as RedirectResult;
            Assert.AreEqual(messageAudit.Url, redirectResult.Url);
        }

        [Test]
        public void link_should_redirect_to_homepage_if_url_is_missing()
        {
            var messageAudit = new MessageAudit
                                   {
                                       Email = "test@test.com",
                                       MessageId = 2
                                   };

            var con = new TrackerController();

            var result = con.Link(messageAudit);
            Assert.IsInstanceOfType(typeof(RedirectToRouteResult), result,
                                    "Wrong result type");
            result.AssertRedirectToRouteResult("index", "home");
        }

        [Test]
        public void link_should_set_action_to_click()
        {
            var messageAudit = new MessageAudit
            {
                Url = "http://test.com",
                Email = "test@test.com",
                MessageId = 2
            };
            var mockService = new Mock<IMessageAuditService>();
            mockService.Expect(s => s.Add(messageAudit)).Returns(1);

            var con = new TrackerController(mockService.Object);

            var result = con.Link(messageAudit);

            var redirectResult = result as RedirectResult;
            Assert.AreEqual(messageAudit.Url, redirectResult.Url);
            Assert.AreEqual("Click", messageAudit.Action);
            mockService.VerifyAll();
        }

    }
}