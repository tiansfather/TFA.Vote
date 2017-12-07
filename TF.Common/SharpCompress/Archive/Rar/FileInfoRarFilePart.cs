using TF.Common.SharpCompress.Common.Rar.Headers;
using System.IO;

namespace TF.Common.SharpCompress.Archive.Rar
{
    internal class FileInfoRarFilePart : SeekableFilePart
    {
        private readonly FileInfoRarArchiveVolume volume;

        internal FileInfoRarFilePart(FileInfoRarArchiveVolume volume, MarkHeader mh, FileHeader fh, FileInfo fi)
            : base(mh, fh, volume.Stream, volume.Password)
        {
            this.volume = volume;
            FileInfo = fi;
        }

        internal FileInfo FileInfo { get; private set; }

        internal override string FilePartName
        {
            get
            {
                return "Rar File: " + FileInfo.FullName
                       + " File Entry: " + FileHeader.FileName;
            }
        }
    }
}