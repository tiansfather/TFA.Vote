﻿using TF.Common.SharpCompress.Common;
using TF.Common.SharpCompress.IO;
using System.IO;

namespace TF.Common.SharpCompress.Archive
{
    public static class IArchiveEntryExtensions
    {
        public static void WriteTo(this IArchiveEntry archiveEntry, Stream streamToWriteTo)
        {
            if (archiveEntry.Archive.Type == ArchiveType.Rar && archiveEntry.Archive.IsSolid)
            {
                throw new InvalidFormatException("Cannot use Archive random access on SOLID Rar files.");
            }

            if (archiveEntry.IsDirectory)
            {
                throw new ExtractionException("Entry is a file directory and cannot be extracted.");
            }

            var streamListener = archiveEntry.Archive as IArchiveExtractionListener;
            streamListener.EnsureEntriesLoaded();
            streamListener.FireEntryExtractionBegin(archiveEntry);
            streamListener.FireFilePartExtractionBegin(archiveEntry.Key, archiveEntry.Size, archiveEntry.CompressedSize);
            var entryStream = archiveEntry.OpenEntryStream();
            if (entryStream == null)
            {
                return;
            }
            using (entryStream)
            using (Stream s = new ListeningStream(streamListener, entryStream))
            {
                s.TransferTo(streamToWriteTo);
            }
            streamListener.FireEntryExtractionEnd(archiveEntry);
        }

#if !PORTABLE && !NETFX_CORE

        /// <summary>
        /// Extract to specific directory, retaining filename
        /// </summary>
        public static void WriteToDirectory(this IArchiveEntry entry, string destinationDirectory,
                                            ExtractOptions options = ExtractOptions.Overwrite)
        {
            string destinationFileName;
            string file = Path.GetFileName(entry.Key);

            if (options.HasFlag(ExtractOptions.ExtractFullPath))
            {
                string folder = Path.GetDirectoryName(entry.Key);
                string destdir = Path.Combine(destinationDirectory, folder);
                if (!Directory.Exists(destdir))
                {
                    Directory.CreateDirectory(destdir);
                }
                destinationFileName = Path.Combine(destdir, file);
            }
            else
            {
                destinationFileName = Path.Combine(destinationDirectory, file);
            }
            entry.WriteToFile(destinationFileName, options);
        }

        /// <summary>
        /// Extract to specific file
        /// </summary>
        public static void WriteToFile(this IArchiveEntry entry, string destinationFileName,
                                       ExtractOptions options = ExtractOptions.Overwrite)
        {
            if (entry.IsDirectory)
            {
                return;
            }
            FileMode fm = FileMode.Create;

            if (!options.HasFlag(ExtractOptions.Overwrite))
            {
                fm = FileMode.CreateNew;
            }
            using (FileStream fs = File.Open(destinationFileName, fm))
            {
                entry.WriteTo(fs);
            }

            if (options.HasFlag(ExtractOptions.PreserveFileTime) || options.HasFlag(ExtractOptions.PreserveAttributes))
            {
                // update file time to original packed time
                FileInfo nf = new FileInfo(destinationFileName);
                if (nf.Exists)
                {
                    if (options.HasFlag(ExtractOptions.PreserveFileTime))
                    {
                        if (entry.CreatedTime.HasValue)
                        {
                            nf.CreationTime = entry.CreatedTime.Value;
                        }

                        if (entry.LastModifiedTime.HasValue)
                        {
                            nf.LastWriteTime = entry.LastModifiedTime.Value;
                        }

                        if (entry.LastAccessedTime.HasValue)
                        {
                            nf.LastAccessTime = entry.CreatedTime.Value;
                        }
                    }

                    if (options.HasFlag(ExtractOptions.PreserveAttributes))
                    {
                        if (entry.Attrib.HasValue)
                        {
                            nf.Attributes = (FileAttributes)System.Enum.ToObject(typeof(FileAttributes), entry.Attrib.Value);
                        }
                    }
                }
            }
        }

#endif
    }
}