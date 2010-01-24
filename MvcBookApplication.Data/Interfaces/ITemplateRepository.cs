using System.Linq;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data.Interfaces
{
    public interface ITemplateRepository
    {
        IQueryable<Template> Get();
        int Save(string username, string name, string content);
    }
}