using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SQLCover.Gateway;

namespace SQLCover.Trace
{
    class SqlTraceController : TraceController
    {
        
        protected const string CreateTrace = @"CREATE EVENT SESSION [{0}] ON SERVER 
ADD EVENT sqlserver.sp_statement_starting(action (sqlserver.plan_handle, sqlserver.tsql_stack) where ([sqlserver].[database_id]=({1})))
ADD TARGET package0.asynchronous_file_target(
     SET filename='{2}')
WITH (MAX_MEMORY=100 MB,EVENT_RETENTION_MODE=NO_EVENT_LOSS,MAX_DISPATCH_LATENCY=1 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=OFF) 
";

        private const string StartTraceFormat = @"alter event session [{0}] on server state = start
";

        private const string StopTraceFormat = @"alter event session [{0}] on server state = stop
";

        private const string DropTraceFormat = @"drop EVENT SESSION [{0}] ON SERVER ";

        private const string ReadTraceFormat = @"select
    event_data
FROM sys.fn_xe_file_target_read_file(N'{0}*.xel', N'{0}*.xem', null, null);";

        private const string GetLogDir = @"EXEC xp_readerrorlog 0, 1, N'Logging SQL Server messages in file'";
        
        public SqlTraceController(DatabaseGateway gateway, string databaseName) : base(gateway, databaseName)
        {
            
        }

        public SqlTraceController(DatabaseGateway gateway, string databaseName, string name) : base(gateway, databaseName, name)
        {

        }

        public override void ComposeLogFileName()
        {
            var logDir = Gateway.GetRecords(GetLogDir).Rows[0].ItemArray[2].ToString();
            if (string.IsNullOrEmpty(logDir))
            {
                throw new InvalidOperationException("Unable to use xp_readerrorlog to find log directory to write extended event file");
            }

            logDir = logDir.ToUpper().Replace("Logging SQL Server messages in file '".ToUpper(), "").Replace("'", "").Replace("ERRORLOG.", "").Replace("ERROR.LOG", "");
            FileName = Path.Combine(logDir, Name);
        }

        protected virtual void Create()
        {
            ComposeLogFileName();
            RunScript(CreateTrace, "Error creating the extended events trace, error: {0}");
        }

        public override void Start()
        {
            Create();
            RunScript(StartTraceFormat, "Error starting the extended events trace, error: {0}");
        }

        public override void Stop()
        {
            RunScript(StopTraceFormat, "Error stopping the extended events trace, error: {0}");
        }

        public override List<string> ReadTrace()
        {
            var data = Gateway.GetTraceRecords(string.Format(ReadTraceFormat, FileName));
            var events = new List<string>();
            foreach (DataRow row in data.Rows)
            {
                events.Add(row.ItemArray[0].ToString());
            }


            return events;
        }

        public override void Drop()
        {
            RunScript(DropTraceFormat, "Error dropping the extended events trace, error: {0}");
            try
            {
                foreach (var file in new DirectoryInfo(new FileInfo(FileName).DirectoryName).EnumerateFiles(new FileInfo(FileName).Name + "*.*"))
                {
                    File.Delete(file.FullName);
                }
            }
            catch (Exception)
            {
            }
        }

        
    }
}