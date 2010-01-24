using System;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public interface IContactListService
    {
        int Add(ContactList contactList);
        PagedList<ContactList> GetPage(Guid userid, int? page,
                                       string sortBy, string sortDirection);

        ContactList Get(Guid userid, int id);
        bool Save(Guid userid, ContactList contactList);
        bool Delete(Guid userid, int i);
    }
}