using TF.Common.SharpCompress.Archive;
using TF.Common.SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TF.Common
{
    public enum ZipCompressType
    {
        None, DEFLATE, BZip2, LZMA, PPMd
    }

    public enum TarCompressType
    {
        None, BZip2, GZip
    }

    public static class CompressHelper
    {
        public static void CompressAsZip(string srcFolder, string tarZipFile, ZipCompressType type = ZipCompressType.DEFLATE)
        {
            CompressionInfo info = new CompressionInfo();
            switch (type)
            {
                case ZipCompressType.None: info.Type = CompressionType.None; break;
                case ZipCompressType.DEFLATE: info.Type = CompressionType.Deflate; break;
                case ZipCompressType.BZip2: info.Type = CompressionType.BZip2; break;
                case ZipCompressType.LZMA: info.Type = CompressionType.LZMA; break;
                case ZipCompressType.PPMd: info.Type = CompressionType.PPMd; break;
                default: info.Type = CompressionType.Deflate; break;
            }
            using (var archive = ArchiveFactory.Create(ArchiveType.Zip))
            {
                archive.AddAllFromDirectory(srcFolder);
                var fs = File.Open(tarZipFile, FileMode.CreateNew);
                archive.SaveTo(fs, info);
                fs.Close();
            }
        }

        public static void CompressAsTar(string srcFolder, string tarZipFile, TarCompressType type = TarCompressType.None)
        {
            CompressionInfo info = new CompressionInfo();
            switch (type)
            {
                case TarCompressType.None: info.Type = CompressionType.None; break;
                case TarCompressType.BZip2: info.Type = CompressionType.BZip2; break;
                case TarCompressType.GZip: info.Type = CompressionType.GZip; break;
                default: info.Type = CompressionType.None; break;
            }
            using (var archive = ArchiveFactory.Create(ArchiveType.Tar))
            {
                archive.AddAllFromDirectory(srcFolder);
                var fs = File.Open(tarZipFile, FileMode.CreateNew);
                archive.SaveTo(fs, info);
                fs.Close();
            }
        }

        /// <summary>
        /// 文件解压，支持RAR, 7Zip, Zip, Tar, GZip, BZip2
        /// </summary>
        /// <param name="filePath">压缩文件地址</param>
        /// <param name="tarFolder">目标文件夹</param>
        /// <returns>返回输出文件名称集合</returns>
        public static List<string> UnCompression(string filePath, string tarFolder)
        {
            List<string> files = new List<string>();
            using (var archive = ArchiveFactory.Open(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        files.Add(entry.Key);
                        entry.WriteToDirectory(tarFolder, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// 文件解压，支持RAR, 7Zip, Zip, Tar, GZip, BZip2
        /// </summary>
        /// <param name="filePath">压缩文件地址</param>
        /// <param name="tarFolder">目标文件夹</param>
        /// <param name="allowFileExt">允许的文件后缀</param>
        /// <returns>返回输出文件名称集合</returns>
        public static List<string> UnCompression(string filePath, string tarFolder, List<string> allowFileExt)
        {
            for (int i = 0; i < allowFileExt.Count; i++)
            {
                allowFileExt[i] = allowFileExt[i].ToLower();
            }
            List<string> files = new List<string>();
            using (var archive = ArchiveFactory.Open(filePath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        var ext = Path.GetExtension(entry.Key).ToLower();
                        if (allowFileExt.Contains(ext))
                        {
                            files.Add(entry.Key);
                            entry.WriteToDirectory(tarFolder, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                }
            }
            return files;
        }

        public static byte[] GZip(byte[] data)
        {
            MemoryStream mStream = new MemoryStream();
            GZipStream gStream = new GZipStream(mStream, CompressionMode.Compress);
            BinaryWriter bw = new BinaryWriter(gStream);
            bw.Write(data);
            bw.Close();
            gStream.Close();
            var bytes = mStream.ToArray();
            mStream.Close();
            return bytes;
        }

        public static byte[] UnGZip(byte[] data)
        {
            int count = BitConverter.ToInt32(data, data.Length - 4);

            MemoryStream mStream = new MemoryStream(data);
            GZipStream gStream = new GZipStream(mStream, CompressionMode.Decompress);

            BinaryReader streamR = new BinaryReader(gStream);
            var outs = streamR.ReadBytes(count);
            streamR.Close();

            gStream.Close();
            mStream.Close();
            return outs;
        }

        public static byte[] GZipAsBytes(string tozipstr)
        {
            MemoryStream mStream = new MemoryStream();
            GZipStream gStream = new GZipStream(mStream, CompressionMode.Compress);

            StreamWriter sw = new StreamWriter(gStream, Encoding.UTF8);
            sw.Write(tozipstr);
            sw.Close();

            gStream.Close();
            var bytes = mStream.ToArray();
            mStream.Close();
            return bytes;
        }

        public static string UnGZipAsString(byte[] data)
        {
            MemoryStream mStream = new MemoryStream(data);
            GZipStream gStream = new GZipStream(mStream, CompressionMode.Decompress);

            StreamReader streamR = new StreamReader(gStream, Encoding.UTF8);
            string outs = streamR.ReadToEnd();
            streamR.Close();

            gStream.Close();
            mStream.Close();
            return outs;
        }

        public static void SaveWithGZip(string file, string str)
        {
            var bs = GZipAsBytes(str);
            System.IO.File.WriteAllBytes(file, bs);
        }

        public static string OpenWithUnGZip(string file)
        {
            var bs = File.ReadAllBytes(file);
            return UnGZipAsString(bs);
        }

        public static void GZipAsFile(object obj, string filePath)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);

                MemoryStream mStream = new MemoryStream();
                GZipStream gStream = new GZipStream(mStream, CompressionMode.Compress);

                BinaryWriter bw = new BinaryWriter(gStream);
                bw.Write(ms.ToArray());
                bw.Close();

                gStream.Close();
                var bytes = mStream.ToArray();
                //string outs = Convert.ToBase64String(mStream.ToArray());
                mStream.Close();

                File.WriteAllBytes(filePath, bytes);
            }
        }

        public static T UnGZipAsFile<T>(string filePath)
        {
            var bytes = UnGZip(File.ReadAllBytes(filePath));
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}