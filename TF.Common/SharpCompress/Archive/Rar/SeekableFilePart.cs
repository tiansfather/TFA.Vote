﻿using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using System.IO;

namespace TF.Common.SharpCompress.Archive.Rar
{
    internal class SeekableFilePart : RarFilePart
    {
        private readonly Stream stream;
        private readonly string password;

        internal SeekableFilePart(MarkHeader mh, FileHeader fh, Stream stream, string password)
            : base(mh, fh)
        {
            this.stream = stream;
            this.password = password;
        }

        internal override Stream GetCompressedStream()
        {
            stream.Position = FileHeader.DataStartPosition;
            if (FileHeader.Salt != null)
            {
#if PORTABLE
                throw new NotSupportedException("Encrypted Rar files aren't supported in portable distro.");
#else
                return new RarCryptoWrapper(stream, password, FileHeader.Salt);
#endif
            }
            return stream;
        }

        internal override string FilePartName
        {
            get { return "Unknown Stream - File Entry: " + FileHeader.FileName; }
        }
    }
}