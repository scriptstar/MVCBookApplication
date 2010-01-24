using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.Web.Security;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    public class ContactListController : Controller
    {
        #region Construction Logic

        public IContactListService Service { get; set; }
        public ContactListController()
            : this(null, null)
        {
        }

        [Inject]
        public ContactListController(IContactListService service, MembershipProvider provider)
        {
            Service = service;
            Provider = provider;
        }
        public MembershipProvider Provider { get; set; }
        public Guid UserId
        {
            get
            {
                return (Guid)Provider.
                                 GetUser(User.Identity.Name, false)
                                 .ProviderUserKey;
            }
        }

        #endregion


        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult Create()
        {
            ViewData["Title"] = "New Contact List";
            return View("create", new ContactList());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Create([Bind(Prefix = "", Exclude = "Id")] ContactList model)
        {
            ViewData["Title"] = "New Contact List";
            int id=0;
            try
            {
                model.User = new User
                {
                    Username = User.Identity.Name,
                    UserId = UserId
                };
                id = Service.Add(model);
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
                return View("create", model);
            }
            TempData["flash"] = "Contact list successfully created";
            return RedirectToAction("addcontacts",new RouteValueDictionary{{"id",id}});
        }


        [AcceptVerbs(HttpVerbs.Get), Authorize]
        public ActionResult Browse(int? page, string sortBy, string sortDir)
        {
            ViewData["Title"] = "Browse Contact Lists";
            ViewData["sortdir"] = sortDir;
            ViewData["sortby"] = sortBy;

            page = page ?? 1;
            ViewData.Model = Service.GetPage(UserId,
                                             page, sortBy, sortDir);
            return View("browse");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        [HandleError]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit Contact List";
            ViewData.Model = Service.Get(UserId, id);
            if (ViewData.Model == null)
            {
                TempData["error"] = "The contact list you requested could not be found";
                throw new ArgumentException();
            }
            return View("edit");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize]
        [HandleError]
        public ActionResult Edit(int id, [Bind(Prefix = "")] ContactList model)
        {
            ViewData["Title"] = "Edit Contact List";
            try
            {
                model.Id = id;
                if (!Service.Save(UserId, model))
                {
                    ViewData.ModelState.AddModelError("save",
                        "Error saving the contact list");
                }
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
                return View("edit", model);
            }
            TempData["flash"] = "Contact list successfully saved";
            return RedirectToAction("browse");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        [HandleError]

        public ActionResult Delete(int id)
        {
            ViewData["title"] = "Delete Contact List";
            ViewData.Model = Service.Get(UserId, id);
            if (ViewData.Model == null)
            {
                TempData["error"] = "The contact list you are trying to delete does not exist";
                throw new ArgumentException();
            }
            return View("delete");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize, ActionName("Delete")]
        [HandleError]
        public ActionResult DeleteSubmit(int id)
        {
            if (!Service.Delete(UserId, id))
            {
                TempData["error"] = "Error deleting the contact list";
                throw new ArgumentException();
            }
            TempData["flash"] = "Contact list successfully deleted";
            return RedirectToAction("browse");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AddContacts(int id, int? page, string sortBy, string sortDir)
        {
            ViewData["Title"] = "Manage Contact List";
            var contactList = Service.Get(UserId, id);
            var contactLists = Service.GetPage(UserId,page,sortBy,sortDir);
            ViewData["contactlists"] = contactLists.Where(l=> l.Id != id).ToList();
            ViewData.Model = contactList;
            return View();
        }

    }
}
