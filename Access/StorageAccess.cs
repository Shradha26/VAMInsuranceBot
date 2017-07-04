using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace VAMInsuranceBot.Access
{
    public class StorageAccess
    {
        private static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        private static string buffer = string.Empty;

        public static string GetDateTime(DateTime dateTime)
        {
            string date_time = string.Format("{0:dd-MM-yyyy:HH-mm-ss.ffffff}", dateTime);

            return date_time;
        }

        public static void StoreTemporaryLog(string dateTime, string sender = null, string message = null, string panNo = null)
        {
            CloudBlobContainer container = blobClient.GetContainerReference("temporary-logs");
            container.CreateIfNotExists();

            CloudAppendBlob appendBlob = container.GetAppendBlobReference(string.Format("temporary-log-date-{0}.log", dateTime));
            if (!appendBlob.Exists())
                appendBlob.CreateOrReplace();

            if (sender != null && message != null)
            {
                appendBlob.AppendText(string.Format("Timestamp: {0:dd/MM/yyyy HH:mm:ss.fff} \t{1}: {2}{3}", DateTime.UtcNow, sender, message, Environment.NewLine));
            }

            if (panNo != null)
            {
                buffer = appendBlob.DownloadText();
                CreateContainerAndAppendBlob(panNo, dateTime);
                appendBlob.Delete();
            }
        }

        private static void CreateContainerAndAppendBlob(string panNo, string dateTime)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(string.Format("pan-number-{0}", panNo));
            container.CreateIfNotExists();

            CloudAppendBlob appendBlob = container.GetAppendBlobReference(string.Format("verification/date-{0}.log", dateTime));
            if (!appendBlob.Exists())
            {
                appendBlob.CreateOrReplace();
                appendBlob.UploadText(buffer);
                buffer = string.Empty;
            }
        }

        public static void StoreStructuredLog(string dateTime, string panNo, string sender, string message, string polNo = null)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(string.Format("pan-number-{0}", panNo));
            container.CreateIfNotExists();

            CloudAppendBlob appendBlob = null;

            if (polNo != null)
            {
                appendBlob = container.GetAppendBlobReference(string.Format("policy-number-{0}/logs/date-{1}.log", polNo, dateTime));
                if (!appendBlob.Exists())
                    appendBlob.CreateOrReplace();
            }
            else
            {
                appendBlob = container.GetAppendBlobReference(string.Format("verification/date-{0}.log",dateTime));
            }

            appendBlob.AppendText(string.Format("Timestamp: {0:dd/MM/yyyy HH:mm:ss.fff} \t{1}: {2}{3}", DateTime.UtcNow, sender, message, Environment.NewLine));
        }

        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        public static void StorePicture(string polNo, string panNo, string picType, string picturePath, string contentType, string dateTime)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(string.Format("pan-number-{0}", panNo));
            container.CreateIfNotExists();

            string blobName = string.Format("policy-number-{0}/images/image_{1}_{2}-date-{3}{4}", polNo, picType, Guid.NewGuid(), dateTime, Path.GetExtension(picturePath));
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.Properties.ContentType = contentType;
            blockBlob.UploadFromStream(GetStreamFromUrl(picturePath));
        }
    }
}