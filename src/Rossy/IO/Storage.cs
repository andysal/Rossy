using Azure.Storage.Blobs;
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


        public string UploadFile(Stream file)
        {
            var token = Guid.NewGuid();
            string blobName = token.ToString();

            var container = new BlobContainerClient(Config.ConnectionString, Config.ContainerName);
            var blob = container.GetBlobClient(blobName);
            blob.Upload(file);

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
    }
}
