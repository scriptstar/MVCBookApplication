using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
public interface IParser
{
    IList<Contact> Parse();
}

public class StringParser : IParser
{
    public string Contacts { get; set; }

    public StringParser(string contacts)
    {
        Contacts = contacts;
    }

    public IList<Contact> Parse()
    {
        var parsedContacts = new List<Contact>();
        var emails = Contacts.Split(new char[]
                                        {
                                            Convert.ToChar("\r"),
                                            Convert.ToChar("\n")
                                        });
        foreach (var email in emails)
        {
            if(AppHelper.IsValidEmail(email))
            {
                parsedContacts.Add(new Contact{Email = email.Trim()});
            }
        }
        return parsedContacts;
    }
}

public class TextFileParser:IParser
{
    public HttpPostedFileBase PostedFile { get; set; }

    public TextFileParser(HttpPostedFileBase postedFile)
    {
        PostedFile = postedFile;
    }

public IList<Contact> Parse()
{
    var reader = new StreamReader(PostedFile.InputStream);
    var filecontent = reader.ReadToEnd();
    var parsedContacts = new List<Contact>();
    var emails = filecontent.Split(new char[]
                                    {
                                        Convert.ToChar("\r"),
                                        Convert.ToChar("\n")
                                    });
    foreach (var email in emails)
    {
        if (AppHelper.IsValidEmail(email))
        {
            parsedContacts.Add(new Contact { Email = email.Trim() });
        }
    }
    return parsedContacts;
}
}
}