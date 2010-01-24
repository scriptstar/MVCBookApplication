using System.Collections.Generic;
using System.Linq;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using Ninject.Core;

namespace MvcBookApplication.Services
{
    public class InMemoryTemplateService : ITemplateService
    {
        public ITemplateRepository Repository { get; set; }

        public InMemoryTemplateService()
            : this(null)
        {

        }

        [Inject]
        public InMemoryTemplateService(ITemplateRepository repository)
        {
            Repository = repository;
        }

        public IList<Template> Get()
        {
            return Repository.Get().Where(t=> string.IsNullOrEmpty(t.Username)).ToList();
        }

        public int Save(string username, string name, string content)
        {
            return Repository.Save(username, name, content);
        }

        public IList<Template> Get(string username)
        {
            return Repository.Get().Where(t => t.Username == username).ToList();
        }
    }
}