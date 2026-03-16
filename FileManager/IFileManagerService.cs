using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.FileManager
{
    public interface IFileManagerService
    {
        Task<DownloadFileResultDto> DowonloadFileAsync(string logoUrl);

        Task<string> SaveStreamAsync(Stream stream, string folderPath, string fileName);
        Task<string> SaveFileAsync(IFormFile file, string folderPath, string? fileName = null);

        Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<IFormFile> files, string folderPath);

        Task<bool> DeleteFileAsync(string relativePath);

        Task<int> DeleteFilesAsync(IEnumerable<string> relativePaths);

        string GetAbsolutePath(string relativePath);

        string GetRelativePath(string folderPath, string fileName);

        bool ValidateFile(IFormFile file, long maxSizeInBytes, IEnumerable<string> allowedExtensions, out string errorMessage);

        bool FileExists(string relativePath);

        string GetContentType(string filePath);
    }
    public class DownloadFileResultDto
    {
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    }
}
