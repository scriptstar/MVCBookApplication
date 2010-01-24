using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests.Services
{
    [TestFixture]
    public class TemplateServiceTests
    {
        [Test]
        public void get_should_return_list_of_templates()
        {
            var mockRepo = new Mock<ITemplateRepository>();
            mockRepo.Expect(r => r.Get()).Returns(new List<Template>
                                              {
                                                  new Template(),
                                                  new Template()
                                              }.AsQueryable());
            ITemplateService service = new InMemoryTemplateService(mockRepo.Object);
            var templates = service.Get();
            Assert.IsNotNull(templates);
            Assert.AreEqual(2, templates.Count);
        }

        [Test]
        public void save_should_save_to_repository_and_return_id()
        {
            var template = new Template();
            var username = "test";
            var mockRepo = new Mock<ITemplateRepository>();
            mockRepo.Expect(r => r.Save(username, "name", "content")).Returns(1);

            ITemplateService service = new InMemoryTemplateService(mockRepo.Object);
            var id = service.Save(username, "name", "content");

            mockRepo.VerifyAll();
            Assert.AreEqual(1, id);
        }
    }
}