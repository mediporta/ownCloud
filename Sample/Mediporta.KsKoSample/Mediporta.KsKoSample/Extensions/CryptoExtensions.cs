using System;
using System.IO;
using System.Security.Cryptography;

namespace Mediporta.KsKoSample.Extensions
{
    public static class CryptoExtensions
    {
        public static string ComputeSha1Hash(Stream stream)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoProvider.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
