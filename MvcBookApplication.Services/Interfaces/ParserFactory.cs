using System;
using System.IO;
using System.Web;

namespace MvcBookApplication.Services
{
    public class ParserFactory : IParserFactory
    {
public IParser Create(string contacts, HttpPostedFileBase file)
{
    if (!string.IsNullOrEmpty(contacts))
        return new StringParser(contacts);

    if (file != null)
    {
        if (Path.GetExtension(file.FileName).ToLower() == ".txt")
            return new TextFileParser(file);
        else
            throw new NotImplementedException();
    }
    throw new ArgumentNullException();
}
    }
}