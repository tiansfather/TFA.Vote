﻿using System;
using System.Diagnostics;
using System.IO;

namespace TF.Common.SharpCompress.Compressor.LZMA.Utilites
{
    internal enum BlockType : byte
    {
        #region Constants

        End = 0,
        Header = 1,
        ArchiveProperties = 2,
        AdditionalStreamsInfo = 3,
        MainStreamsInfo = 4,
        FilesInfo = 5,
        PackInfo = 6,
        UnpackInfo = 7,
        SubStreamsInfo = 8,
        Size = 9,
        CRC = 10,
        Folder = 11,
        CodersUnpackSize = 12,
        NumUnpackStream = 13,
        EmptyStream = 14,
        EmptyFile = 15,
        Anti = 16,
        Name = 17,
        CTime = 18,
        ATime = 19,
        MTime = 20,
        WinAttributes = 21,
        Comment = 22,
        EncodedHeader = 23,
        StartPos = 24,
        Dummy = 25,

        #endregion Constants
    }

    internal static class Utils
    {
        [Conditional("DEBUG")]
        public static void Assert(bool expression)
        {
            if (!expression)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                throw new Exception("Assertion failed.");
            }
        }

        public static void ReadExact(this Stream stream, byte[] buffer, int offset, int length)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (length < 0 || length > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("length");

            while (length > 0)
            {
                int fetched = stream.Read(buffer, offset, length);
                if (fetched <= 0)
                    throw new EndOfStreamException();

                offset += fetched;
                length -= fetched;
            }
        }
    }
}