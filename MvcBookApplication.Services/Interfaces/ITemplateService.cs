using System.Collections.Generic;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public  interface ITemplateService
    {
        IList<Template> Get();
        IList<Template> Get(string username);
        int Save(string username, string name, string content);
    }
}