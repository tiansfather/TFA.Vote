﻿using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.Common.Rar;
using TF.Common.SharpCompress.Common.Rar.Headers;
using TF.Common.SharpCompress.IO;
using System.Collections.Generic;
using System.IO;

namespace TF.Common.SharpCompress.Archive.Rar
{
    /// <summary>
    /// A rar part based on a FileInfo object
    /// </summary>
    internal class FileInfoRarArchiveVolume : RarVolume
    {
        internal FileInfoRarArchiveVolume(FileInfo fileInfo, string password, Options options)
            : base(StreamingMode.Seekable, fileInfo.OpenRead(), password, FixOptions(options))
        {
            FileInfo = fileInfo;
            FileParts = base.GetVolumeFileParts().ToReadOnly();
        }

        private static Options FixOptions(Options options)
        {
            //make sure we're closing streams with fileinfo
            if (options.HasFlag(Options.KeepStreamsOpen))
            {
                options = (Options)FlagUtility.SetFlag(options, Options.KeepStreamsOpen, false);
            }
            return options;
        }

        internal ReadOnlyCollection<RarFilePart> FileParts { get; private set; }

        internal FileInfo FileInfo { get; private set; }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new FileInfoRarFilePart(this, markHeader, fileHeader, FileInfo);
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return FileParts;
        }
    }
}