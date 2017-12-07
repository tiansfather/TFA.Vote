﻿using System;

namespace TF.Common.SharpCompress.Common
{
    [Flags]
    public enum Options
    {
        /// <summary>
        /// No options specified
        /// </summary>
        None = 0,

        /// <summary>
        /// TF.Common.SharpCompress will keep the supplied streams open
        /// </summary>
        KeepStreamsOpen = 1,

        /// <summary>
        /// Look for RarArchive (Check for self-extracting archives or cases where RarArchive isn't at the start of the file)
        /// </summary>
        LookForHeader = 2,
    }
}