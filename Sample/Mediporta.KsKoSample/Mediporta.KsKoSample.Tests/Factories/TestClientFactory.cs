using OwncloudClient = owncloudsharp.Client;

namespace Mediporta.KsKoSample.Tests.Factories
{
    public class TestClientFactory
    {
        public OwncloudClient GetTestClient()
        {
            return new OwncloudClient("https://ks-ko.mediporta.pl", "pps-897354", "idjcL6xLCKuJyOmh9o03");
        }
    }
}
