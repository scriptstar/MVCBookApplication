using System.Collections.Generic;
using System.Linq;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Validation;
using Ninject.Core;

namespace MvcBookApplication.Services
{
    public class InMemoryMessageService : IMessageService
    {
        private IValidationRunner ValidationRunner { get; set; }
        private IMessageRepository Repository { get; set; }

        public InMemoryMessageService()
            : this(null, null)
        {
        }

        [Inject]
        public InMemoryMessageService(IMessageRepository repository,
            IValidationRunner validationRunner)
        {
            ValidationRunner = validationRunner ?? new ValidationRunner();
            Repository = repository ?? new InMemoryMessageRepository();
        }


        public int Add(Message message)
        {
            var errors = ValidationRunner.Run(message);
            if (errors != null && errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
            return Repository.Add(message);
        }


        public Message GetById(int Id)
        {
            return Repository.Get().Single(m => m.Id == Id);
        }

        public List<Message> Search(string searchTerm)
        {
            return Repository.Get().Where(m => m.Text.Contains(searchTerm)).ToList();
        }

        public PagedList<Message> GetPage(string username, int? page)
        {
            page = page ?? 1;
            page = page < 1 ? 1 : page;
            return Repository.Get()
                .Where(c => c.User.Username.ToLower() == username.ToLower())
                .ToPagedList((int)(page - 1), 20);
        }

public Message Get(string username, int id)
{

    return Repository.Get().Where(c =>
                                c.Id == id &&
                                c.User.Username.ToLower() == username.ToLower())
                          .SingleOrDefault();
}

        public bool Save(string username, Message message)
        {
            return Repository.Save(message);
        }
    }
}