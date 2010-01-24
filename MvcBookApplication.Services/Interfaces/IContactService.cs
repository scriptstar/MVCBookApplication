using System;
using System.Collections.Generic;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public interface IContactService
    {
        int Add(Contact contact);
        PagedList<Contact> GetPage(Guid userid, int? page);

        PagedList<Contact> GetPage(Guid userid, int? page,
                                   string sortBy, string sortDirection);

        Contact Get(Guid userid, int id);
        bool Save(Guid userid, Contact contact);
        bool Delete(Guid userid, int id);
        void Import(User user, IList<Contact> contacts);
    }
}