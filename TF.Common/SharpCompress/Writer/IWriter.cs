using TF.Common.SharpCompress.Common;
using System;
using System.IO;

namespace TF.Common.SharpCompress.Writer
{
    public interface IWriter : IDisposable
    {
        ArchiveType WriterType { get; }

        void Write(string filename, Stream source, DateTime? modificationTime);
    }
}