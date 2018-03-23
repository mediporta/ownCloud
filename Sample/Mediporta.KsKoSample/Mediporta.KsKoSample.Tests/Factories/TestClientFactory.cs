using Mediporta.KsKoSample.Tests.Webdav;
using OwncloudClient = owncloudsharp.Client;

namespace Mediporta.KsKoSample.Tests.Factories
{
    public class TestClientFactory
    {
        private const string baseUrl = "https://ks-ko.mediporta.pl";
        private const string userName = "pps-897354";
        private const string userPassword = "idjcL6xLCKuJyOmh9o03";

        public OwncloudClient GetTestClient()
        {
            return new OwncloudClient(baseUrl, userName, userPassword);
        }

        public CustomWebdavClient GetCustomWebdavClient()
        {
            return new CustomWebdavClient(baseUrl, userName, userPassword);
        }
    }
}
