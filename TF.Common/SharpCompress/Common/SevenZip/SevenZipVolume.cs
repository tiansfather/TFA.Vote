using System.IO;

namespace TF.Common.SharpCompress.Common.SevenZip
{
    public class SevenZipVolume : Volume
    {
        public SevenZipVolume(Stream stream, Options options)
            : base(stream, options)
        {
        }
    }
}