using System;
using System.IO;
using System.Reflection;
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
        public void It_can_upload_and_download_correct_file()
        {
            var client = testClientFactory.GetTestClient();

            var uploadResult = client.Upload(remotePath, GetSampleFile(), "application/zip");

            var existResult = client.Exists(remotePath);

            var ocHash = testClientFactory.GetCustomWebdavClient().GetHash(remotePath);

            var fileHash = CryptoExtensions.ComputeSha1Hash(GetSampleFile());
            
            var downloadStream = client.Download(remotePath);

            Assert.That(uploadResult, Is.True);
            Assert.That(existResult, Is.True);
            Assert.That(fileHash, Is.EqualTo(ocHash));
            Assert.That(StreamExtensions.StreamsAreIdentical(GetSampleFile(), downloadStream));
        }
        
        private Stream GetSampleFile()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Mediporta.KsKoSample.Tests.sampleInput.zip");
        }
    }
}
