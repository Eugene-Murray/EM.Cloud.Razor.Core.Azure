using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Microsoft.Extensions.Configuration;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;



namespace RazorCore.Pages
{
    public class BlobStorageModel : PageModel
    {
        public List<string> blobs { get; set; }

        public void OnGet()
        {
            //CloudBlobContainer container = GetCloudBlobContainer();
            //var blogCloudBlob = container.GetBlobReference("blog");

            this.blobs = GetBlobs().GetAwaiter().GetResult();
        }

        public async Task<List<string>> GetBlobs()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    "emcloud",
                    "HBR/W2YAagG4IKfD9owR9iY9oyok1hgGOepCgRmqSmJi6YMWMH18wICD+z5Xgd2i+6FPLXN6InJ/1MojBMYL9g=="), true);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named "mycontainer."
            CloudBlobContainer container = blobClient.GetContainerReference("blog");
            List<string> blobs = new List<string>();
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;

                
                foreach (IListBlobItem item in resultSegment.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        blobs.Add(blob.Name);
                    }

                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        CloudPageBlob pageBlob = (CloudPageBlob)item;

                        blobs.Add(pageBlob.Name);
                    }

                    else if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;

                        blobs.Add(directory.Uri.ToString());
                    }
                }

            } while (token != null);

            return blobs;
        }

        //public void ListBlobs()
        //{
        //    CloudBlobContainer container = GetCloudBlobContainer();
        //    List<string> blobs = new List<string>();
        //    foreach (IListBlobItem item in container.ListBlobsSegmentedAsync(useFlatBlobListing: true))
        //    {
        //        if (item.GetType() == typeof(CloudBlockBlob))
        //        {
        //            CloudBlockBlob blob = (CloudBlockBlob)item;
        //            blobs.Add(blob.Name);
        //        }
        //        else if (item.GetType() == typeof(CloudPageBlob))
        //        {
        //            CloudPageBlob blob = (CloudPageBlob)item;
        //            blobs.Add(blob.Name);
        //        }
        //        else if (item.GetType() == typeof(CloudBlobDirectory))
        //        {
        //            CloudBlobDirectory dir = (CloudBlobDirectory)item;
        //            blobs.Add(dir.Uri.ToString());
        //        }
        //    }
        //}

        //private CloudBlobContainer GetCloudBlobContainer()
        //{
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("emcloud_AzureStorageConnectionString"));
        //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        //    CloudBlobContainer container = blobClient.GetContainerReference("blog");
        //    return container;
        //}
    }
}