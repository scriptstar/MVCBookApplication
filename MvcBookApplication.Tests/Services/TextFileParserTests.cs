using System;
using System.Collections.Generic;
using System.IO;
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
    public class TextFileParserTests
    {
[Test]
public void parser_should_return_collection_of_valid_emails()
{
    var mockFile = new Mock<HttpPostedFileBase>();

    var emails = "user1@test.com\r\n" +
               "user2@test.com\r\n" +
               "bad email @ test. com\r\n" +
               "user3@test.com";
    var encoding = new ASCIIEncoding();
    var buffer = encoding.GetBytes(emails);
    var stream = new MemoryStream(buffer);

    mockFile.Expect(f => f.InputStream)
        .Returns(stream);

    var parser = new TextFileParser(mockFile.Object);
    var contacts = parser.Parse();
    Assert.AreEqual(3, contacts.Count);
    Assert.AreEqual("user1@test.com", contacts[0].Email);
    Assert.AreEqual("user2@test.com", contacts[1].Email);
    Assert.AreEqual("user3@test.com", contacts[2].Email);

}
    }
}
