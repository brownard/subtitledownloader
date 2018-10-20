using System;
using System.IO;
using System.IO.Compression;

namespace SubtitleDownloader.Util
{
	class Decoder
	{
        public static byte[] DecodeAndDecompress(string str)
        {
            return Decompress(Decode(str));
        }

        public static byte[] Decode(string str)
        {
            return Convert.FromBase64String(str);
        }

        static public byte[] Decompress(byte[] b)
        {
            using (var ms = new MemoryStream(b.Length))
            {
                ms.Write(b, 0, b.Length);
                ms.Seek(-4, SeekOrigin.Current);
            
                var lb = new byte[4];
                ms.Read(lb, 0, 4);
                int len = BitConverter.ToInt32(lb, 0);
                ms.Seek(0, SeekOrigin.Begin);
                var ob = new byte[len];
                
                using (var zs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zs.Read(ob, 0, len);
                    return ob;                
                }
            }
        }
	}
}
