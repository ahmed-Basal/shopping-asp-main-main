using core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    public class Imagemangemt : IIamgeServices
    {
        private readonly IFileProvider _fileProvider;
        public Imagemangemt(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        public async Task<List<string>> AddImageAsync(IFormFile file, string src)
        {
            var saveImageSrc = new List<string>();

            if (file == null || file.Length == 0)
                return saveImageSrc;

            if (string.IsNullOrEmpty(src))
                src = "DefaultFolder";

            var wwwrootPath = (_fileProvider as PhysicalFileProvider)?.Root ?? "wwwroot";
            var imageDirectory = Path.Combine(wwwrootPath, "Images", src);
            if (!Directory.Exists(imageDirectory))
                Directory.CreateDirectory(imageDirectory);

            var imageName = file.FileName;
            var imageSrc = $"/Images/{src}/{imageName}";
            var root = Path.Combine(imageDirectory, imageName);

            using var stream = new FileStream(root, FileMode.Create);
            await file.CopyToAsync(stream);

            saveImageSrc.Add(imageSrc);

            return saveImageSrc;
        }


        public void DeleteImageAsync(string src)
        {
            var info = _fileProvider.GetFileInfo(src);

            var root = info.PhysicalPath;
            File.Delete(root);
        }




    }
}
