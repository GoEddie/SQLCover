using System.Collections.Generic;
using SQLCover.Objects;

namespace SQLCover.Source
{
    public interface SourceGateway
    {
        SqlServerVersion GetVersion();
        IList<Batch> GetBatches(List<string> objectFilter);
    }
}