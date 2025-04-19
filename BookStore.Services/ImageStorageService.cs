using BookStore.Core.Exceptions;
using BookStore.Core.Strategies;
using Microsoft.AspNetCore.Http;
using MimeKit;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class ImageStorageService(IImageStorageStrategy storageStrategy) : IImageStorageService
    {
        private readonly IImageStorageStrategy _storageStrategy = storageStrategy;
        private static readonly Dictionary<string, string> MimeTypeToFileExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "image/jpeg", ".jpg" },
            { "image/png", ".png" }
        };

        public async Task<string> UploadAsync(IFormFile file)
        {
            using var imageStream = file.OpenReadStream();
            String contentType = file.ContentType.ToLower();
            if(!IsImageValid(imageStream, contentType)) throw new InvalidImageFormatException("The uploaded image format is not supported.");
            String fileName = GenerateFilePath(contentType);
            await _storageStrategy.UploadAsync(fileName, imageStream);
            return fileName;
        }
        private static string GenerateFilePath(String contentType)
        {
            if (!MimeTypeToFileExtensions.TryGetValue(contentType, out var extensions)) {
                throw new ArgumentException("No valid file extension found for the provided content type.", nameof(contentType));
            }

            var now = DateTime.Now;
            string datePart = now.ToString("yyyyMMdd_HHmmss");
            var randomStr = Path.GetRandomFileName()[..4].ToUpper();
            return $"{datePart}_{randomStr}{extensions}";
        }

        private static bool IsImageValid(Stream imageStream, string contentType)
        {
            byte[] buffer = new byte[8];
            imageStream.Read(buffer, 0, 8);
            imageStream.Position = 0;

            if (contentType == "image/jpeg" && IsJpegByMagic(buffer))
                return true;
            if (contentType == "image/png" && IsPngByMagic(buffer))
                return true;

            return false;
        }

        private static bool IsJpegByMagic(byte[] buffer)
        {
            // JPEG 魔術字節是 0xFF 0xD8 0xFF 0xE0 或 0xFF 0xD8 0xFF 0xE1
            return buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF &&
                   (buffer[3] == 0xE0 || buffer[3] == 0xE1);
        }

        private static bool IsPngByMagic(byte[] buffer)
        {
            // PNG 魔術字節是 0x89 0x50 0x4E 0x47 0x0D 0x0A 0x1A 0x0A
            return buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                   buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
        }
    }
}
