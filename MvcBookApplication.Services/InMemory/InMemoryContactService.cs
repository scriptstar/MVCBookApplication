using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Security;
using System.Web.Security;
using MvcBookApplication.Data;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Validation;
using Ninject.Core;
using IValidationRunner = MvcBookApplication.Validation.IValidationRunner;

namespace MvcBookApplication.Services
{
    public class InMemoryContactService : IContactService
    {
        private IValidationRunner ValidationRunner { get; set; }
        public IContactRepository Repository { get; set; }

        public InMemoryContactService()
            : this(null, null)
        {

        }
        [Inject]
        public InMemoryContactService(IContactRepository repository, IValidationRunner validationRunner)
        {
            ValidationRunner = validationRunner ?? new ValidationRunner();
            Repository = repository ?? new InMemoryContactRepository();
        }


        public int Add(Contact contact)
        {
            var errors = ValidationRunner.Run(contact);
            if (errors != null && errors.Count > 0)
            {
                throw new ValidationException(errors);
            }

            //check if email is unique
            if (Repository.Get()
                .Count(c => c.Email.ToLower() == contact.Email.ToLower() &&
                    c.User.UserId == contact.User.UserId) > 0)
            {
                throw new ValidationException(
                    new List<ValidationError>
                        {
                            new ValidationError("Email", "Contact already exists")
                        });
            }
            return Repository.Add(contact);
        }

        public PagedList<Contact> GetPage(Guid userid, int? page)
        {
            return GetPage(userid, page, "email", "asc");
        }

        public PagedList<Contact> GetPage(Guid userid, int? page,
            string sortBy, string sortDirection)
        {
            sortBy = sortBy ?? "email";
            sortDirection = sortDirection ?? "asc";
            page = page ?? 1;
            page = page < 1 ? 1 : page;
            return Repository.Get()
                .Where(c => c.User.UserId == userid)
                .OrderBy(sortBy + " " + sortDirection)
                .ToPagedList((int)(page - 1), 20);
        }

        public Contact Get(Guid userid, int id)
        {
            return Repository.Get().Where(c =>
                                          c.Id == id &&
                                          c.User.UserId == userid)
                                    .SingleOrDefault();
        }

        public bool Save(Guid userid, Contact contact)
        {
            var errors = ValidationRunner.Run(contact);
            if (errors != null && errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
            //check if email is unique
            if (Repository.Get()
                .Count(c => c.Email.ToLower() == contact.Email.ToLower() &&
                    c.User.UserId == userid && c.Id != contact.Id) > 0)
            {
                throw new ValidationException(
                    new List<ValidationError>
                {
                    new ValidationError("Email", "Email already exists")
                });
            }
            //make sure user has permission to save
            if (Repository.Get().Count(c =>
                                   c.Id == contact.Id &&
                                   c.User.UserId == userid) == 0)
                return false;
            return Repository.Save(contact);
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

public void Import(User user, IList<Contact> contacts)
{
    var errors = new List<ValidationError>();
    for (int i = 0; i < contacts.Count; i++)
    {
        var contact = contacts[i];
        contact.User = user;
        try
        {
            var contactErrors = ValidationRunner.Run(contact);
            if (contactErrors != null && contactErrors.Count > 0)
            {
                foreach (var contactError in contactErrors)
                {
                    contactError.ErrorMessage = string.Format("{0}: {1}",
                                                              contact.Email,
                                                              contactError.ErrorMessage);
                }
                errors.AddRange(contactErrors);
                continue;
            }
            //check if email exists
            if (Repository.Get()
                    .Count(c => c.Email.ToLower() == contact.Email.ToLower() &&
                                c.User.UserId == contact.User.UserId) > 0)
            {
                errors.Add(
                    new ValidationError("Import",
                                        string.Format("{0} already exists",
                                                      contact.Email)));
            }
            else
            {
                Repository.Add(contact);                
            }
        }
        catch
        {
            errors.Add(
                new ValidationError("Import",
                                    string.Format("Error importing {0}",
                                                  contact.Email)));
        }
    }
    if (errors.Count > 0)
    {
        throw new ValidationException(errors);
    }
}
    }
}