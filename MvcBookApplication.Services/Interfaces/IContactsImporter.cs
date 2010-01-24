using System.Collections.Generic;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public interface IContactsImporter
    {
        void Parse();
        //void Parse(string contacts);
        //void Parse(HttpPostedFileBase fileBase);
        void OnParseCompleted(List<Contact> contact);
        void OnError(List<string> errors);
    }
}