using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class AttachmentServices : IAttachmentServices
    {
        private readonly long maxFileSize = 5* 1024 * 1024;
        private readonly string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
        private readonly ILogger<AttachmentServices> logger;
        private readonly IWebHostEnvironment env;

        public AttachmentServices(ILogger<AttachmentServices> logger, IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }


        public bool Delete(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

            try
            {
                var fullPath = Path.Combine(env.ContentRootPath, fileName);

                if(!File.Exists(fullPath)) {return false;}

                File.Delete(fullPath);
                return true;

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete file.");
                return false;
                
            }


        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || !fileStream.CanRead) return null;

            if (fileStream.Length > maxFileSize)
            {
                logger.LogWarning("Rejected: File is too large");
                return null;
            }

            var extension = Path.GetExtension(fileName);

            if(string.IsNullOrEmpty(extension)|| !allowedExtensions.Contains(extension))
            {
                logger.LogWarning("Rejected: File type not allowed");
                return null;
            }

            var UploadedFolder = Path.Combine(env.ContentRootPath, folderName);

            Directory.CreateDirectory(UploadedFolder);

            var storedFileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(UploadedFolder, storedFileName);

            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew,FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(fs);
                return storedFileName;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to upload file.");
                return null;
            }


        }
    }
}
