using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests.Services
{
    [TestFixture]
    public class ParserFactoryTests
    {
        [Test]
        public void factory_should_return_string_parser_if_contacts_are_passed_in()
        {
            var factory = new ParserFactory();
            var contacts = "bob@email.com";
            var parser = factory.Create(contacts, null);
            Assert.IsInstanceOfType(typeof(StringParser), parser,
                                    "Wrong parser type returned");
        }

        [Test]
        public void factory_should_return_text_file_parser_for_uploaded_text_files()
        {
            var factory = new ParserFactory();
            var contacts = string.Empty;
            var mockUploadedTextFile = new Mock<HttpPostedFileBase>();
            mockUploadedTextFile.Expect(f => f.FileName).Returns("textfile.txt");
            var parser = factory.Create(contacts, mockUploadedTextFile.Object);
            Assert.IsInstanceOfType(typeof(TextFileParser), parser,
                                    "Wrong parser type returned");
        }
        [Test]
        public void factory_should_throw_exception_if_file_format_is_not_supported()
        {
            var factory = new ParserFactory();
            var contacts = string.Empty;
            var mockUploadedTextFile = new Mock<HttpPostedFileBase>();
            mockUploadedTextFile.Expect(f => f.FileName).Returns("textfile.xls");
            Assert.Throws(typeof(NotImplementedException),
                          () => factory.Create(contacts, mockUploadedTextFile.Object));
        }

        [Test]
        public void factory_should_throw_exception_if_arguments_are_invalid()
        {
            var factory = new ParserFactory();
            var contacts = string.Empty;
            Assert.Throws(typeof(ArgumentNullException),
                          () => factory.Create(contacts, null));
        }
    }
}
