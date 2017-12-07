using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using TF.Common.SharpCompress.IO;
using System.Collections.Generic;
using System.IO;

namespace TF.Common.SharpCompress.Archive.Rar
{
    internal class StreamRarArchiveVolume : RarVolume
    {
        internal StreamRarArchiveVolume(Stream stream, string password, Options options)
            : base(StreamingMode.Seekable, stream, password, options)
        {
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new SeekableFilePart(markHeader, fileHeader, Stream, Password);
        }
    }
}