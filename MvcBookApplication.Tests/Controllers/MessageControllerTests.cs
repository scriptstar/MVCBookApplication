using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data;
using MvcBookApplication.Data.InMemory;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;
using Ninject.Core.Behavior;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class MessageControllerTests
    {
        private string Username = "testusername";
        private Guid UserId = Guid.NewGuid();
        static StandardKernel kernel;

        private Mock<MembershipProvider> MockMembership =
            new Mock<MembershipProvider>();
        private Mock<MembershipUser> MockMembershipUser =
            new Mock<MembershipUser>();
        private Mock<IPrincipal> user =
            new Mock<IPrincipal>();
        private Mock<IIdentity> identity =
            new Mock<IIdentity>();
        private Mock<HttpContextBase> httpContext =
            new Mock<HttpContextBase>();
        private Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();

        private Mock<ControllerBase> controllerbase =
            new Mock<ControllerBase>();

        private MessageController controller;
        private Message model;

        [SetUp]
        public void SetUp()
        {
            MockMembership = new Mock<MembershipProvider>();
            MockMembershipUser = new Mock<MembershipUser>();
            user = new Mock<IPrincipal>();
            identity = new Mock<IIdentity>();
            httpContext = new Mock<HttpContextBase>();
            request = new Mock<HttpRequestBase>();
            controllerbase = new Mock<ControllerBase>();

            SetupMocks(Username, UserId);

            controller = GetController();

            model = new Message()
                        {
                            Subject = "My newsletter subject",
                            Name = "October newsletter",
                            Text = "Hello subscriber",
                            Html = "Hello <b>subscriber</b>"
                        };
        }

        #region Create tests

        [Test]
        public void create_should_return_view()
        {
            var result = controller.Create();
            result.AssertViewResult(controller, "Create New Message");
        }

        [Test]
        public void create_should_set_templates_in_viewdata_on_get_requests()
        {
            var mockSer = new Mock<ITemplateService>();
            mockSer.Expect(s => s.Get()).Returns(new List<Template>
                                     {
                                         new Template(),
                                         new Template()
                                     });

            controller.TemplateService = mockSer.Object;
            controller.Create();

            mockSer.VerifyAll();
            Assert.IsInstanceOfType(typeof(IList<Template>),
                controller.ViewData["templates"]);
            var templates = controller.ViewData["templates"] as IList<Template>;
            Assert.IsNotNull(templates);
            Assert.AreEqual(2, templates.Count);
        }

        [Test]
        public void create_should_set_mytemplates_in_viewdata_on_get_requests()
        {
            var mockSer = new Mock<ITemplateService>();
            mockSer.Expect(s => s.Get(Username)).Returns(new List<Template>
                                     {
                                         new Template(),
                                         new Template(),
                                         new Template()
                                     });

            controller.TemplateService = mockSer.Object;
            controller.Create();

            mockSer.VerifyAll();
            Assert.IsInstanceOfType(typeof(IList<Template>),
                controller.ViewData["mytemplates"]);
            var templates = controller.ViewData["mytemplates"] as IList<Template>;
            Assert.IsNotNull(templates);
            Assert.AreEqual(3, templates.Count);
        }

        [Test]
        public void create_should_return_error_if_name_is_missing()
        {
            model.Name = string.Empty;
            var result = controller.Create(model);
            var errorKey = "Name";
            var errorMessage = "Name is required";
            AssertCreateValidationError(result, errorKey, errorMessage);
        }

        [Test]
        public void create_should_return_error_if_subject_is_missing()
        {
            model.Subject = string.Empty;
            var result = controller.Create(model);
            var errorKey = "Subject";
            var errorMessage = "Subject is required";
            AssertCreateValidationError(result, errorKey, errorMessage);
        }

        [Test]
        public void create_should_return_error_if_text_is_missing()
        {
            model.Text = string.Empty;
            var result = controller.Create(model);
            var errorKey = "Text";
            var errorMessage = "A plain text body is required";
            AssertCreateValidationError(result, errorKey, errorMessage);
        }

        [Test]
        public void create_should_add_message_to_repository()
        {
            //mock the repo
            var mockRepo = new Mock<IMessageRepository>();
            //set expectations
            mockRepo.Expect(r => r.Add(model)).Returns(1);
            var mockValidationRunner = new Mock<IValidationRunner>();
            var service = new InMemoryMessageService(mockRepo.Object,
                                                     mockValidationRunner.Object);
            controller = new MessageController(service, null);
            var result = controller.Create(model);
            mockRepo.VerifyAll();
        }

        [Test]
        public void create_should_add_flash_message_if_save_is_successful()
        {
            var mockService = new Mock<IMessageService>();
            mockService.Expect(s => s.Add(model)).Returns(1);
            controller = new MessageController(mockService.Object, null);
            var result = controller.Create(model);
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Message successfully created"),
                            "Flash message is missing");
            mockService.VerifyAll();
        }

        [Test]
        public void create_should_redirect_to_list_view_after_successful_save()
        {
            var mockService = new Mock<IMessageService>();
            mockService.Expect(s => s.Add(model)).Returns(1);
            controller = new MessageController(mockService.Object, null);
            var result = controller.Create(model);
            result.AssertRedirectToRouteResult("browse");
            mockService.VerifyAll();
        }

        #endregion

        #region Browse Tests

        [Test]
        public void browse_messages_should_retrieve_20_or_less_messages_at_once()
        {
            PopulateRepository();
            var result = controller.Browse(null);
            Assert.IsInstanceOfType(typeof(PagedList<Message>),
                                    controller.ViewData.Model,
                                    "View data is the wrong type");
            Assert.LessThanOrEqualTo(((PagedList<Message>)controller.ViewData.Model).Count,
                                     20, "Page size is wrong");
            result.AssertViewResult(controller, "Browse Messages", "browse");
        }

        [Test]
        public void browse_contacts_should_retrieve_messages_for_loggedin_user_only()
        {
            PopulateRepository();
            var result = controller.Browse(null);
            Assert.AreEqual(25,
                            ((PagedList<Message>)controller.ViewData.Model).TotalItemCount,
                            "Item count is wrong");
        }

        #endregion


        #region Edit tests

        [Test]
        public void edit_message_should_return_view()
        {
            PopulateRepository();
            var result = controller.Edit(1);
            result.AssertViewResult(controller, "Edit Message", "edit");
        }

        [Test]
        public void edit_message_should_get_requested_message()
        {
            PopulateRepository();
            var result = controller.Edit(1);
            Assert.IsInstanceOfType(typeof(Message), controller.ViewData.Model);
            Assert.AreEqual(1, ((Message)controller.ViewData.Model).Id);
            result.AssertViewResult(controller, "Edit Message", "edit");

        }

        [Test]
        public void edit_message_should_return_error_page_if_requested_message_is_not_found()
        {
            try
            {
                PopulateRepository();
                var result = controller.Edit(45789); //non-existing message
                Assert.Fail("Failed to throw exception");
            }
            catch (ArgumentException)
            {
                controller.TempData.AssertItem("error",
                                               "The message you requested could not be found");
            }
        }

        [Test]
        public void edit_message_should_return_error_page_if_user_is_not_owner()
        {
            var id = 2;
            try
            {
                PopulateRepository();
                var result = controller.Edit(id); //owned by another user
                Assert.Fail("Failed to throw exception");
            }
            catch (ArgumentException)
            {
                var repo = (IMessageRepository)kernel.Get(typeof(IMessageRepository));
                Assert.AreEqual(1, repo.Get().Count(c => c.Id == id));
                controller.TempData.AssertItem("error",
                                               "The message you requested could not be found");
            }
        }

        [Test]
        public void edit_message_should_save_changes_to_repository()
        {
            PopulateRepository();
            var repo = (IMessageRepository)kernel.Get(typeof(IMessageRepository));
            var oldMessage = repo.Get().SingleOrDefault(m => m.Id == 1);
            var oldName = oldMessage.Name;
            var newName = "New name";
            var newMessage = new Message
                                 {
                                     Name = newName,
                                     Html = oldMessage.Html,
                                     Id = oldMessage.Id,
                                     Subject = oldMessage.Subject,
                                     Text = oldMessage.Text,
                                     User = oldMessage.User
                                 };
            Assert.AreEqual(oldName, repo.Get().SingleOrDefault(m => m.Id == 1).Name);

            var result = controller.Edit(newMessage);

            Assert.AreEqual(newName, repo.Get().SingleOrDefault(m => m.Id == 1).Name);
        }
        #endregion


        private void PopulateRepository()
        {
            var anotherUsername = Username + 2;
            var anotherUserId = Guid.NewGuid();
            var repo = (IMessageRepository)kernel.Get(typeof(IMessageRepository));
            for (var i = 0; i < 50; i++)
            {
                repo.Add(
                    new Message
                        {
                            Html = "random <b>html</b> " + i,
                            Id = i,
                            Name = "Message " + i,
                            Subject = "Subject " + i,
                            Text = "random text " + i,
                            User = i % 2 == 0
                                      ? new User
                                      {
                                          UserId = UserId,
                                          Username = Username
                                      }
                                      :
                                          new User
                                          {
                                              UserId = anotherUserId,
                                              Username = anotherUsername
                                          }
                        });
            }
        }

        private StandardKernel GetIoCKernel()
        {
            var modules = new IModule[]
                              {
                                  new InlineModule(
                                      new Action<InlineModule>[]
                                          {
                                              m => m.Bind<IMessageService>()
                                                       .To<InMemoryMessageService>(),
                                              m => m.Bind<IMessageRepository>()
                                                       .To<InMemoryMessageRepository>()
                                                       .Using<SingletonBehavior>(),
                                              m => m.Bind<IValidationRunner>()
                                                       .To<ValidationRunner>(),
                                              m => m.Bind<ITemplateService>()
                                                       .To<InMemoryTemplateService>(),
                                              m => m.Bind<ITemplateRepository>()
                                                       .To<InMemoryTemplateRepository>()
                                          })
                              };
            return new StandardKernel(modules);
        }

        private MessageController GetController()
        {
            kernel = GetIoCKernel();

            var messageController = (MessageController)kernel
                .Get(typeof(MessageController));
            messageController.ControllerContext = new ControllerContext(
                httpContext.Object,
                new RouteData(),
                controllerbase.Object);
            return messageController;
        }

        private void SetupMocks(string username, Guid userid)
        {
            identity.Expect(i => i.Name).Returns(username);
            user.Expect(u => u.Identity).Returns(identity.Object);
            httpContext.Expect(h => h.User).Returns(user.Object);
            httpContext.Expect(h => h.Request).Returns(request.Object);
            MockMembership.Expect(m => m.GetUser(username, false))
                .Returns(MockMembershipUser.Object);
            MockMembershipUser.Expect(u => u.ProviderUserKey)
                .Returns(userid);
        }
        private void AssertCreateValidationError(ActionResult result,
                                                 string errorKey,
                                                 string errorMessage)
        {
            //assert results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            controller.ViewData.ModelState.AssertErrorMessage(errorKey, errorMessage);
            Assert.IsInstanceOfType(typeof(Message),
                                    ((ViewResult)result).ViewData.Model);
            //assert that the ViewData.Model
            var outModel = (((ViewResult)result).ViewData.Model as Message);
            Assert.AreEqual(model.Subject, outModel.Subject);
            Assert.AreEqual(model.Name, outModel.Name);
            Assert.AreEqual(model.Text, outModel.Text);
            Assert.AreEqual("Create New Message", controller.ViewData["Title"],
                            "Page title is wrong");
        }
    }
}