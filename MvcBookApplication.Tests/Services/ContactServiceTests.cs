using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Action = Gallio.Action;

namespace MvcBookApplication.Tests.Services
{
    [TestFixture]
    public class ContactServiceTests
    {
        [Test]
        public void import_should_return_errors_for_existing_emails()
        {
            var userid = Guid.NewGuid();
            var username = "testuser";
            var contact1 = new Contact
                               {
                                   Email = "user1@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contact2 = new Contact
                               {
                                   Email = "existing@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contact3 = new Contact
                               {
                                   Email = "user2@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contacts = new List<Contact>
                       {
                           contact1,
                           contact2,
                           contact3
                       };

            var repository = new InMemoryContactRepository();
            repository.Add(contact2);

            var mockRunner = new Mock<IValidationRunner>();
            mockRunner.Expect(v => v.Run(contact1))
                .Returns(new List<ValidationError>());
            mockRunner.Expect(v => v.Run(contact2))
                .Returns(new List<ValidationError>());
            mockRunner.Expect(v => v.Run(contact3))
                .Returns(new List<ValidationError>());

            var service = new InMemoryContactService(
                repository,
                mockRunner.Object);

            var exception = (ValidationException)
                            Assert.Throws(typeof(ValidationException),
                                          () => service.Import(
                                              new User
                                                  {
                                                      UserId = userid,
                                                      Username = username
                                                  }, contacts));

            Assert.AreEqual(1, exception.ValidationErrors.Count);
            Assert.AreEqual(string.Format("{0} already exists",
                                          contact2.Email),
                            exception.ValidationErrors[0].ErrorMessage);
            Assert.AreEqual(1, repository.Get()
                                   .Count(c => c.Email == contact2.Email));
            mockRunner.VerifyAll();
        }

        [Test]
        public void import_should_add_contacts_to_repository()
        {
            var userid = Guid.NewGuid();
            var username = "testuser";
            var contact1 = new Contact
                               {
                                   Email = "user1@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contact2 = new Contact
                               {
                                   Email = "user2@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contact3 = new Contact
                               {
                                   Email = "user3@abc.com",
                                   User = new User { UserId = userid }
                               };
            var contacts = new List<Contact>
               {
                   contact1,
                   contact2,
                   contact3
               };

            var mockRepo = new Mock<IContactRepository>();
            mockRepo.Expect(r => r.Add(contact1)).Returns(1);
            mockRepo.Expect(r => r.Add(contact2)).Returns(2);
            mockRepo.Expect(r => r.Add(contact3)).Returns(3);
            mockRepo.Expect(r => r.Get())
                .Returns((new List<Contact>()).AsQueryable());

            var mockRunner = new Mock<IValidationRunner>();
            mockRunner.Expect(v => v.Run(contact1))
                .Returns(new List<ValidationError>());
            mockRunner.Expect(v => v.Run(contact2))
                .Returns(new List<ValidationError>());
            mockRunner.Expect(v => v.Run(contact3))
                .Returns(new List<ValidationError>());

            var service = new InMemoryContactService(
                mockRepo.Object,
                mockRunner.Object);

            service.Import(new User
                               {
                                   UserId = userid,
                                   Username = username
                               }, contacts);

            mockRepo.VerifyAll();
            mockRunner.VerifyAll();
        }

        [Test]
        public void import_should_return_error_for_invalid_emails_and_not_add_them()
        {
            var userid = Guid.NewGuid();
            var username = "testuser";
            var contact1 = new Contact
            {
                Email = "user1@abc.com",
                User = new User { UserId = userid }
            };
            var contact2 = new Contact
            {
                Email = "user 2 bad email@abc.com",
                User = new User { UserId = userid }
            };
            var contact3 = new Contact
            {
                Email = "user3@abc.com",
                User = new User { UserId = userid }
            };
            var contacts = new List<Contact>
       {
           contact1,
           contact2,
           contact3
       };

            var mockRepo = new Mock<IContactRepository>();
            mockRepo.Expect(r => r.Add(contact1)).Returns(1);
            mockRepo.Expect(r => r.Add(contact3)).Returns(3);
            mockRepo.Expect(r => r.Get())
                .Returns((new List<Contact>()).AsQueryable());

            var mockRunner = new Mock<IValidationRunner>();
            mockRunner.Expect(v => v.Run(contact1))
                .Returns(new List<ValidationError>());
            mockRunner.Expect(v => v.Run(contact2))
                .Returns(new List<ValidationError>
                     {
                         new ValidationError("Email","Invalid email")
                     });
            mockRunner.Expect(v => v.Run(contact3))
                .Returns(new List<ValidationError>());

            var service = new InMemoryContactService(
                mockRepo.Object,
                mockRunner.Object);


            var exception = (ValidationException)
                           Assert.Throws(typeof(ValidationException),
                                         () => service.Import(new User
                                                                  {
                                                                      UserId = userid,
                                                                      Username = username
                                                                  }, contacts));
            Assert.AreEqual(1, exception.ValidationErrors.Count);
            Assert.AreEqual(string.Format("{0}: Invalid email",
                                          contact2.Email),
                            exception.ValidationErrors[0].ErrorMessage);

            mockRunner.VerifyAll();
            mockRepo.VerifyAll();
            mockRunner.VerifyAll();
        }


[Test]
public void import_should_set_user_object_on_each_contact()
{
    var userid = Guid.NewGuid();
    var username = "testuser";
    var contact1 = new Contact
    {
        Email = "user1@abc.com"
    };
    var contact2 = new Contact
    {
        Email = "user2@abc.com"
    };
    var contact3 = new Contact
    {
        Email = "user3@abc.com"
    };
    var contacts = new List<Contact>
       {
           contact1,
           contact2,
           contact3
       };

    var repo = new InMemoryContactRepository();

    var mockRunner = new Mock<IValidationRunner>();
    mockRunner.Expect(v => v.Run(contact1))
        .Returns(new List<ValidationError>());
    mockRunner.Expect(v => v.Run(contact2))
        .Returns(new List<ValidationError>());
    mockRunner.Expect(v => v.Run(contact3))
        .Returns(new List<ValidationError>());

    var service = new InMemoryContactService(
        repo,
        mockRunner.Object);

    service.Import(new User
    {
        UserId = userid,
        Username = username
    }, contacts);

    Assert.AreEqual(3,repo.Get().Count());
    Assert.AreEqual(3, repo.Get().Count(
                           c => c.User.UserId == userid &&
                                c.User.Username == username));
    mockRunner.VerifyAll();
}

    }
}
