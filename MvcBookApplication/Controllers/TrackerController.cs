using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    public class TrackerController : Controller
    {
        public IMessageAuditService MessageAuditService { get; set; }

        public TrackerController()
            : this(null)
        {
        }

        [Inject]
        public TrackerController(IMessageAuditService messageAuditService)
        {
            MessageAuditService = messageAuditService ?? new InMemoryMessageAuditService();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DynamicImage(MessageAudit messageAudit)
        {
            if (messageAudit != null &&
                !string.IsNullOrEmpty(messageAudit.Email) &&
                messageAudit.MessageId > 0)
            {
                messageAudit.Action = "View";
                MessageAuditService.Add(messageAudit);

            }

            var content = System.IO.File.ReadAllBytes
                                (Server.MapPath("~/content/tracker.jpg"));
            return File(content, "image/jpeg", "tracker.jpg");
        }

[AcceptVerbs(HttpVerbs.Get)]
public ActionResult Link(MessageAudit messageAudit)
{
    if (messageAudit == null || string.IsNullOrEmpty(messageAudit.Url))
    {
        return RedirectToAction("index", "home");
    }
    if(messageAudit.MessageId > 0)
    {
        messageAudit.Action = "Click";
        MessageAuditService.Add(messageAudit);
    }
    
    return Redirect(messageAudit.Url);
}
    }
}