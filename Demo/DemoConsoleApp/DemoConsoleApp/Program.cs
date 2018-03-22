using System;
using System.Configuration;
using System.IO;
using System.Linq;
using OwncloudClient = owncloudsharp.Client;

/// <summary>
/// Sample owncloud demo app using https://github.com/bnoffer/owncloud-sharp
/// </summary>
namespace DemoConsoleApp
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
                Console.Write($"Exception: {e.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static void TryRemoveFile(OwncloudClient client, string remotePath)
        {
            if (!bool.Parse(ConfigurationSettings.AppSettings["TryDeleteTestFile"]))
                return;

            if (!client.Delete(remotePath))
                throw new Exception("Cannot delete file.");

            Console.WriteLine("Test file deleted.");
        }

        private static void TryCompare(Stream uploadFile, Stream downloadFile)
        {
            if (!StreamsIdentical(uploadFile, downloadFile))
                throw new Exception("File streams not equal.");

            Console.WriteLine("Files are identical.");
        }

        private static Stream TryDownload(OwncloudClient client)
        {
            Console.WriteLine("Downloading file...");

            return client.Download(remotePath);
        }

        private static void TryUpload(OwncloudClient client)
        {
            using (var fileStream = File.OpenRead(localPath))
            {
                Console.WriteLine("Uploading file...");

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

        static bool StreamsIdentical(Stream first, Stream second)
        {
            const int bytesToRead = sizeof(Int64);

            first.Position = 0;
            second.Position = 0;

            if (first.Length != second.Length)
                return false;

            int iterations = (int)Math.Ceiling((double)first.Length / bytesToRead);

            byte[] one = new byte[bytesToRead];
            byte[] two = new byte[bytesToRead];

            for (int i = 0; i < iterations; i++)
            {
                first.Read(one, 0, bytesToRead);
                second.Read(two, 0, bytesToRead);

                if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }

            return true;
        }
    }
}
