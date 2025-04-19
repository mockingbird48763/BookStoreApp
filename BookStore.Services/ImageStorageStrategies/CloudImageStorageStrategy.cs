using BookStore.Core.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.FileStorageStrategies
{
    public class CloudImageStorageStrategy : IImageStorageStrategy
    {
        public Task UploadAsync(string fileName, Stream fileStream)
        {
            throw new NotImplementedException();
        }
    }
}
