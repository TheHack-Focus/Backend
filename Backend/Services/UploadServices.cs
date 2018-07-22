using Backend.DataObjects;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class UploadServices
    {
        private IOptions<StorageAccountConfig> _storageConfig;
        private CloudStorageAccount _azStorageAcct;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _fileContainer;

        public UploadServices(IOptions<StorageAccountConfig> storageConfig)
        {
            _storageConfig = storageConfig;
            _azStorageAcct = CloudStorageAccount.Parse(_storageConfig.Value.AccessString);
            _blobClient = _azStorageAcct.CreateCloudBlobClient();
            _fileContainer = _blobClient.GetContainerReference("ugcfiles");
        }

        public async Task<Guid> UploadBlobAsync(Stream fileStream, string contentType)
        {
            var fileId = Guid.NewGuid();

            await _fileContainer.CreateIfNotExistsAsync();
            var fileReference = _fileContainer.GetBlockBlobReference(fileId.ToString());
            await fileReference.UploadFromStreamAsync(fileStream);

            fileReference.Properties.ContentType = contentType;
            await fileReference.SetPropertiesAsync();

            return fileId;
        }


        private static string MapContentTypeToExtension(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                case "image/jpg":
                    return ".jpg";
                case "image/gif":
                    return ".gif";
                case "image/png":
                    return ".png";
                default:
                    return string.Empty;
            }
        }

    }
}
