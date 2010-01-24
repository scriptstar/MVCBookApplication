using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;

//using MvcBookApplication.ValidationX;

namespace MvcBookApplication.Tests.Services
{
    [TestFixture]
    public class MessageServiceTests
    {
        private Message message;
        private IValidationRunner vrunner;
        private IMessageService service;

        [SetUp]
        public void SetUp()
        {
            vrunner = new ValidationRunner();
            service = new InMemoryMessageService(null, vrunner);
            message = new Message()
                          {
                              Html = "<b>Hello user</b>",
                              Name = "Message Name",
                              Subject = "Subject",
                              Text = "Hello user"
                          };
        }

        [Test]
        public void Add_Throws_Exception_If_Validation_Returns_Errors()
        {
            var invalidMessage = new Message();

            //mock a failed validation
            var mockValidationRunner = new Mock<IValidationRunner>();
            mockValidationRunner.Expect(v => v.Run(invalidMessage))
                .Returns(new List<ValidationError>
                             {
                                 new ValidationError("prop",
                                                     "error")
                             });

            service = new InMemoryMessageService(null, mockValidationRunner.Object);
            Assert.Throws<ValidationException>(() => service.Add(invalidMessage));

            mockValidationRunner.VerifyAll();
        }

        [Test]
        public void Add_Throws_Exception_If_Name_Is_Missing()
        {
            message.Name = string.Empty;

            var ex = Assert.Throws<ValidationException>(() => service.Add(message));
            AssertValidationError(ex, "Name", "Name is required");
        }

        [Test]
        public void Add_Throws_Exception_If_Text_Is_Missing()
        {
            message.Text = null;
            var ex = Assert.Throws<ValidationException>(() => service.Add(message));
            AssertValidationError(ex, "Text", "A plain text body is required");
        }

        [Test]
        public void Add_Throws_Exception_If_Subject_Is_Missing()
        {
            message.Subject = null;
            var ex = Assert.Throws<ValidationException>(() => service.Add(message));
            AssertValidationError(ex, "Subject", "Subject is required");
        }

        [Test]
        public void Add_Throws_Exception_With_Multiple_Errors()
        {
            message.Subject = null;
            message.Name = null;
            message.Text = string.Empty;

            var ex = Assert.Throws<ValidationException>(() => service.Add(message));

            Assert.IsNotNull(ex.ValidationErrors);
            Assert.AreEqual(ex.ValidationErrors.Count, 3);
        }

        private static void AssertValidationError(ValidationException validationException,
                                                  string propertyName,
                                                  string errorMessage)
        {
            Assert.IsNotNull(validationException.ValidationErrors);
            Assert.GreaterThan(validationException.ValidationErrors.Count, 0);
            //asser the correct error
            Assert.AreEqual(validationException.ValidationErrors
                                .Count(e => e.PropertyName == propertyName &&
                                            e.ErrorMessage == errorMessage), 1);
        }
    }
}