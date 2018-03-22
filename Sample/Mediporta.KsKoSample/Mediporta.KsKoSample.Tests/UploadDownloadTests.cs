using System;
using System.IO;
using System.Reflection;
using Mediporta.KsKoSample.Extensions;
using NUnit.Framework;
using OwncloudClient = owncloudsharp.Client;

namespace Mediporta.KsKoSample.Tests
{
    public class UploadDownloadTests
    {
        private string remotePath;

        [SetUp]
        public void Setup()
        {
            remotePath = $"/{Guid.NewGuid().ToString("N")}.zip";
        }

        [TearDown]
        public void TearDown()
        {
            var client = GetTestClient();

            client.Delete(remotePath);
        }

        [Test]
        public void It_can_upload_and_download_correct_file()
        {
            var client = GetTestClient();

            var uploadResult = client.Upload(remotePath, GetSampleFile(), "application/zip");

            var existResult = client.Exists(remotePath);

            var downloadStream = client.Download(remotePath);

            Assert.That(uploadResult, Is.True);
            Assert.That(existResult, Is.True);
            Assert.That(StreamExtensions.StreamsAreIdentical(GetSampleFile(), downloadStream));
        }

        private OwncloudClient GetTestClient()
        {
            return new OwncloudClient("https://ks-ko.mediporta.pl", "pps-897354", "idjcL6xLCKuJyOmh9o03");
        }

        private Stream GetSampleFile()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Mediporta.KsKoSample.Tests.sampleInput.zip");
        }
    }
}
