﻿using System;

namespace TF.Common.SharpCompress.Common
{
    public class InvalidFormatException : ExtractionException
    {
        public InvalidFormatException(string message)
            : base(message)
        {
        }

        public InvalidFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}