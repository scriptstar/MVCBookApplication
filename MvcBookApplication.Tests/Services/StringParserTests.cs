using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests.Services
{
[TestFixture]
public class StringParserTests
{
   [Test]
   public void parser_should_return_collection_of_valid_emails()
   {
       var emails = "user1@test.com\r\n" +
                      "user2@test.com\r\n" +
                      "bad email @ test. com\r\n" +
                      "user3@test.com";
       var parser = new StringParser(emails);
       var contacts = parser.Parse();
       Assert.AreEqual(3, contacts.Count);
       Assert.AreEqual("user1@test.com", contacts[0].Email);
       Assert.AreEqual("user2@test.com", contacts[1].Email);
       Assert.AreEqual("user3@test.com", contacts[2].Email);
   }
}
}
