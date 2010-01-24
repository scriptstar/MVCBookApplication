using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Gallio.Framework;
using MbUnit.Framework;
using Moq;
using MvcBookApplication.Controllers;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using MvcBookApplication.Services;
using MvcBookApplication.Tests.Properties;

namespace MvcBookApplication.Tests.Controllers
{
    [TestFixture]
    public class GalleryControllerTests
    {
        private string username = "test";

        [Test]
        public void upload_should_save_uploaded_file()
        {
            //mock file
            var filename = "fish.jpg";
            var file1 = new Mock<HttpPostedFileBase>();
            file1.Expect(d => d.FileName).Returns(filename);
            file1.Expect(d => d.ContentLength).Returns(1);

            var ms = new MemoryStream();
            Resources.Fish.Save(ms, ImageFormat.Jpeg);
            file1.Expect(d => d.InputStream).Returns(ms);
            MyMocks.Request.Expect(r => r.Files.Count).Returns(1);
            MyMocks.Request.Expect(r => r.Files[0]).Returns(file1.Object);

            //mock service
            var mockService = new Mock<IGalleryService>();
            mockService.Expect(s => s.Upload(username, filename,
                                             ms)).Returns(0);

            var con = new GalleryController(mockService.Object);
            con.SetFakeControllerContext();

            var result = con.Upload();

            result.AssertViewResult(con, null, "Uploader");

            //verify mocks
            MyMocks.Request.VerifyAll();
            file1.VerifyAll();
            mockService.VerifyAll();
        }

        [Test]
        public void upload_should_return_error_if_file_is_missing()
        {
            //mock file
            var file1 = new Mock<HttpPostedFileBase>();
            MyMocks.Request.Expect(r => r.Files.Count).Returns(0);

            //mock service
            var mockService = new Mock<IGalleryService>();

            var con = new GalleryController(mockService.Object);
            con.SetFakeControllerContext();

            var result = con.Upload();

            result.AssertViewResult(con, null, "Uploader");
            con.ModelState.AssertErrorMessage("imageuploader", "No files to upload");

            //verify mocks
            MyMocks.Request.VerifyAll();
            file1.VerifyAll();
            mockService.VerifyAll();
        }

        [Test]
        public void getallimages_gets_list_of_uploaded_images()
        {
            var mockService = new Mock<IGalleryService>();
            mockService.Expect(s => s.GetAllImages(username))
                .Returns(GetFakeListOfImages());

            var con = new GalleryController(mockService.Object);
            con.SetFakeControllerContext();

            var result = con.GetAllImages();

            Assert.IsInstanceOfType(typeof(JsonResult), result);
            var jsonResult = result as JsonResult;
            Assert.IsInstanceOfType(typeof(IList), jsonResult.Data);
            Assert.AreEqual(3, ((IList)jsonResult.Data).Count);
        }

        [Test]
        public void getimage_returns_image_as_a_file()
        {
            var ms = new MemoryStream();
            Resources.Fish.Save(ms, ImageFormat.Jpeg);
            var bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);

            var mockService = new Mock<IGalleryService>();
            mockService.Expect(s => s.GetImageBytes(username, 1))
                .Returns(bytes);

            var con = new GalleryController(mockService.Object);
            con.SetFakeControllerContext();

            var result = con.GetImage(1);

            Assert.IsInstanceOfType(typeof(FileResult), result);
            var fileResult = result as FileResult;
            Assert.AreEqual("image/jpeg", fileResult.ContentType);
        }

        private List<GalleryFile> GetFakeListOfImages()
        {
            var images = new List<GalleryFile>();
            images.Add(new GalleryFile { Id = 1 });
            images.Add(new GalleryFile { Id = 2 });
            images.Add(new GalleryFile { Id = 3 });
            return images;
        }
    }
}