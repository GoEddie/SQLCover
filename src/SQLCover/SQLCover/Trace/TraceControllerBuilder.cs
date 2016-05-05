using SQLCover.Gateway;
using SQLCover.Source;

namespace SQLCover.Trace
{
    class TraceControllerBuilder
    {
        public TraceController GetTraceController(DatabaseGateway gateway, string databaseName)
        {
            var source = new DatabaseSourceGateway(gateway);
            var isAzure = source.IsAzure();

            //if(isAzure)
            return new SqlTraceController(gateway, databaseName);  


        }
    }
}