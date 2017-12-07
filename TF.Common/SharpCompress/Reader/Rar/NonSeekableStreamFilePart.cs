using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using System.IO;

namespace TF.Common.SharpCompress.Reader.Rar
{
    internal class NonSeekableStreamFilePart : RarFilePart
    {
        internal NonSeekableStreamFilePart(MarkHeader mh, FileHeader fh)
            : base(mh, fh)
        {
        }

        internal override Stream GetCompressedStream()
        {
            return FileHeader.PackedStream;
        }

        internal override string FilePartName
        {
            get { return "Unknown Stream - File Entry: " + FileHeader.FileName; }
        }
    }
}