using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MvcBookApplication.Data.Interfaces;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data.InMemory
{
public class InMemoryGalleryRepository : IGalleryRepository
{
    private List<GalleryFile> GalleryFiles { get; set; }

    private Dictionary<int, byte[]> Files { get; set; }

    public InMemoryGalleryRepository()
    {
        GalleryFiles = new List<GalleryFile>();
        Files = new Dictionary<int, byte[]>();
    }


    private int _autoId;
    private int AutoId
    {
        get
        {
            _autoId += 1;
            return _autoId;
        }
    }

    public int Upload(string username, string filename, Stream stream)
    {
        var galleryFile = new GalleryFile
                              {
                                  Id = AutoId,
                                  Username = username,
                                  OriginalFilename = filename,
                                  Filename = Guid.NewGuid().ToString() +
                                  Path.GetExtension(filename)
                              };
        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        Files.Add(galleryFile.Id, bytes);
        GalleryFiles.Add(galleryFile);
        return galleryFile.Id;
    }

    public List<GalleryFile> GetAllImages(string username)
    {
        return GalleryFiles.Where(f => f.Username == username).ToList();
    }

    public byte[] GetImageBytes(int id)
    {
        return Files[id];
    }
}
}
