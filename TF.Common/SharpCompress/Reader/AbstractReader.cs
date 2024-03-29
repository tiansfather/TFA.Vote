﻿using TF.Common.SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if PORTABLE
using TF.Common.SharpCompress.Common.Rar.Headers;
#endif

namespace TF.Common.SharpCompress.Reader
{
    /// <summary>
    /// A generic push reader that reads unseekable comrpessed streams.
    /// </summary>
    public abstract class AbstractReader<TEntry, TVolume> : IReader, IReaderExtractionListener
        where TEntry : Entry
        where TVolume : Volume
    {
        private bool completed;
        private IEnumerator<TEntry> entriesForCurrentReadStream;
        private bool wroteCurrentEntry;

        public event EventHandler<ReaderExtractionEventArgs<IEntry>> EntryExtractionBegin;

        public event EventHandler<ReaderExtractionEventArgs<IEntry>> EntryExtractionEnd;

        public event EventHandler<CompressedBytesReadEventArgs> CompressedBytesRead;

        public event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

        internal AbstractReader(Options options, ArchiveType archiveType)
        {
            ArchiveType = archiveType;
            Options = options;
        }

        internal Options Options { get; private set; }

        public ArchiveType ArchiveType { get; private set; }

        /// <summary>
        /// Current volume that the current entry resides in
        /// </summary>
        public abstract TVolume Volume { get; }

        /// <summary>
        /// Current file entry
        /// </summary>
        public TEntry Entry
        {
            get { return entriesForCurrentReadStream.Current; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (entriesForCurrentReadStream != null)
            {
                entriesForCurrentReadStream.Dispose();
            }
            Volume.Dispose();
        }

        #endregion IDisposable Members

        public bool Cancelled { get; private set; }

        /// <summary>
        /// Indicates that the remaining entries are not required.
        /// On dispose of an EntryStream, the stream will not skip to the end of the entry.
        /// An attempt to move to the next entry will throw an exception, as the compressed stream is not positioned at an entry boundary.
        /// </summary>
        public void Cancel()
        {
            if (!completed)
            {
                Cancelled = true;
            }
        }

        public bool MoveToNextEntry()
        {
            if (completed)
            {
                return false;
            }
            if (Cancelled)
            {
                throw new InvalidOperationException("Reader has been cancelled.");
            }
            if (entriesForCurrentReadStream == null)
            {
                return LoadStreamForReading(RequestInitialStream());
            }
            if (!wroteCurrentEntry)
            {
                SkipEntry();
            }
            wroteCurrentEntry = false;
            if (NextEntryForCurrentStream())
            {
                return true;
            }
            completed = true;
            return false;
        }

        internal bool LoadStreamForReading(Stream stream)
        {
            if (entriesForCurrentReadStream != null)
            {
                entriesForCurrentReadStream.Dispose();
            }
            if ((stream == null) || (!stream.CanRead))
            {
                throw new MultipartStreamRequiredException("File is split into multiple archives: '"
                                                           + Entry.Key +
                                                           "'. A new readable stream is required.  Use Cancel if it was intended.");
            }
            entriesForCurrentReadStream = GetEntries(stream).GetEnumerator();
            if (entriesForCurrentReadStream.MoveNext())
            {
                return true;
            }
            return false;
        }

        internal virtual Stream RequestInitialStream()
        {
            return Volume.Stream;
        }

        internal virtual bool NextEntryForCurrentStream()
        {
            return entriesForCurrentReadStream.MoveNext();
        }

        internal abstract IEnumerable<TEntry> GetEntries(Stream stream);

        #region Entry Skip/Write

        private void SkipEntry()
        {
            if (!Entry.IsDirectory)
            {
                Skip();
            }
        }

        private readonly byte[] skipBuffer = new byte[4096];

        private void Skip()
        {
            if (!Entry.IsSolid)
            {
                var rawStream = Entry.Parts.First().GetRawStream();

                if (rawStream != null)
                {
                    var bytesToAdvance = Entry.CompressedSize;
                    for (var i = 0; i < bytesToAdvance / skipBuffer.Length; i++)
                    {
                        rawStream.Read(skipBuffer, 0, skipBuffer.Length);
                    }
                    rawStream.Read(skipBuffer, 0, (int)(bytesToAdvance % skipBuffer.Length));
                    return;
                }
            }
            using (var s = OpenEntryStream())
            {
                while (s.Read(skipBuffer, 0, skipBuffer.Length) > 0)
                {
                }
            }
        }

        public void WriteEntryTo(Stream writableStream)
        {
            if (wroteCurrentEntry)
            {
                throw new ArgumentException("WriteEntryTo or OpenEntryStream can only be called once.");
            }
            if ((writableStream == null) || (!writableStream.CanWrite))
            {
                throw new ArgumentNullException(
                    "A writable Stream was required.  Use Cancel if that was intended.");
            }

            var streamListener = this as IReaderExtractionListener;
            streamListener.FireEntryExtractionBegin(this.Entry);
            Write(writableStream);
            streamListener.FireEntryExtractionEnd(this.Entry);
            wroteCurrentEntry = true;
        }

        internal void Write(Stream writeStream)
        {
            using (Stream s = OpenEntryStream())
            {
                s.TransferTo(writeStream);
            }
        }

        public EntryStream OpenEntryStream()
        {
            if (wroteCurrentEntry)
            {
                throw new ArgumentException("WriteEntryTo or OpenEntryStream can only be called once.");
            }
            var stream = GetEntryStream();
            wroteCurrentEntry = true;
            return stream;
        }

        /// <summary>
        /// Retains a reference to the entry stream, so we can check whether it completed later.
        /// </summary>
        protected EntryStream CreateEntryStream(Stream decompressed)
        {
            return new EntryStream(this, decompressed);
        }

        protected virtual EntryStream GetEntryStream()
        {
            return CreateEntryStream(Entry.Parts.First().GetCompressedStream());
        }

        #endregion Entry Skip/Write

        IEntry IReader.Entry
        {
            get { return Entry; }
        }

        void IExtractionListener.FireCompressedBytesRead(long currentPartCompressedBytes, long compressedReadBytes)
        {
            if (CompressedBytesRead != null)
            {
                CompressedBytesRead(this, new CompressedBytesReadEventArgs()
                {
                    CurrentFilePartCompressedBytesRead = currentPartCompressedBytes,
                    CompressedBytesRead = compressedReadBytes
                });
            }
        }

        void IExtractionListener.FireFilePartExtractionBegin(string name, long size, long compressedSize)
        {
            if (FilePartExtractionBegin != null)
            {
                FilePartExtractionBegin(this, new FilePartExtractionBeginEventArgs()
                {
                    CompressedSize = compressedSize,
                    Size = size,
                    Name = name,
                });
            }
        }

        void IReaderExtractionListener.FireEntryExtractionBegin(Entry entry)
        {
            if (EntryExtractionBegin != null)
            {
                EntryExtractionBegin(this, new ReaderExtractionEventArgs<IEntry>(entry));
            }
        }

        void IReaderExtractionListener.FireEntryExtractionEnd(Entry entry)
        {
            if (EntryExtractionEnd != null)
            {
                EntryExtractionEnd(this, new ReaderExtractionEventArgs<IEntry>(entry));
            }
        }
    }
}