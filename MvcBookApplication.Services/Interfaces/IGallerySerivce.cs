using System.Collections.Generic;
using System.IO;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Services
{
    public interface IGalleryService
    {
        int Upload(string username, string filename, Stream stream);
        List<GalleryFile> GetAllImages(string username);

        byte[] GetImageBytes(string username, int id);
    }
}