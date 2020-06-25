using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure;
using Azure.Storage.Sas;
using System.Runtime.CompilerServices;
using Azure.Storage;

namespace Hiku.Services.Service.BlobStorage
{
    public class BlobStorage : IBlobStorage, IAudioBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string _blobContainerName;

        public BlobStorage(string connectionString, string blobContainerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            _blobContainerClient.CreateIfNotExists();
            _blobContainerName = blobContainerName;
        }


        public async Task<Uri> CreateBlockBlobAsync(string blobId, Stream stream, string contentType)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }).ConfigureAwait(false);
            return blobClient.Uri;
        }

        public async Task<Uri> CreateBlockBlobAsync(string blobId, string filePath)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            await blobClient.UploadAsync(filePath).ConfigureAwait(false);
            return blobClient.Uri;
        }

        public async Task<Response<BlobProperties>> GetBlobPropertiesAsync(string blobId)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            return await blobClient.GetPropertiesAsync().ConfigureAwait(false);
        }


        public async Task<string> GetBlockBlobDataAsStringAsync(string blobId)
        {
            string text = string.Empty;

            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            BlobDownloadInfo download = await blobClient.DownloadAsync().ConfigureAwait(false);
            using (StreamReader reader = new StreamReader(download.Content))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }

        public async Task<Stream> GetBlockBlobDataAsStreamAsync(string blobId)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            BlobDownloadInfo download = await blobClient.DownloadAsync().ConfigureAwait(false);
            return download.Content;
        }



        public Pageable<BlobItem> ListBlobsInContainer()
        {
            return _blobContainerClient.GetBlobs();
        }

        public async Task DeleteBlobAsync(string blobId)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobId);
            await blobClient.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        public async Task DeleteBlobContainerAsync()
        {
            await _blobContainerClient.DeleteIfExistsAsync().ConfigureAwait(false);
        }

        //public string GetBlobSasUri(string blobName, DateTimeOffset accessExpiryTime)
        //{
        //    BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

        //    //SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
        //    //{
        //    //    SharedAccessExpiryTime = accessExpiryTime,
        //    //    Permissions = permissions
        //    //};

        //    //blobClient.get
        //    //string sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);
        //    //return blob.Uri + sasBlobToken;


        //    TimeSpan clockSkew = TimeSpan.FromMinutes(15d);
        //    TimeSpan accessDuration = TimeSpan.FromMinutes(15d);

        //    //  Defines the resource being accessed and for how long the access is allowed.
        //    var blobSasBuilder = new BlobSasBuilder
        //    {
        //        StartsOn = DateTime.UtcNow.Subtract(clockSkew),
        //        ExpiresOn = DateTime.UtcNow.Add(accessDuration) + clockSkew,
        //        BlobContainerName = _blobContainerName,
        //        BlobName = blobName,
        //    };

        //    //  Defines the type of permission.
        //    blobSasBuilder.SetPermissions(BlobSasPermissions.Write);

        //    //  Builds an instance of StorageSharedKeyCredential      
        //    var storageSharedKeyCredential = new StorageSharedKeyCredential(< AccountName >, < AccountKey >);

        //    //  Builds the Sas URI.
        //    BlobSasQueryParameters sasQueryParameters = blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential);
        //    return sasQueryParameters.ToString();
        //}



    }
}
