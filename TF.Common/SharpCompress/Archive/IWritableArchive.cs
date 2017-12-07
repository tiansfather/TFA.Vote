using TF.Common.SharpCompress.Common;
using System;
using System.IO;

namespace TF.Common.SharpCompress.Archive
{
    public interface IWritableArchive : IArchive
    {
        void RemoveEntry(IArchiveEntry entry);

        IArchiveEntry AddEntry(string key, Stream source, bool closeStream, long size = 0, DateTime? modified = null);

        void SaveTo(Stream stream, CompressionInfo compressionType);
    }
}