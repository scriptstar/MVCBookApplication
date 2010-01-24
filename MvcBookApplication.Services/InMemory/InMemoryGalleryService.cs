using System.Collections.Generic;
using System.IO;
using MvcBookApplication.Data.InMemory;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;
using Ninject.Core;

namespace MvcBookApplication.Services
{
    public class InMemoryGalleryService : IGalleryService
    {
        private IGalleryRepository Repository { get; set; }

        public InMemoryGalleryService()
            : this(null)
        {
        }

        [Inject]
        public InMemoryGalleryService(IGalleryRepository repository)
        {
            Repository = repository ?? new InMemoryGalleryRepository();
        }

        public int Upload(string username, string filename, Stream stream)
        {
            return Repository.Upload(username, filename, stream);
        }

        public List<GalleryFile> GetAllImages(string username)
        {
            return Repository.GetAllImages(username);
        }

        public byte[] GetImageBytes(string username, int id)
        {
            return Repository.GetImageBytes(id);
        }
    }
}