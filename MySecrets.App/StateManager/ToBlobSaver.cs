using System;
using System.Threading.Tasks;
using MySecrets.App.AzureStorage;
using MySecrets.App.UiRenderer;

namespace MySecrets.App.StateManager
{
    public static class ToBlobSaver
    {
        public static async Task SaveToBlobAsync(this byte[] src, SettingsModel model)
        {

            if (model.BlobConnectionString == null)
                return;

            try
            {

                var blobStorage = new AzureBlobStorage(model.BlobConnectionString);
                var filename = DateTime.UtcNow.ToString("s");
                
                await blobStorage.SaveBlobAsync("secrets", filename, src);

            }
            catch (Exception )
            {
                UiNotifications.Notify(ConsoleColor.Red, ConsoleColor.White, "Did not backup to blob", 5);
            }
            
        } 
    }
}