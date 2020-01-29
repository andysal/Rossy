using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rossy.IO
{
    public class Storage
    {
        public class Configuration
        {
            public string ConnectionString { get; set; }
            public string ContainerName { get; set; }
        }
        public Configuration Config { get; private set; }

        public Storage(Configuration configuration)
        {
            Config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public string UploadFile(Stream file, string extension)
        {
            var token = Guid.NewGuid();
            string blobName = $"{token}{extension}";

            var container = new BlobContainerClient(Config.ConnectionString, Config.ContainerName);
            var blob = container.GetBlobClient(blobName);
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = GetMimeType(extension);
            blob.Upload(file, blobHttpHeader);

            var blobUrl = blob.Uri.AbsoluteUri;

            return blobUrl;
        }

        public void DeleteFile(Uri uri)
        {
            var builder = new BlobUriBuilder(uri);
            var blobName = builder.BlobName;
            DeleteFile(blobName);
        }

        public void DeleteFile(string blobName)
        {
            var container = new BlobContainerClient(Config.ConnectionString, Config.ContainerName);
            container.DeleteBlobIfExists(blobName);
        }

        private string GetMimeType(string extension)
        {
            switch(extension)
            {
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                case ".jfif":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
