using System.IO;

namespace TF.Common.SharpCompress.Common.Zip
{
    public class ZipVolume : Volume
    {
        public ZipVolume(Stream stream, Options options)
            : base(stream, options)
        {
        }

        public string Comment { get; internal set; }
    }
}