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
    public class JQueryValidationGeneratorTests
    {
        private ModelToValidate model;
        private JQueryValidationGenerator generator;
        [SetUp]
        public void SetUp()
        {
            generator = new JQueryValidationGenerator();
            model = new ModelToValidate()
            {
                Age = 24,
                Email = "test@test.com",
                Name = "Jessica Alba",
                WorkEmail = "work@test.com",
                Birthdate = "02/17/2009"
            };
        }
        [Test]
        public void Generates_Rule_For_Required_Field()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("name:{required:true}"));
        }

        [Test]
        public void Generates_Rule_For_Range_Field()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("age:{range:[18,35]}"));
        }

        [Test]
        public void Generates_Rule_For_Email_Field()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("email:{email:true}"));
        }

        [Test]
        public void generates_rule_for_date_field()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("date:{date:true}"));
        }

        [Test]
        public void Generates_Rule_For_Multiple_Validations()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("workemail:{required:true,email:true}"));
        }

        [Test]
        public void Generates_Message()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("name:{required:'Name is required'}"));
        }

        [Test]
        public void Generates_Multiple_Messages()
        {
            string script = generator.Generate("createForm", model);
            Assert.IsTrue(script.Contains("workemail:{required:'Work email is required',email:'Work email is invalid'}"));
        }

        internal class ModelToValidate
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            [Range(18, 35, ErrorMessage = "Age should be 18 to 35")]
            public int? Age { get; set; }

            [Email(ErrorMessage = "Email is invalid")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Work email is required")]
            [Email(ErrorMessage = "Work email is invalid")]
            public string WorkEmail { get; set; }

            [Date]
            public string Birthdate { get; set; }
        }
    }
}
