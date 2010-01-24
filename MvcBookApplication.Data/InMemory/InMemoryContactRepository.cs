using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public class InMemoryContactRepository : IContactRepository
    {
        private List<Contact> Contacts { get; set; }

        public InMemoryContactRepository()
        {
            Contacts = new List<Contact>();
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

        public int Add(Contact contact)
        {
            contact.Id = AutoId;
            Contacts.Add(contact);
            return contact.Id;
        }

        public IQueryable<Contact> Get()
        {
            var q = from m in Contacts select m;
        return q.AsQueryable();
        }

        public bool Save(Contact contact)
        {
            var old = Contacts.Single(c => c.Id == contact.Id);
            old.Dob = contact.Dob;
            old.Email = contact.Email;
            old.Name = contact.Name;
            old.Sex = contact.Sex;
            return true;
        }

        public bool Delete(int id)
        {
            var contact = Contacts.Single(c => c.Id == id);
            return Contacts.Remove(contact);
        }
    }

    }
