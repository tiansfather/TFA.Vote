using System;

namespace TF.Common.SharpCompress.Common
{
    public class CryptographicException : Exception
    {
        public CryptographicException(string message)
            : base(message)
        {
        }
    }
}