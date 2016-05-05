using System;
using SQLCover.Gateway;
using SQLCover.Objects;
using SQLCover.Source;

namespace SQLCover.Trace
{
    class TraceControllerBuilder
    {
        public TraceController GetTraceController(DatabaseGateway gateway, string databaseName)
        {
            var source = new DatabaseSourceGateway(gateway);
            var isAzure = source.IsAzure();

            if(!isAzure)
                return new SqlTraceController(gateway, databaseName);


            var version = source.GetVersion();
            if(version < SqlServerVersion.Sql120)
                throw  new Exception("SQL Azure is only supported from Version 12");

            return new AzureTraceController(gateway, databaseName);
        }
    }
}