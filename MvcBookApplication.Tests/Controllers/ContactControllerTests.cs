using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;
using Ninject.Core.Behavior;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class ContactControllerTests
    {
        #region Fields

        private string Username = "testusername";
        private Guid UserId = Guid.NewGuid();
        private ContactController controller;
        private Contact model;

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
        private Mock<HttpFileCollectionBase> mockFiles;
        private Mock<ControllerBase> controllerbase =
            new Mock<ControllerBase>();

        private Mock<IParserFactory> mockParserFactory =
            new Mock<IParserFactory>();
        static StandardKernel kernel;
        private NameValueCollection form;

        #endregion

        [SetUp]
        public void SetUp()
        {
            Username = "testusername";
            UserId = Guid.NewGuid();
            MockMembership = new Mock<MembershipProvider>();
            MockMembershipUser = new Mock<MembershipUser>();
            user = new Mock<IPrincipal>();
            identity = new Mock<IIdentity>();
            httpContext = new Mock<HttpContextBase>();
            request = new Mock<HttpRequestBase>();
            controllerbase = new Mock<ControllerBase>();
            mockParserFactory = new Mock<IParserFactory>();
            mockFiles = new Mock<HttpFileCollectionBase>();
            form = new NameValueCollection();

            SetupMocks(Username, UserId);
            controller = GetController();
            model = new Contact()
                        {
                            Email = "test@test.com",
                            Name = "Julia Roberts",
                            Dob = new DateTime(1967, 10, 28),
                            Sex = Sex.Female
                        };
        }

        #region Create tests

        [Test]
        public void create_returns_view()
        {
            var result = controller.Create();
            Assert.IsInstanceOfType(typeof(Contact), controller.ViewData.Model);
            result.AssertViewResult(controller, "New Contact", "create");
        }
        [Test]
        public void create_should_define_sex_in_viewdata()
        {
            var result = controller.Create();
            var sexlist = controller.ViewData["sex"];
            Assert.IsNotNull(sexlist);
            Assert.IsInstanceOfType(typeof(SelectList), sexlist);
        }
        [Test]
        public void create_returns_error_if_email_is_missing()
        {
            model.Email = null;
            var result = controller.Create(model);
            AssertValidationError(result, "Email", "Email is required");
        }
        [Test]
        public void create_returns_error_if_email_is_invalid()
        {
            model.Email = "bad email";
            var result = controller.Create(model);
            AssertValidationError(result, "Email", "Invalid email");
        }
        [Test]
        public void create_redirects_to_create_action_if_successful()
        {
            var result = controller.Create(model);
            result.AssertRedirectToRouteResult("create");
        }
        [Test]
        public void create_adds_flash_message_if_contact_creation_is_successful()
        {
            var result = controller.Create(model);
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact successfully created"),
                            "Flash message is missing");
        }
        [Test]
        public void create_returns_error_if_contact_already_exists()
        {
            //add it once
            var result = controller.Create(model);
            //make sure it gets added
            result.AssertRedirectToRouteResult("create");
            //add it again
            result = controller.Create(model);
            AssertValidationError(result, "Email", "Contact already exists");

            //verify mocks
            MockMembership.VerifyAll();
            MockMembershipUser.VerifyAll();
        }
        [Test]
        public void create_succeeds_for_duplicate_contacts_by_different_users()
        {
            //add a contact to the repository
            var anotherUsername = Username + 2;
            var anotherUserId = Guid.NewGuid();
            var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
            repo.Add(new Contact()
                         {
                             Email = "test@test.com",
                             Name = "Julia Roberts",
                             Dob = new DateTime(1967, 10, 28),
                             Sex = Sex.Female,
                             User = new User
                                        {
                                            UserId = anotherUserId,
                                            Username = anotherUsername
                                        }

                         });

            //add the same contact for a different user
            var result = controller.Create(model);
            //make sure it gets added
            result.AssertRedirectToRouteResult("create");
            Assert.AreEqual(2, repo.Get().Count());

            //verify mocks
            MockMembership.VerifyAll();
            MockMembershipUser.VerifyAll();
        }

        #endregion

        #region Browse Tests

        [Test]
        public void browse_contacts_should_retrieve_20_or_less_contacts_at_once()
        {
            PopulateRepository();
            var result = controller.Browse(null, null, null);
            Assert.IsInstanceOfType(typeof(PagedList<Contact>),
                                    controller.ViewData.Model,
                                    "View data is the wrong type");
            Assert.LessThanOrEqualTo(((PagedList<Contact>)controller.ViewData.Model).Count,
                                     20, "Page size is wrong");
            result.AssertViewResult(controller, "Browse Contacts", "browse");
        }

        [Test]
        public void browse_contacts_should_retrieve_contacts_for_loggedin_user_only()
        {
            PopulateRepository();
            var result = controller.Browse(null, null, null);
            Assert.AreEqual(25,
                            ((PagedList<Contact>)controller.ViewData.Model).TotalItemCount,
                            "Item count is wrong");
        }

        [Test]
        public void browse_contacts_should_save_sort_options_to_viewdata()
        {
            var result = controller.Browse(null, "email", "asc");
            controller.ViewData.AssertItem("sortby", "email");
            controller.ViewData.AssertItem("sortdir", "asc");
            result.AssertViewResult(controller, "Browse Contacts", "browse");
        }

        #endregion

        #region Edit tests

        [Test]
        public void edit_contact_should_return_view()
        {
            PopulateRepository();
            var result = controller.Edit(1);
            result.AssertViewResult(controller, "Edit Contact", "edit");
        }

        [Test]
        public void edit_contact_should_get_requested_contact()
        {
            PopulateRepository();
            var result = controller.Edit(1);
            Assert.IsInstanceOfType(typeof(Contact), controller.ViewData.Model);
            Assert.AreEqual(1, ((Contact)controller.ViewData.Model).Id);
            result.AssertViewResult(controller, "Edit Contact", "edit");

        }

        [Test]
        public void edit_should_return_error_page_if_requested_contact_is_not_found()
        {
            try
            {
                PopulateRepository();
                var result = controller.Edit(45789); //non-existing contact
                Assert.Fail("Failed to throw exception");
            }
            catch (ArgumentException)
            {
                controller.TempData.AssertItem("error",
                                               "The contact you requested could not be found");
            }
        }

        [Test]
        public void edit_should_return_error_page_if_user_is_not_owner()
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
                var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
                Assert.AreEqual(1, repo.Get().Count(c => c.Id == id));
                controller.TempData.AssertItem("error",
                                               "The contact you requested could not be found");
            }
        }

        //[Test]
        //public void edit_should_define_sex_in_viewdata()
        //{
        //    PopulateRepository();
        //    var result = controller.Edit(1);
        //    var sexlist = controller.ViewData["sex"];
        //    Assert.IsNotNull(sexlist);
        //    Assert.IsInstanceOfType(typeof(SelectList), sexlist);
        //}

        #endregion

        #region Save tests
        [Test]
        public void save_should_return_error_if_email_is_missing()
        {
            PopulateRepository();
            model.Email = null;
            var result = controller.Edit(1, model);
            AssertValidationError(result, "Email", "Email is required");
        }

        [Test]
        public void save_should_return_error_if_email_is_invalid()
        {
            PopulateRepository();
            model.Email = "bad email";
            var result = controller.Edit(1, model);
            AssertValidationError(result, "Email", "Invalid email");
        }

        [Test]
        public void save_should_return_error_if_user_is_not_owner()
        {
            PopulateRepository();
            var result = controller.Edit(2, model);
            AssertValidationError(result, "save", "Error saving the contact");
        }

        [Test]
        public void save_should_return_error_if_email_is_changed_to_a_duplicate_owned_by_user()
        {
            PopulateRepository();
            model.Email = "user2@test.com"; //already exists for user
            var result = controller.Edit(1, model);
            AssertValidationError(result, "Email", "Email already exists");
        }

        [Test]
        public void save_should_redirect_to_browse_view_if_successful()
        {
            PopulateRepository();
            var result = controller.Edit(1, model);
            result.AssertRedirectToRouteResult("browse");
        }

        [Test]
        public void save_should_add_flash_message_if_save_is_successful()
        {
            PopulateRepository();
            var result = controller.Edit(1, model);
            result.AssertRedirectToRouteResult("browse");
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact successfully saved"),
                            "Flash message is missing");
        }

        [Test]
        public void save_should_succeed_if_email_changes_to_a_duplicate_owned_by_other_user()
        {
            PopulateRepository();
            model.Email = "user1@test.com"; //already exists for another user
            var result = controller.Edit(1, model);
            var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
            Assert.AreEqual("user1@test.com",
                            repo.Get()
                                .Single(c => c.Id == 1 &&
                                             c.User.UserId == UserId).Email);
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact successfully saved"),
                            "Flash message is missing");
            result.AssertRedirectToRouteResult("browse");
        }

        [Test]
        public void save_should_save_changes_to_repository()
        {
            PopulateRepository();
            model.Id = 1;
            model.Email = "new@test.com";
            var result = controller.Edit(1, model);
            var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
            Assert.AreEqual("new@test.com",
                            repo.Get()
                                .Single(c => c.Id == 1 &&
                                             c.User.UserId == UserId).Email);
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact successfully saved"),
                            "Flash message is missing");
            result.AssertRedirectToRouteResult("browse");
        }


        #endregion

        #region Delete tests

        [Test]
        public void delete_should_return_view()
        {
            PopulateRepository();
            var result = controller.Delete(1);
            result.AssertViewResult(controller, "Delete Contact", "delete");
        }

        [Test]
        public void delete_should_get_requested_contact()
        {
            PopulateRepository();
            var result = controller.Delete(1);
            Assert.IsInstanceOfType(typeof(Contact), controller.ViewData.Model);
            Assert.AreEqual(1, ((Contact)controller.ViewData.Model).Id);
        }

        [Test]
        public void delete_should_return_error_if_user_is_not_owner()
        {
            try
            {
                PopulateRepository();
                var result = controller.Delete(2); //owned by another user
                Assert.Fail("Failed to throw exception");
            }
            catch (ArgumentException)
            {
                controller.TempData.AssertItem("error",
                                                 "The contact you are trying to delete does not exist");
            }
        }

        #endregion

        #region Delete Submit

        [Test]
        public void deletesubmit_should_return_error_if_user_is_not_owner()
        {
            try
            {
                PopulateRepository();
                var result = controller.DeleteSubmit(2);//owned by another user
                Assert.Fail("Failed to throw exception");
            }
            catch (ArgumentException)
            {
                //make sure it is not deleted

                controller.TempData.AssertItem("error",
                                               "Error deleting the contact");
            }
        }

        [Test]
        public void deletesubmit_should_redirect_to_browse_view_if_successful()
        {
            PopulateRepository();
            var result = controller.DeleteSubmit(1);
            result.AssertRedirectToRouteResult("browse");
        }

        [Test]
        public void deletesubmit_should_add_flash_message_if_successful()
        {
            PopulateRepository();
            var result = controller.DeleteSubmit(1);
            result.AssertRedirectToRouteResult("browse");
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact successfully deleted"),
                            "Flash message is missing");
        }

        [Test]
        public void deletesubmit_should_delete_contact_from_repository()
        {
            PopulateRepository();
            var id = 1;
            var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
            Assert.AreEqual(1,
                            repo.Get().Count(c => c.Id == id));
            var result = controller.DeleteSubmit(id);
            Assert.AreEqual(0,
                            repo.Get().Count(c => c.Id == id));
        }

        #endregion


        #region Import tests

        [Test]
        public void import_should_return_view()
        {
            var result = controller.Import();
            result.AssertViewResult(controller, "Import Contacts", "import");
        }

        [Test]
        public void import_should_return_error_if_contacts_and_file_are_missing()
        {
            var result = controller.Import(null);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            controller.ViewData.ModelState
                .AssertErrorMessage("Import",
                                    "You must enter some contacts or upload a file");
            result.AssertViewResult(controller, "Import Contacts", "import");
        }

        [Test]
        public void import_should_add_flash_message_if_import_is_successful()
        {
            var mockParser = new Mock<IParser>();
            mockParserFactory.Expect(f => f.Create("user1@test.com", null))
                .Returns(mockParser.Object);
            mockParser.Expect(p => p.Parse())
                .Returns(new List<Contact>());
            var mockService = new Mock<IContactService>();
            mockService.Expect(s => s.Import(new User
                                                 {
                                                     UserId = UserId,
                                                     Username = Username
                                                 }, new List<Contact>
                                                        {
                                                            new Contact{Email="user1@test.com"}
                                                        }));
            controller.Service = mockService.Object;
            var result = controller.Import("user1@test.com");
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contacts successfully imported"),
                            "Flash message is missing");

        }

        [Test]
        public void import_should_redirect_to_browse_action_if_successful()
        {
            var mockParser = new Mock<IParser>();
            mockParserFactory.Expect(f => f.Create("user1@test.com", null))
                .Returns(mockParser.Object);
            mockParser.Expect(p => p.Parse())
                .Returns(new List<Contact>());
            var mockService = new Mock<IContactService>();
            mockService.Expect(s => s.Import(new User
                                                 {
                                                     UserId = UserId,
                                                     Username = Username
                                                 }, new List<Contact>
                                                        {
                                                            new Contact {Email = "user1@test.com"}
                                                        }));
            controller.Service = mockService.Object;
            var result = controller.Import("user1@test.com");
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            result.AssertRedirectToRouteResult("browse");
        }

        [Test]
        public void import_should_call_the_parser_factory()
        {
            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(5000);

            var mockParser = new Mock<IParser>();
            mockParserFactory.Expect(f => f.Create(string.Empty, mockFile.Object))
                .Returns(mockParser.Object);

            mockFiles = new Mock<HttpFileCollectionBase>();
            mockFiles.Expect(f => f.Count).Returns(1);
            mockFiles.Expect(f => f[0]).Returns(mockFile.Object);
            request.Expect(r => r.Files).Returns(mockFiles.Object);

            var mockService = new Mock<IContactService>();
            mockService.Expect(s => s.Import(new User
                                                 {
                                                     UserId = UserId,
                                                     Username = Username
                                                 }, null));
            controller.Service = mockService.Object;
            controller.Import(string.Empty);
            mockParserFactory.VerifyAll();
        }

        [Test]
        public void import_should_call_the_parse_method()
        {
            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(5000);
            var mockParser = new Mock<IParser>();
            mockParserFactory.Expect(f => f.Create(string.Empty, mockFile.Object))
                .Returns(mockParser.Object);
            var contacts = new List<Contact>
                               {
                                   new Contact {Email = "user1@test.com"},
                                   new Contact {Email = "user2@test.com"},
                                   new Contact {Email = "user3@test.com"}
                               };
            mockParser.Expect(p => p.Parse())
                .Returns(contacts);
            mockFiles = new Mock<HttpFileCollectionBase>();
            mockFiles.Expect(f => f.Count).Returns(1);
            mockFiles.Expect(f => f[0]).Returns(mockFile.Object);
            request.Expect(r => r.Files).Returns(mockFiles.Object);
            var mockService = new Mock<IContactService>();
            mockService.Expect(s => s.Import(new User
                                                 {
                                                     UserId = UserId,
                                                     Username = Username
                                                 }, contacts));
            controller.Service = mockService.Object;

            controller.Import(string.Empty);
            mockParser.VerifyAll();
        }


        [Test]
        public void import_should_call_import_on_service()
        {
            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Expect(f => f.ContentLength).Returns(5000);

            var mockParser = new Mock<IParser>();
            mockParserFactory.Expect(f => f.Create(string.Empty, mockFile.Object))
                .Returns(mockParser.Object);
            var contacts = new List<Contact>
                               {
                                   new Contact {Email = "user1@test.com"},
                                   new Contact {Email = "user2@test.com"},
                                   new Contact {Email = "user3@test.com"}
                               };
            mockParser.Expect(p => p.Parse())
                .Returns(contacts);
            mockFiles = new Mock<HttpFileCollectionBase>();
            mockFiles.Expect(f => f.Count).Returns(1);
            mockFiles.Expect(f => f[0]).Returns(mockFile.Object);
            request.Expect(r => r.Files).Returns(mockFiles.Object);

            var mockService = new Mock<IContactService>();
            mockService.Expect(s => s.Import(new User
                                                 {
                                                     UserId = UserId,
                                                     Username = Username
                                                 }, contacts));
            controller.Service = mockService.Object;
            controller.Import(string.Empty);

            mockService.VerifyAll();
        }

        #endregion

        #region Private Methods

        private void PopulateRepository()
        {
            var anotherUsername = Username + 2;
            var anotherUserId = Guid.NewGuid();
            var repo = (IContactRepository)kernel.Get(typeof(IContactRepository));
            for (var i = 0; i < 50; i++)
            {
                repo.Add(
                    new Contact
                        {
                            Email = ("user" + i + "@test.com"),
                            Name = string.Format("First{0} Last{1}", i, i),
                            Dob = (new DateTime(1967, 10, 28)).AddDays(i),
                            Sex = (i % 3 == 0
                                       ?
                                           Sex.Undefined
                                       :
                                           (i % 2 == 0
                                                ?
                                                    Sex.Female
                                                :
                                                    Sex.Male)),
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

        private void AssertValidationError(ActionResult result,
                                           string errorKey,
                                           string errorMessage)
        {
            //assert results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            controller.ViewData.ModelState.AssertErrorMessage(errorKey, errorMessage);
            Assert.IsInstanceOfType(typeof(Contact),
                                    ((ViewResult)result).ViewData.Model);
            //asser that the ViewData.Model
            var outModel = (((ViewResult)result).ViewData.Model as Contact);
            Assert.AreEqual(model.Email, outModel.Email);
            Assert.AreEqual(model.Name, outModel.Name);
            Assert.AreEqual(model.Sex, outModel.Sex);
            Assert.AreEqual(model.Dob, outModel.Dob);
        }

        private StandardKernel GetIoCKernel()
        {
            var modules = new IModule[]
                              {
                                  new InlineModule(
                                      new Action<InlineModule>[]
                                          {
                                              m => m.Bind<MembershipProvider>()
                                                       .ToConstant(MockMembership.Object),
                                              m => m.Bind<IContactService>()
                                                       .To<InMemoryContactService>(),
                                              m => m.Bind<IContactRepository>()
                                                       .To<InMemoryContactRepository>()
                                                       .Using<SingletonBehavior>(),
                                              m => m.Bind<IValidationRunner>()
                                                       .To<ValidationRunner>(),
                                              m => m.Bind<IParserFactory>()
                                                       .ToConstant(mockParserFactory.Object)
                                          })
                              };
            return new StandardKernel(modules);
        }

        private ContactController GetController()
        {
            kernel = GetIoCKernel();

            var contactController = (ContactController)kernel
                .Get(typeof(ContactController));
            contactController.ControllerContext = new ControllerContext(
                httpContext.Object,
                new RouteData(),
                controllerbase.Object);
            return contactController;
        }

        private void SetupMocks(string username, Guid userid)
        {
            identity.Expect(i => i.Name).Returns(username);
            user.Expect(u => u.Identity).Returns(identity.Object);
            httpContext.Expect(h => h.User).Returns(user.Object);
            httpContext.Expect(h => h.Request).Returns(request.Object);
            request.Expect(r => r.Files).Returns(mockFiles.Object);
            request.Expect(r => r.Form).Returns(form);
            MockMembership.Expect(m => m.GetUser(username, false))
                .Returns(MockMembershipUser.Object);
            MockMembershipUser.Expect(u => u.ProviderUserKey)
                .Returns(userid);
        }

        #endregion

    }


}
