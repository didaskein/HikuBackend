using Azure;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hiku.Services.Service.BlobStorage
{
    public interface IBlobStorage
    {
        Task<Uri> CreateBlockBlobAsync(string blobId, Stream stream, string contentType);
        Task<Uri> CreateBlockBlobAsync(string blobId, string filePath);
        Task DeleteBlobAsync(string blobId);
        Task DeleteBlobContainerAsync();
        Task<Response<BlobProperties>> GetBlobPropertiesAsync(string blobId);
        Task<Stream> GetBlockBlobDataAsStreamAsync(string blobId);
        Task<string> GetBlockBlobDataAsStringAsync(string blobId);
        Pageable<BlobItem> ListBlobsInContainer();
    }
}