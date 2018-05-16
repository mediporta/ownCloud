using System;
using System.IO;
using Mediporta.KsKoSample.Extensions;
using Mediporta.KsKoSample.Tests.Factories;
using NUnit.Framework;


namespace Mediporta.KsKoSample.Tests
{
    public class UploadDownloadTests
    {
        private string remotePath;
        private TestClientFactory testClientFactory;

        [SetUp]
        public void Setup()
        {
            remotePath = $"/{Guid.NewGuid().ToString("N")}.zip";
            testClientFactory = new TestClientFactory();
        }

        [TearDown]
        public void TearDown()
        {
            var client = testClientFactory.GetTestClient();

            client.Delete(remotePath);
        }

        [Test]
        [TestCase(0.1)]
        [TestCase(1)]
        [TestCase(10)]
        public void It_can_upload_and_download_correct_file(double size)
        {
            var client = testClientFactory.GetTestClient();
            var fileStream = GetSampleFile(size);
            var uploadResult = client.Upload(remotePath, fileStream, "application/zip");
            var existResult = client.Exists(remotePath);
            var ocHash = testClientFactory.GetCustomWebdavClient().GetHash(remotePath);
            fileStream.Seek(0, SeekOrigin.Begin);
            var fileHash = CryptoExtensions.ComputeSha1Hash(fileStream);
            var downloadStream = client.Download(remotePath);

            Assert.That(uploadResult, Is.True);
            Assert.That(existResult, Is.True);
            Assert.That(fileHash, Is.EqualTo(ocHash));
            fileStream.Seek(0, SeekOrigin.Begin);
            Assert.That(StreamExtensions.StreamsAreIdentical(fileStream, downloadStream));
        }

        private Stream GetSampleFile(double size)
        {
            // Note: block size must be a factor of 1MB to avoid rounding errors :)
            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;
            var data = new byte[blockSize];
            var rng = new Random();
            using (var stream = File.OpenWrite("samplefile"))
            {
                // There 
                for (int i = 0; i < size * blocksPerMb; i++)
                {
                    rng.NextBytes(data);
                    stream.Write(data, 0, data.Length);
                }

                return stream;
            }
        }
    }
}
