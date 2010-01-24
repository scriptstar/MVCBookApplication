using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcBookApplication.Services;
using Ninject.Core;

namespace MvcBookApplication.Controllers
{
    public class GalleryController : Controller
    {
        public IGalleryService Service { get; set; }

        [Inject]
        public GalleryController(IGalleryService service)
        {
            Service = service;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult Uploader()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Upload()
        {
            if (Request.Files.Count == 0 || Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("imageuploader", "No files to upload");
                return View("Uploader");
            }

            var filename = Request.Files[0].FileName;
            var stream = Request.Files[0].InputStream;
            TempData["UploadedFiled"] = Service.Upload(User.Identity.Name,
                                                       filename, stream);
            return View("Uploader");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult GetAllImages()
        {
            var images = Service.GetAllImages(User.Identity.Name);
            return Json((from img in images select new { id = img.Id }).ToList());
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetImage(int id)
        {
            var img = Service.GetImageBytes(User.Identity.Name, id);
            return File(img, "image/jpeg");
        }
    }
}
