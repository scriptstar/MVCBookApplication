using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    public class MessageController : Controller
    {
        public MessageController()
            : this(null, null)
        {
        }

        [Inject]
        public MessageController(IMessageService service, ITemplateService templateService)
        {
            TemplateService = templateService ?? new InMemoryTemplateService();
            Service = service ?? new InMemoryMessageService();
        }

        public ITemplateService TemplateService { get; set; }
        public IMessageService Service { get; set; }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult Create()
        {
            ViewData["Title"] = "Create New Message";
            ViewData["templates"] = TemplateService.Get();
            ViewData["mytemplates"] = TemplateService.Get(User.Identity.Name);
            return View(new Message());
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Create([Bind(Prefix = "", Exclude = "Id")]  Message model)
        {
            ViewData["Title"] = "Create New Message";
            ViewData.Model = model;
            try
            {
                var id = Service.Add(model);
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.ValidationErrors)
                {
                    ViewData.ModelState.AddModelError(
                        error.PropertyName,
                        error.ErrorMessage);
                }
            }

            if (!ViewData.ModelState.IsValid)
            {
                ViewData["templates"] = TemplateService.Get();
                ViewData["mytemplates"] = TemplateService.Get(User.Identity.Name);
                return View(model);
            }

            TempData["flash"] = "Message successfully created";
            return RedirectToAction("browse");
        }


        [AcceptVerbs(HttpVerbs.Get), Authorize]
        public ActionResult Browse(int? page)
        {
            ViewData["Title"] = "Browse Messages";

            page = page ?? 1;
            ViewData.Model = Service.GetPage(User.Identity.Name, page);
            return View("browse");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        [HandleError]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Message";
            ViewData.Model = Service.Get(User.Identity.Name, id);
            if (ViewData.Model == null)
            {
                TempData["error"] = "The message you requested could not be found";
                throw new ArgumentException();
            }
            return View("edit");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize, HandleError, ValidateInput(false)]
        public ActionResult Edit([Bind(Prefix = "")] Message message)
        {
            ViewData["Title"] = "Edit Message";
            Service.Save(User.Identity.Name, message);
            TempData["flash"] = "Message successfully saved";
            return RedirectToAction("browse");
        }

        //[AcceptVerbs("post")]
        //[Authorize]
        //public ActionResult Create([Bind(Prefix = "", Exclude = "Id")] Message model)
        //{
        //    ViewData["Title"] = "New Message";

        //    try
        //    {
        //        var id = Service.Add(model);
        //    }
        //    catch (ValidationException ex)
        //    {
        //        foreach (var error in ex.ValidationErrors)
        //        {
        //            ViewData.ModelState.AddModelError(
        //                error.PropertyName,
        //                error.ErrorMessage);
        //        }
        //    }

        //    if (!ViewData.ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    return View();
        //}

    }
}