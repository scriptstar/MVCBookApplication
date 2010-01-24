using System.Collections.Generic;
using System.Linq;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public class InMemoryContactListRepository : IContactListRepository
    {
        private List<ContactList> ContactLists { get; set; }

        public InMemoryContactListRepository()
        {
            ContactLists = new List<ContactList>();
        }

        private int _autoId;
        private int AutoId
        {
            get
            {
                _autoId += 1;
                return _autoId;
            }
        }

        public int Add(ContactList contactList)
        {
            contactList.Id = AutoId;
            ContactLists.Add(contactList);
            return contactList.Id;
        }

        public IQueryable<ContactList> Get()
        {
            var q = from m in ContactLists select m;
            return q.AsQueryable();
        }

        public bool Save(ContactList contactList)
        {
            var old = ContactLists.Single(c => c.Id == contactList.Id);
            old.Name = contactList.Name;
            old.Description = contactList.Description;
            return true;
        }

        public bool Delete(int id)
        {
            var contactlist = ContactLists.Single(c => c.Id == id);
            return ContactLists.Remove(contactlist);
        }
    }
}