﻿namespace TF.Common.SharpCompress.Common.Zip.Headers
{
    internal enum ZipHeaderType
    {
        Ignore,
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
        Split,
    }
}