using TF.Common.SharpCompress.Common.GZip;
using System.IO;
using System.Linq;

namespace TF.Common.SharpCompress.Archive.GZip
{
    public class GZipArchiveEntry : GZipEntry, IArchiveEntry
    {
        internal GZipArchiveEntry(GZipArchive archive, GZipFilePart part)
            : base(part)
        {
            Archive = archive;
        }

        public virtual Stream OpenEntryStream()
        {
            return Parts.Single().GetCompressedStream();
        }

        #region IArchiveEntry Members

        public IArchive Archive { get; private set; }

        public bool IsComplete
        {
            get { return true; }
        }

        #endregion IArchiveEntry Members
    }
}