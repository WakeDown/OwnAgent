using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Azure;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;

namespace Data.Objects
{
    public class AzureStorageClient
    {
        //private CloudStorageAccount StorageAccount;
        //private CloudBlobClient BlobClient;

        //public AzureStorageClient()
        //{
        //    StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("Un1tDeskStorageConnectionString"));
        //    BlobClient = StorageAccount.CreateCloudBlobClient();
        //}

        //public static AzureStorageClient Initalize()
        //{
        //    return new AzureStorageClient();
        //}

        //public CloudBlobContainer GetWishFilesContainer()
        //{
        //    string catalog = ConfigurationManager.AppSettings["Un1tDeskStorageWishFilesContainer"];
        //    return BlobClient.GetContainerReference(catalog);
        //}
    }
}
