using System.Linq;
using System.Text;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public interface IMessageRepository
    {
        int Add(Message message);
        bool Delete(int Id);
        bool Save(Message message);
        IQueryable<Message> Get();
    }
}
