﻿using System;

namespace TF.Common.SharpCompress.Common
{
    public class ArchiveExtractionEventArgs<T> : EventArgs
    {
        internal ArchiveExtractionEventArgs(T entry)
        {
            Item = entry;
        }

        public T Item { get; private set; }
    }
}