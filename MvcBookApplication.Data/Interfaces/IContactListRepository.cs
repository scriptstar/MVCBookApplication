using System.Linq;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public interface IContactListRepository
    {
        int Add(ContactList list);
        IQueryable<ContactList> Get();
        bool Save(ContactList contactList);
        bool Delete(int id);
    }
}