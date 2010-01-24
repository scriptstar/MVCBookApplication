using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
public interface IMessageAuditService
{
    int Add(MessageAudit messageAudit);
}
}