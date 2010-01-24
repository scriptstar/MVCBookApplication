using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public interface IMessageAuditRepository
    {
        int Add(MessageAudit messageAudit);
    }
}