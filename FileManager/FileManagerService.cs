using HappyTools.DependencyInjection.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.FileManager
{
    public class FileManagerService : IFileManagerService, IScopedDependency
    {
        private readonly IWebHostEnvironment _env;

        public FileManagerService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderPath, string? fileName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var extension = Path.GetExtension(file.FileName);
            fileName ??= $"{Guid.NewGuid()}{extension}";

            var fullFolder = Path.Combine(_env.WebRootPath, folderPath);
            Directory.CreateDirectory(fullFolder);

            var fullPath = Path.Combine(fullFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        public async Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<IFormFile> files, string folderPath)
        {
            var paths = new List<string>();
            foreach (var file in files)
            {
                var path = await SaveFileAsync(file, folderPath);
                paths.Add(path);
            }
            return paths;
        }

        public async Task<bool> DeleteFileAsync(string relativePath)
        {
            var fullPath = GetAbsolutePath(relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task<int> DeleteFilesAsync(IEnumerable<string> relativePaths)
        {
            int deleted = 0;
            foreach (var path in relativePaths)
            {
                if (await DeleteFileAsync(path))
                    deleted++;
            }
            return deleted;
        }

        public string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        }

        public string GetRelativePath(string folderPath, string fileName)
        {
            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        public bool ValidateFile(IFormFile file, long maxSizeInBytes, IEnumerable<string> allowedExtensions, out string errorMessage)
        {
            errorMessage = "";

            if (file == null || file.Length == 0)
            {
                errorMessage = "File is empty";
                return false;
            }

            if (file.Length > maxSizeInBytes)
            {
                errorMessage = $"File size exceeds {maxSizeInBytes / 1024 / 1024} MB";
                return false;
            }

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                errorMessage = $"Invalid file extension. Allowed: {string.Join(", ", allowedExtensions)}";
                return false;
            }

            return true;
        }

        public bool FileExists(string relativePath)
        {
            var absolutePath = GetAbsolutePath(relativePath);

            return File.Exists(absolutePath);
        }

        public string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        public async Task<string> SaveStreamAsync(Stream stream, string folderPath, string fileName)
        {
            if (stream == null || stream.Length == 0)
                throw new ArgumentException("Stream is empty");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required");

            var fullFolder = Path.Combine(_env.WebRootPath, folderPath);
            Directory.CreateDirectory(fullFolder);

            var fullPath = Path.Combine(fullFolder, fileName);

            stream.Position = 0;

            using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);

            return Path.Combine(folderPath, fileName).Replace("\\", "/");
        }

        public async Task<DownloadFileResultDto> DowonloadFileAsync(string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl))
                return null;

            if (!FileExists(logoUrl))
                return null;

            var absoulutePath = GetAbsolutePath(logoUrl);
            var fileBytes = await System.IO.File.ReadAllBytesAsync(absoulutePath);

            return new DownloadFileResultDto
            {
                FileName = Path.GetFileName(absoulutePath),
                ContentType = GetContentType(absoulutePath),
                FileBytes = fileBytes
            };
        }
    }

}
