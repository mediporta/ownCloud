using System.Net;
using WebDav;

namespace Mediporta.KsKoSample.Tests.Webdav
{
    public class CustomWebdavClient
    {
        private readonly string baseUrl;
        private readonly string userName;
        private readonly string userPassword;

        private string DavPath => $"{baseUrl}/remote.php/webdav";

        private readonly WebClient.WebClient webClient;

        public CustomWebdavClient(string baseUrl, string userName, string userPassword)
        {
            this.baseUrl = baseUrl;
            this.userName = userName;
            this.userPassword = userPassword;

            webClient = new WebClient.WebClient()
            {
                Credentials = new WebDavCredential(userName, userPassword)
                {
                    AuthenticationType = AuthType.Basic
                }
            };
        }

        internal string GetHash(string remotePath)
        {
            HttpWebRequest httpGetRequest = (HttpWebRequest)WebRequest.Create($"{DavPath}{remotePath}");
            httpGetRequest.Credentials = new NetworkCredential(userName, userPassword);
            httpGetRequest.PreAuthenticate = true;
            httpGetRequest.Method = @"GET";
            httpGetRequest.Headers.Add(@"Translate", "F");

            HttpWebResponse httpGetResponse = (HttpWebResponse)httpGetRequest.GetResponse();
            
            return httpGetResponse.Headers.Get("OC-Checksum").Split(':')[1];
        }
    }
}
