using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Linq;
using MvcBookApplication.Services;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    [HandleError]
    [OutputCache(Location = OutputCacheLocation.None)]
    public class PayController : Controller
    {
        public IPaymentService PaymentService { get; set; }

        public PayController()
            : this(null)
        {
        }

        [Inject]
        public PayController(IPaymentService paymentService)
        {
            PaymentService = paymentService;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Callback()
        {
            PaymentService.PerformHandShake(Request);
            PaymentService.ProcessPayment(Request.Form);
            return null;
        }

[AcceptVerbs(HttpVerbs.Get)]
public ActionResult Subscribe()
{
    ViewData["Title"] = "Subscribe";
    return View("subscribe");
}
    }

}