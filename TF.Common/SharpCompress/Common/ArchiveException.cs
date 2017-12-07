using System;

namespace TF.Common.SharpCompress.Common
{
    public class ArchiveException : Exception
    {
        public ArchiveException(string message)
            : base(message)
        {
        }
    }
}