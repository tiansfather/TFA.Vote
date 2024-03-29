﻿using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace TF.Common.SharpCompress.Archive.GZip
{
    internal class GZipWritableArchiveEntry : GZipArchiveEntry, IWritableArchiveEntry
    {
        private readonly string path;
        private readonly long size;
        private readonly DateTime? lastModified;
        private readonly bool closeStream;
        private readonly Stream stream;

        internal GZipWritableArchiveEntry(GZipArchive archive, Stream stream,
                                          string path, long size, DateTime? lastModified, bool closeStream)
            : base(archive, null)
        {
            this.stream = stream;
            this.path = path;
            this.size = size;
            this.lastModified = lastModified;
            this.closeStream = closeStream;
        }

        public override long Crc
        {
            get { return 0; }
        }

        public override string Key
        {
            get { return path; }
        }

        public override long CompressedSize
        {
            get { return 0; }
        }

        public override long Size
        {
            get { return size; }
        }

        public override DateTime? LastModifiedTime
        {
            get { return lastModified; }
        }

        public override DateTime? CreatedTime
        {
            get { return null; }
        }

        public override DateTime? LastAccessedTime
        {
            get { return null; }
        }

        public override DateTime? ArchivedTime
        {
            get { return null; }
        }

        public override bool IsEncrypted
        {
            get { return false; }
        }

        public override bool IsDirectory
        {
            get { return false; }
        }

        public override bool IsSplit
        {
            get { return false; }
        }

        internal override IEnumerable<FilePart> Parts
        {
            get { throw new NotImplementedException(); }
        }

        Stream IWritableArchiveEntry.Stream
        {
            get
            {
                return stream;
            }
        }

        public override Stream OpenEntryStream()
        {
            //ensure new stream is at the start, this could be reset
            stream.Seek(0, SeekOrigin.Begin);
            return new NonDisposingStream(stream);
        }

        internal override void Close()
        {
            if (closeStream)
            {
                stream.Dispose();
            }
        }
    }
}