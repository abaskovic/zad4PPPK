using Azure.Storage.Blobs;
using System;
using System.Configuration;

namespace zad4.Dal
{
    internal class Repository
    {
        private const string ContainerName = "blobcontainer";
        private static readonly string cs = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

        private static readonly Lazy<BlobContainerClient> container = new(
            () => new BlobServiceClient(cs).GetBlobContainerClient(ContainerName)
        );


        public static BlobContainerClient Container = container.Value;


    }
}
