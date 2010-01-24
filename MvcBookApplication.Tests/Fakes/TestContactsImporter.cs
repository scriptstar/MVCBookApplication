using System.Collections.Generic;
using System.Web;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;

namespace MvcBookApplication.Tests
{
    public class TestContactsImporter :IContactsImporter
    {
        public void Parse()
        {
            throw new System.NotImplementedException();
        }

        public void OnParseCompleted(List<Contact> contact)
        {
            throw new System.NotImplementedException();
        }

        public void OnError(List<string> errors)
        {
            throw new System.NotImplementedException();
        }
    }
}