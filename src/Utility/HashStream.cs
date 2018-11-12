
namespace FolderCompare
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class HashStream
    {
        private const int FileBufferSize = 1024 * 1024 * 8;     // 8MB buffer

        public static string GetStringHashSHA512(string str)
        {
            string result = null;

            using (HashAlgorithm algo = new SHA512Managed())
            {
                algo.TransformString(str);

                result = algo.GetAsString();
                algo.Clear();
            }
            return result;
        }

        public static string GetFileHashSHA512(string path)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string result = null;

            using (var fs = File.OpenRead(path))
            {
                result = GetStreamHashSHA512(fs);
            }
            sw.Stop();

            Trace.TraceInformation($"{sw.Elapsed} {result} {path}");

            return result;
        }

        /// <summary>
        /// Calculates the SHA512 hash of a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetStreamHashSHA512(Stream stream)
        {
            string result = null;

            using (HashAlgorithm algo = new SHA512Managed())
            {
                algo.TranformStream(stream);

                result = algo.GetAsString();
                algo.Clear();
            }
            return result;
        }

        private static void TransformString(this HashAlgorithm algo, string str)
        {
            if (String.IsNullOrWhiteSpace(str) == false)
            {
                byte[] data = Encoding.UTF8.GetBytes(str);

                algo.TransformFinalBlock(data, 0, data.Length);
            }
        }

        private static void TranformStream(this HashAlgorithm algo, Stream stream)
        {
            int bytesRead;
            var buffer = new byte[FileBufferSize];

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                algo.TransformBlock(buffer, 0, bytesRead, null, 0);
            }
            algo.TransformFinalBlock(buffer, 0, 0);
        }

        private static string GetAsString(this HashAlgorithm algo)
        {
            return BitConverter.ToString(algo.Hash).Replace("-", String.Empty);
        }
    }
}
