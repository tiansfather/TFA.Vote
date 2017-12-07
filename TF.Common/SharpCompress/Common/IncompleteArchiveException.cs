namespace TF.Common.SharpCompress.Common
{
    public class IncompleteArchiveException : ArchiveException
    {
        public IncompleteArchiveException(string message)
            : base(message)
        {
        }
    }
}