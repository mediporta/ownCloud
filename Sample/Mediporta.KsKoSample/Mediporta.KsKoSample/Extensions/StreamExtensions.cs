using System;
using System.IO;

namespace Mediporta.KsKoSample.Extensions
{
    public static class StreamExtensions
    {
        public static bool StreamsAreIdentical(Stream first, Stream second)
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
