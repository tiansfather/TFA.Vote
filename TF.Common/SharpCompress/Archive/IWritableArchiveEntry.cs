using System.IO;

namespace TF.Common.SharpCompress.Archive
{
    internal interface IWritableArchiveEntry
    {
        Stream Stream { get; }
    }
}