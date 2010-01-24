using System.Linq;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public interface IContactRepository
    {
        int Add(Contact contact);
        IQueryable<Contact> Get();
        bool Save(Contact contact);
        bool Delete(int id);
    }
}