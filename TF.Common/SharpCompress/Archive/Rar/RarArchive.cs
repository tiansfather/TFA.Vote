﻿using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using TF.Common.SharpCompress.Compressor.Rar;
using TF.Common.SharpCompress.IO;
using TF.Common.SharpCompress.Reader;
using TF.Common.SharpCompress.Reader.Rar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TF.Common.SharpCompress.Archive.Rar
{
    public class RarArchive : AbstractArchive<RarArchiveEntry, RarVolume>
    {
        private readonly Unpack unpack = new Unpack();

        internal Unpack Unpack
        {
            get { return unpack; }
        }

#if !PORTABLE && !NETFX_CORE

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        internal RarArchive(FileInfo fileInfo, Options options, string password)
            : base(ArchiveType.Rar, fileInfo, options, password)
        {
        }

        protected override IEnumerable<RarVolume> LoadVolumes(FileInfo file, Options options)
        {
            return RarArchiveVolumeFactory.GetParts(file, Password, options);
        }

#endif

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        internal RarArchive(IEnumerable<Stream> streams, Options options, string password)
            : base(ArchiveType.Rar, streams, options, password)
        {
        }

        protected override IEnumerable<RarArchiveEntry> LoadEntries(IEnumerable<RarVolume> volumes)
        {
            return RarArchiveEntryFactory.GetEntries(this, volumes);
        }

        protected override IEnumerable<RarVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return RarArchiveVolumeFactory.GetParts(streams, Password, options);
        }

        protected override IReader CreateReaderForSolidExtraction()
        {
            var stream = Volumes.First().Stream;
            stream.Position = 0;
            return RarReader.Open(stream);
        }

        public override bool IsSolid
        {
            get { return Volumes.First().IsSolidArchive; }
        }

        #region Creation

#if !PORTABLE && !NETFX_CORE

        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        public static RarArchive Open(string filePath, Options options = Options.None, string password = null)
        {
            filePath.CheckNotNullOrEmpty("filePath");
            return Open(new FileInfo(filePath), options, password);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        public static RarArchive Open(FileInfo fileInfo, Options options = Options.None, string password = null)
        {
            fileInfo.CheckNotNull("fileInfo");
            return new RarArchive(fileInfo, options, password);
        }

#endif

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        public static RarArchive Open(Stream stream, Options options = Options.KeepStreamsOpen, string password = null)
        {
            stream.CheckNotNull("stream");
            return Open(stream.AsEnumerable(), options, password);
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        /// <param name="password"></param>
        public static RarArchive Open(IEnumerable<Stream> streams, Options options = Options.KeepStreamsOpen, string password = null)
        {
            streams.CheckNotNull("streams");
            return new RarArchive(streams, options, password);
        }

#if !PORTABLE && !NETFX_CORE

        public static bool IsRarFile(string filePath)
        {
            return IsRarFile(new FileInfo(filePath));
        }

        public static bool IsRarFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return false;
            }
            using (Stream stream = fileInfo.OpenRead())
            {
                return IsRarFile(stream);
            }
        }

#endif

        public static bool IsRarFile(Stream stream)
        {
            return IsRarFile(stream, Options.None);
        }

        public static bool IsRarFile(Stream stream, Options options)
        {
            try
            {
                var headerFactory = new RarHeaderFactory(StreamingMode.Seekable, options);
                RarHeader header = headerFactory.ReadHeaders(stream).FirstOrDefault();
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof(HeaderType), header.HeaderType);
            }
            catch
            {
                return false;
            }
        }

        #endregion Creation
    }
}