using System.Linq.Dynamic;
using System;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using System.Linq;
using MvcBookApplication.Validation;
using Ninject.Core;
using IValidationRunner = MvcBookApplication.Validation.IValidationRunner;
using System.Collections.Generic;

namespace MvcBookApplication.Services
{
    public class InMemoryContactListService : IContactListService
    {
        private IValidationRunner ValidationRunner { get; set; }
        public IContactListRepository Repository { get; set; }
        public InMemoryContactListService()
            : this(null, null)
        {

        }
        [Inject]
        public InMemoryContactListService(IContactListRepository repository, IValidationRunner validationRunner)
        {
            ValidationRunner = validationRunner ?? new ValidationRunner();
            Repository = repository ?? new InMemoryContactListRepository();
        }
        
        
        public int Add(ContactList contactList)
        {
            var errors = ValidationRunner.Run(contactList);
            if (errors != null && errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
            //check if name is unique
            if (Repository.Get()
                .Count(c => c.Name.ToLower() == contactList.Name.ToLower() &&
                    c.User.UserId == contactList.User.UserId) > 0)
            {
                throw new ValidationException(
                    new List<ValidationError>
                        {
                            new ValidationError("Name", "Contact list already exists")
                        });
            }

            return Repository.Add(contactList);
        }

        public PagedList<ContactList> GetPage(Guid userid, int? page, string sortBy, string sortDirection)
        {
            sortBy = sortBy ?? "name";
            sortDirection = sortDirection ?? "asc";
            page = page ?? 1;
            page = page < 1 ? 1 : page;
            return Repository.Get()
                .Where(c => c.User.UserId == userid)
                .OrderBy(sortBy + " " + sortDirection)
                .ToPagedList((int)(page - 1), 20);
        }

        public ContactList Get(Guid userid, int id)
        {
            return Repository.Get().Where(c =>
                                        c.Id == id &&
                                        c.User.UserId == userid)
                                  .SingleOrDefault();
        }

        public bool Save(Guid userid, ContactList contactList)
        {
            var errors = ValidationRunner.Run(contactList);
            if (errors != null && errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
            //check if name is unique
            if (Repository.Get()
                .Count(c => c.Name.ToLower() == contactList.Name.ToLower() &&
                    c.User.UserId == userid && c.Id != contactList.Id) > 0
                    )
            {
                throw new ValidationException(
                    new List<ValidationError>
                        {
                            new ValidationError("Name", "Contact list already exists")
                        });
            }
            //make sure user has permission to save
            if (Repository.Get().Count(c =>
                                   c.Id == contactList.Id &&
                                   c.User.UserId == userid) == 0)
                return false;
            return Repository.Save(contactList);
        }

        public bool Delete(Guid userid, int id)
        {
            //make sure user has permission to delete
            if (Repository.Get().Count(c =>
                                   c.Id == id &&
                                   c.User.UserId == userid) == 0)
                return false;
            return Repository.Delete(id);
        }
    }
}