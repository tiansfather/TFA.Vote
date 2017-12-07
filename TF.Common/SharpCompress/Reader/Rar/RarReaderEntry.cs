using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using System.Collections.Generic;

namespace TF.Common.SharpCompress.Reader.Rar
{
    public class RarReaderEntry : RarEntry
    {
        internal RarReaderEntry(bool solid, RarFilePart part)
        {
            Part = part;
            IsSolid = solid;
        }

        internal RarFilePart Part { get; private set; }

        internal override IEnumerable<FilePart> Parts
        {
            get { return Part.AsEnumerable<FilePart>(); }
        }

        internal override FileHeader FileHeader
        {
            get { return Part.FileHeader; }
        }

        public override CompressionType CompressionType
        {
            get { return CompressionType.Rar; }
        }

        /// <summary>
        /// The compressed file size
        /// </summary>
        public override long CompressedSize
        {
            get { return Part.FileHeader.CompressedSize; }
        }

        /// <summary>
        /// The uncompressed file size
        /// </summary>
        public override long Size
        {
            get { return Part.FileHeader.UncompressedSize; }
        }
    }
}