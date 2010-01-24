using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class ContactListControllerTests
    {
        private ContactListController controller;
        private ContactList model;
        private InMemoryContactListService clService;
        private InMemoryContactListRepository clRepo;
        private IValidationRunner validationRunner;

        [SetUp]
        public void SetUp()
        {
            clRepo = new InMemoryContactListRepository();
            validationRunner = new ValidationRunner();
            clService = new InMemoryContactListService(clRepo, validationRunner);
            controller = new ContactListController(clService, MyMocks.MembershipProvider.Object);
            controller.SetFakeControllerContext();
            model = new ContactList
                        {
                            Name = "My First List",
                            Description = "My Description of the list"
                        };
        }

        [Test]
        public void create_should_return_view()
        {
            var result = controller.Create();
            Assert.IsInstanceOfType(typeof(ContactList), controller.ViewData.Model);
            result.AssertViewResult(controller, "New Contact List", "create");
        }

        [Test]
        public void create_should_return_error_if_name_is_missing()
        {
            model.Name = null;
            var result = controller.Create(model);
            AssertValidationError(result, "Name", "Name is required");
        }

        [Test]
        public void create_should_return_error_if_name_is_invalid()
        {
            model.Name = "bad%$# name";
            var result = controller.Create(model);
            AssertValidationError(result, "Name", "Invalid name");
        }

        [Test]
        public void create_should_redirect_to_create_action_if_successful()
        {
            var result = controller.Create(model);
            result.AssertRedirectToRouteResult("create");
        }


        [Test]
        public void create_should_add_flash_message_if_contactlist_creation_is_successful()
        {
            var result = controller.Create(model);
            Assert.IsTrue(controller.ModelState.IsValid, "model state is invalid");
            Assert.Contains(controller.TempData,
                            new KeyValuePair<string, object>
                                ("flash",
                                 "Contact List successfully created"),
                            "Flash message is missing");
        }

        [Test]
        public void create_returns_error_if_contactlist_with_same_name_already_exists()
        {
            //add it once
            var result = controller.Create(model);
            //make sure it gets added
            result.AssertRedirectToRouteResult("create");
            //add it again
            result = controller.Create(model);
            AssertValidationError(result, "Name", "Contact list already exists");
        }

        [Test]
        public void create_succeeds_for_duplicate_contactlists_by_different_users()
        {
            //add a contact to the repository
            var anotherUsername = "anotherUser";
            var anotherUserId = Guid.NewGuid();
            
            clRepo.Add(new ContactList()
            {
                Name = "My First List",
                Description = "Hello",
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
            Assert.AreEqual(2, clRepo.Get().Count());
            Assert.AreEqual("My First List", model.Name);
        }

        private void AssertValidationError(ActionResult result,
                                       string errorKey,
                                       string errorMessage)
        {
            //assert results
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(typeof(ViewResult), result);
            controller.ViewData.ModelState.AssertErrorMessage(errorKey, errorMessage);
            Assert.IsInstanceOfType(typeof(ContactList),
                                    ((ViewResult)result).ViewData.Model);
            //asser that the ViewData.Model
            var outModel = (((ViewResult)result).ViewData.Model as ContactList);
            Assert.AreEqual(model.Name, outModel.Name);
            Assert.AreEqual(model.Description, outModel.Description);
        }
    }
}