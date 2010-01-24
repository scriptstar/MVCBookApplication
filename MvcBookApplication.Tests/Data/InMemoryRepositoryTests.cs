using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Tests.Data
{
    [TestFixture]
    public class InMemoryRepositoryTests
    {
        private Message message;

        [SetUp]
        public void SetUp()
        {
            message = new Message()
            {
                Subject = "My newsletter subject",
                Name = "October newsletter",
                Text = "Hello subscriber",
                Html = "Hello <b>subscriber</b>"
            };
        }

        [Test]
        public void Add_Message_To_Repository()
        {
            var repo = new InMemoryMessageRepository();
            repo.Add(message);
            //verify message is added correctly
            var msg = repo.Get().Where(m => m.Id == 1).Single();
            Assert.IsNotNull(msg);
            Assert.AreEqual(message.Html, msg.Html);
            Assert.AreEqual(message.Text, msg.Text);
            Assert.AreEqual(message.Subject, msg.Subject);
            Assert.AreEqual(message.Name, msg.Name);
        }

        [Test]
        public void Add_Assigns_Unique_Ids()
        {
            var repo = new InMemoryMessageRepository();
            repo.Add(new Message()); //1
            repo.Add(new Message()); //2
            repo.Add(new Message()); //3
            var msgs = repo.Get();
            Assert.IsNotNull(msgs);
            Assert.AreEqual(3, msgs.Count());
            Assert.AreEqual(1, msgs.Count(m => m.Id == 1));
            Assert.AreEqual(1, msgs.Count(m => m.Id == 2));
            Assert.AreEqual(1, msgs.Count(m => m.Id == 3));
        }

        [Test]
        public void Delete_Message_From_Repository()
        {
            var repo = new InMemoryMessageRepository();
            repo.Add(message);
            //make sure message is added
            var msgs = repo.Get();
            Assert.IsNotNull(msgs);
            Assert.AreEqual(1, msgs.Count());
            var result = repo.Delete(1);
            Assert.IsTrue(result);
            msgs = repo.Get();
            Assert.IsNotNull(msgs);
            Assert.AreEqual(0, msgs.Count());
        }

        [Test]
        public void Delete_Message_Returns_False_On_Exception()
        {
            var repo = new InMemoryMessageRepository();
            repo.Add(message);
            //make sure message is added
            var msgs = repo.Get();
            Assert.IsNotNull(msgs);
            Assert.AreEqual(1, msgs.Count());
            var result = repo.Delete(12); //Id doesn't exist
            Assert.IsFalse(result);
            msgs = repo.Get();
            Assert.IsNotNull(msgs);
            Assert.AreEqual(1, msgs.Count());
        }

        [Test]
        public void Save_Message_To_Repository()
        {
            var repo = new InMemoryMessageRepository();
            repo.Add(message);
            //get message
            var msg = repo.Get().Where(m => m.Id == 1).Single();
            Assert.IsNotNull(msg);
            //make changes
            var text = "new text";
            var name = "new name";
            var html = "new html";
            var subject = "new subject";
            msg.Name = name;
            msg.Html = html;
            msg.Subject = subject;
            msg.Text = text;
            //save message
            repo.Save(msg);
            //get message again
            var msgAfterSave = repo.Get().Where(m => m.Id == 1).Single();
            //verify changes were saved
            Assert.AreEqual(html, msgAfterSave.Html);
            Assert.AreEqual(text, msgAfterSave.Text);
            Assert.AreEqual(subject, msgAfterSave.Subject);
            Assert.AreEqual(name, msgAfterSave.Name);
        }
    }
}
