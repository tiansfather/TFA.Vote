﻿using System.IO;

namespace TF.Common.SharpCompress.Common
{
    public abstract class FilePart
    {
        internal abstract string FilePartName { get; }

        internal abstract Stream GetCompressedStream();

        internal abstract Stream GetRawStream();
    }
}