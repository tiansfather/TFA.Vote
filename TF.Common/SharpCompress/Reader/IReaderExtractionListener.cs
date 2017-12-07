using TF.Common.SharpCompress.Common;

namespace TF.Common.SharpCompress.Reader
{
    internal interface IReaderExtractionListener : IExtractionListener
    {
        //        void EnsureEntriesLoaded();
        void FireEntryExtractionBegin(Entry entry);

        void FireEntryExtractionEnd(Entry entry);
    }
}