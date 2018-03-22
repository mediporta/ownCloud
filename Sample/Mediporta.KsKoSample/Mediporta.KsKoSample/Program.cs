using Mediporta.KsKoSample.Extensions;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using OwncloudClient = owncloudsharp.Client;

/// <summary>
/// Sample owncloud demo app using https://github.com/bnoffer/owncloud-sharp
/// </summary>
namespace Mediporta.KsKoSample.Console
{
    class Program
    {
        const string remotePath = "/sampleInput.zip";
        const string localPath = @"../../../sampleInput.zip";

        static void Main(string[] args)
        {
            try
            {
                var client = GetClient();

                TryUpload(client);

                var downloadFile = TryDownload(client);

                TryCompare(File.OpenRead(localPath), downloadFile);

                TryRemoveFile(client, remotePath);
            }
            catch (Exception e)
            {
                System.Console.Write($"Exception: {e.Message}");
            }
            finally
            {
                System.Console.ReadKey();
            }
        }

        private static void TryRemoveFile(OwncloudClient client, string remotePath)
        {
            if (!bool.Parse(ConfigurationSettings.AppSettings["TryDeleteTestFile"]))
                return;

            if (!client.Delete(remotePath))
                throw new Exception("Cannot delete file.");

            System.Console.WriteLine("Test file deleted.");
        }

        private static void TryCompare(Stream uploadFile, Stream downloadFile)
        {
            if (!StreamExtensions.StreamsAreIdentical(uploadFile, downloadFile))
                throw new Exception("File streams not equal.");

            System.Console.WriteLine("Files are identical.");
        }

        private static Stream TryDownload(OwncloudClient client)
        {
            System.Console.WriteLine("Downloading file...");

            return client.Download(remotePath);
        }

        private static void TryUpload(OwncloudClient client)
        {
            using (var fileStream = File.OpenRead(localPath))
            {
                System.Console.WriteLine("Uploading file...");

                if (!client.Upload(remotePath, fileStream, "application/zip"))
                    throw new Exception("Upload failed.");

                if (!client.Exists(remotePath))
                    throw new Exception("File not exists.");
            }
        }

        private static OwncloudClient GetClient()
        {
            var credentials = File.ReadAllLines(@"../../../credentials.txt");

            if (credentials.Count() != 2)
                throw new ArgumentException(nameof(credentials));

            return new OwncloudClient(ConfigurationSettings.AppSettings["OwncloudUrl"], credentials[0], credentials[1]);
        }
    }
}
