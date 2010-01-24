using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Validation;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    public class ContactController : Controller
    {
        public IContactsImporter ContactsImporter { get; set; }
        public IParserFactory ParserFactory { get; set; }
        public IContactService Service { get; set; }

        [Inject]
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

        public ContactController()
            : this(null, null)
        {
        }

        [Inject]
        public ContactController(IContactService service, IParserFactory parserFactory)
        {
            ParserFactory = parserFactory;
            Service = service ?? new InMemoryContactService();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult Create()
        {
            ViewData["Title"] = "New Contact";
            PopulateSexDropDown();
            return View("create", new Contact());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Create([Bind(Prefix = "", Exclude = "Id")] Contact model)
        {
            ViewData["Title"] = "New Contact";
            PopulateSexDropDown();
            try
            {
                model.User = new User
                             {
                                 Username = User.Identity.Name,
                                 UserId = UserId
                             };
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
                return View("create", model);
            }
            TempData["flash"] = "Contact successfully created";
            return RedirectToAction("create");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        public ActionResult Browse(int? page, string sortBy, string sortDir)
        {
            ViewData["Title"] = "Browse Contacts";
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
            ViewData["Title"] = "Edit Contact";
            ViewData.Model = Service.Get(UserId, id);
            if (ViewData.Model == null)
            {
                TempData["error"] = "The contact you requested could not be found";
                throw new ArgumentException();
            }
            return View("edit");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize]
        [HandleError]
        public ActionResult Edit(int id, [Bind(Prefix = "")] Contact model)
        {
            ViewData["Title"] = "Edit Contact";
            try
            {
                model.Id = id;
                if (!Service.Save(UserId, model))
                {
                    ViewData.ModelState.AddModelError("save",
                        "Error saving the contact");
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
            TempData["flash"] = "Contact successfully saved";
            return RedirectToAction("browse");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        [HandleError]
        public ActionResult Delete(int id)
        {
            ViewData["title"] = "Delete Contact";
            ViewData.Model = Service.Get(UserId, id);
            if (ViewData.Model == null)
            {
                TempData["error"] = "The contact you are trying to delete does not exist";
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
                TempData["error"] = "Error deleting the contact";
                throw new ArgumentException();
            }
            TempData["flash"] = "Contact successfully deleted";
            return RedirectToAction("browse");
        }


        private void PopulateSexDropDown()
        {
            ViewData["sex"] = new SelectList(new Dictionary<string, object>
                                                 {
                                                     {"Undefined"," "},
                                                     {"Male", "Male"},
                                                     {"Female", "Female"}
                                                 }, "key", "value");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        public ActionResult Import()
        {
            ViewData["Title"] = "Import Contacts";
            return View("import");
        }

        [AcceptVerbs(HttpVerbs.Post), Authorize]
public ActionResult Import(string contacts)
{
    ViewData["Title"] = "Import Contacts";
    if (string.IsNullOrEmpty(contacts) &&
        (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0))
    {
        ViewData.ModelState.AddModelError("Import",
                                          "You must enter some contacts or upload a file");
    }
    if (!ViewData.ModelState.IsValid)
    {
        return View("import");
    }

    var file = Request.Files.Count == 0 ? null : Request.Files[0];
    var parser = ParserFactory.Create(contacts, file);
    var parsedContacts = parser.Parse();

    Service.Import(new User
                       {
                           UserId = UserId,
                           Username = User.Identity.Name
                       }, parsedContacts);

    TempData["flash"] = "Contacts successfully imported";
    return RedirectToAction("browse");
}
    }


}
