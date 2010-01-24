using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MvcBookApplication.Validation;

namespace MvcBookApplication.Tests
{
    [TestFixture]
    public class ValidationRunnerTests
    {
        private ModelToValidate model;
        private ValidationRunner runner;

        [SetUp]
        public void SetUp()
        {
            runner = new ValidationRunner();
            model = new ModelToValidate()
                        {
                            Age = 24,
                            Email = "test@test.com",
                            Name = "Jessica Alba",
                            Birthdate = "10/01/08",
                            WorkEmail ="hello@hello.com"
                        };
        }

        [Test]
        public void Validate_Required_Erros_If_Value_Is_Missing()
        {
            model.Name = string.Empty;
            var errors = runner.Run(model);
            AssertValidationError(errors, "Name", "Name is required");
        }

        [Test]
        public void Validate_Required_No_Erros_If_Value_Is_Set()
        {
            var errors = runner.Run(model);
            if (errors != null)
                Assert.AreEqual(errors.Count, 0);
        }

        [Test]
        public void Validate_Range_Errors_If_Value_Less_Than_Min()
        {
            model.Age = 2;
            var errors = runner.Run(model);
            AssertValidationError(errors, "Age", "Age should be 18 to 35");
        }

        [Test]
        public void Validate_Range_Errors_If_Value_More_Than_Max()
        {
            model.Age = 36;
            var errors = runner.Run(model);
            AssertValidationError(errors, "Age", "Age should be 18 to 35");
        }

        [Test, Row(18), Row(25), Row(35)]
        public void Validate_Range_No_Errors_If_Within_Range(int? value)
        {
            model.Age = value;
            var errors = runner.Run(model);
            if (errors != null)
                Assert.AreEqual(errors.Count, 0);
        }

        [Test]
        public void Validate_RegEx_Errors_If_Value_Is_Invalid()
        {
            model.Email = "bad email#@#";
            var errors = runner.Run(model);
            AssertValidationError(errors, "Email", "Email is invalid");
        }

        [Test]
        public void Validate_RegEx_No_Errors_If_Value_Is_Valid()
        {
            var errors = runner.Run(model);
            if (errors != null)
                Assert.AreEqual(errors.Count, 0);
        }

        [Test]
        public void Validate_Only_If_Required_And_Set()
        {
            model.Age = null;
            model.Email = null;
            model.WorkEmail = null;
            var errors = runner.Run(model);
            if (errors != null)
                Assert.AreEqual(errors.Count, 0);
        }

        [Test]
        public void validate_email_errors_if_value_is_not_email()
        {
            model.WorkEmail = "bad email";
            var errors = runner.Run(model);
            AssertValidationError(errors, "WorkEmail", "Invalid Email");
        }
        [Test]
        public void validate_date_errors_if_value_is_not_date()
        {
            model.Birthdate = "bad date";
            var errors = runner.Run(model);
            AssertValidationError(errors, "Birthdate", "Invalid date");
        }

        [Test]
        public void validate_date_no_errors_if_value_is_a_date()
        {
            model.Birthdate = "10/01/2009";
            var errors = runner.Run(model);
            if (errors != null)
                Assert.AreEqual(errors.Count, 0);
        }

        private static void AssertValidationError(List<ValidationError> errors,
                                                 string propertyName,
                                                 string errorMessage)
        {
            Assert.IsNotNull(errors);
            Assert.GreaterThan(errors.Count, 0);
            Assert.AreEqual(errors.Count(e =>
                                         e.PropertyName == propertyName &&
                                         e.ErrorMessage == errorMessage),
                            1);
        }

        internal class ModelToValidate
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            [Range(18, 35, ErrorMessage = "Age should be 18 to 35")]
            public int? Age { get; set; }

            [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                ErrorMessage = "Email is invalid")]
            public string Email { get; set; }

            [Date]
            public string Birthdate { get; set; }

            [Email(ErrorMessage="Invalid Email")]
            public string WorkEmail { get; set; }
        }
    }
}