using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data.Interfaces
{
public interface IGalleryRepository
{
    int Upload(string username, string filename, Stream stream);
    List<GalleryFile> GetAllImages(string username);
    byte[] GetImageBytes(int id);
}
}
