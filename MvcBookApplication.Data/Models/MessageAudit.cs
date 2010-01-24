using System;

namespace MvcBookApplication.Data.Models
{
public class MessageAudit
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string Email { get; set; }
    public string Action { get; set; }
    public string Url { get; set; }
    public DateTime CreatedOn { get; set; }
}
}