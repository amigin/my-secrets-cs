using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MySecrets.App.AzureStorage
{
    public class AzureBlobStorage
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobStorage(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();

        }



        public Task SaveBlobAsync(string container, string key, Stream bloblStream)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            containerRef.CreateIfNotExistsAsync();

            var blockBlob = containerRef.GetBlockBlobReference(key);

            bloblStream.Position = 0;
            return blockBlob.UploadFromStreamAsync(bloblStream);

        }

        public Task SaveBlobAsync(string container, string key, byte[] blobData)
        {
            var containerRef = _blobClient.GetContainerReference(container);
            containerRef.CreateIfNotExistsAsync();

            var blockBlob = containerRef.GetBlockBlobReference(key);

            return blockBlob.UploadFromByteArrayAsync(blobData, 0, blobData.Length);
        }

        public async Task<byte[]> GetAsBytes(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);

            var blockBlob = containerRef.GetBlockBlobReference(key);
            var ms = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(ms);
            ms.Position = 0;
            return ms.ToArray();
        }


        public async Task<Stream> GetAsync(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);

            var blockBlob = containerRef.GetBlockBlobReference(key);
            var ms = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(ms);
            ms.Position = 0;
            return ms;
        }

        public Task DelBlobAsync(string blobContainer, string key)
        {
            var containerRef = _blobClient.GetContainerReference(blobContainer);

            var blockBlob = containerRef.GetBlockBlobReference(key);
            return blockBlob.DeleteAsync();
        }

    }
}