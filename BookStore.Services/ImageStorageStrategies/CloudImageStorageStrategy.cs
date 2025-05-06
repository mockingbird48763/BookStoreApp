using BookStore.Core.Strategies;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.ImageStorageStrategies
{
    public class CloudImageStorageStrategy : IImageStorageStrategy
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public CloudImageStorageStrategy()
        {
            _storageClient = StorageClient.Create();

            // 從環境變量讀取桶名稱
            _bucketName = Environment.GetEnvironmentVariable("GCS_BUCKET_NAME")
                          ?? throw new InvalidOperationException("Bucket name is not configured in environment variables.");
        }

        public async Task UploadAsync(string fileName, Stream fileStream)
        {
            var contentType = GetMimeTypeFromFileName(fileName);

            await _storageClient.UploadObjectAsync(_bucketName, fileName, contentType, fileStream);
        }

        private static string GetMimeTypeFromFileName(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
        }
    }
}
