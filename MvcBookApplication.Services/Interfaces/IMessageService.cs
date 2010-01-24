using System.Collections.Generic;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public interface IMessageService
    {
        int Add(Message message);
        Message GetById(int Id);
        List<Message> Search(string searchTerm);
        PagedList<Message> GetPage(string username, int? page);
        Message Get(string username, int id);
        bool Save(string username, Message message);
    }
}
