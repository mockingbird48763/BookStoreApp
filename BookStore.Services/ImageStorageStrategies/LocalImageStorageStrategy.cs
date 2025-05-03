using BookStore.Core.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BookStore.Services.FileStorageStrategies
{
    public class LocalImageStorageStrategy : IImageStorageStrategy
    {
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public async Task UploadAsync(string fileName, Stream fileStream)
        {
            var filePath = Path.Combine(_uploadsFolder, fileName);

            try {
                // 將 stream 寫入檔案
                using var output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(output);
            } catch (IOException) {
                throw new IOException("Failed to save the file on the server.");
            }
        }
    }
}
