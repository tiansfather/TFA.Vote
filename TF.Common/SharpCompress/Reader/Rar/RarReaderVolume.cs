using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using TF.Common.SharpCompress.IO;
using System.Collections.Generic;
using System.IO;

namespace TF.Common.SharpCompress.Reader.Rar
{
    public class RarReaderVolume : RarVolume
    {
        internal RarReaderVolume(Stream stream, string password, Options options)
            : base(StreamingMode.Streaming, stream, password, options)
        {
        }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new NonSeekableStreamFilePart(markHeader, fileHeader);
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }
    }
}