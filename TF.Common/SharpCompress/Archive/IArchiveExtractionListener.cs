﻿using TF.Common.SharpCompress.Common;

namespace TF.Common.SharpCompress.Archive
{
    internal interface IArchiveExtractionListener : IExtractionListener
    {
        void EnsureEntriesLoaded();

        void FireEntryExtractionBegin(IArchiveEntry entry);

        void FireEntryExtractionEnd(IArchiveEntry entry);
    }
}