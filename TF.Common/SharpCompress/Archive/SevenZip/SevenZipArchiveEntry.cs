﻿using TF.Common.SharpCompress.Common.SevenZip;
using System.IO;

namespace TF.Common.SharpCompress.Archive.SevenZip
{
    public class SevenZipArchiveEntry : SevenZipEntry, IArchiveEntry
    {
        internal SevenZipArchiveEntry(SevenZipArchive archive, SevenZipFilePart part)
            : base(part)
        {
            Archive = archive;
        }

        public Stream OpenEntryStream()
        {
            return FilePart.GetCompressedStream();
        }

        public IArchive Archive { get; private set; }

        public bool IsComplete
        {
            get { return true; }
        }

        /// <summary>
        /// This is a 7Zip Anti item
        /// </summary>
        public bool IsAnti
        {
            get { return FilePart.Header.IsAnti; }
        }
    }
}