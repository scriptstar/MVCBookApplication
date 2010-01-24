using System.Web;

namespace MvcBookApplication.Services
{
    public interface IParserFactory
    {
        IParser Create(string contacts, HttpPostedFileBase file);
    }
}