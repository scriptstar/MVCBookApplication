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
    public class TemplateController : Controller
    {
        public ITemplateService Service { get; set; }

        public TemplateController()
            : this(null)
        {

        }

        [Inject]
        public TemplateController(ITemplateService service)
        {
            Service = service;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult List(string username)
        {
            if (string.IsNullOrEmpty(username))
                return View("list", Service.Get());
            else
                return View("list", Service.Get(username));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Save(string name, string content)
        {
            var id = Service.Save(User.Identity.Name, name,content);
            return Json(new JsonData()
                            {
                                success = true,
                                id = id
                            });
        }


    }
}
