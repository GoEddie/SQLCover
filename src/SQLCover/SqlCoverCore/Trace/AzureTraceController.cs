using SQLCoverCore.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace SQLCoverCore.Trace
{
    internal class AzureTraceController : TraceController
    {
        private const string CreateTrace = @"CREATE EVENT SESSION [{0}] ON DATABASE 
ADD EVENT sqlserver.sp_statement_starting(action (sqlserver.plan_handle, sqlserver.tsql_stack) where ([sqlserver].[database_id]=({1})))
add target package0.ring_buffer(set max_memory = 500
) /* not file {2}*/
WITH (EVENT_RETENTION_MODE=ALLOW_MULTIPLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=1 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=OFF,STARTUP_STATE=OFF) 
";

        private const string StartTraceFormat = @"alter event session [{0}] on database state = start
";

        private const string StopTraceFormat = @"alter event session [{0}] on database state = stop
";

        private const string DropTraceFormat = @"drop EVENT SESSION [{0}] ON database ";

        private const string ReadTraceFormat = @"declare @x xml = (

    select target_data from sys.dm_xe_database_session_targets AS xet

    JOIN sys.dm_xe_database_sessions AS xe

       ON(xe.address = xet.event_session_address)

    WHERE xe.name = '{0}');
            with a as (
            select x.p.query('.') v from  @x.nodes('/RingBufferTarget/event') as x(p)
	)

select  v.value('(/event/@timestamp)[1]', 'datetime'), v from a

    where v.value('(/event/@timestamp)[1]', 'datetime') > '{1}'";

        private readonly List<string> _events = new List<string>();

        private bool _stop;
        private bool _stopped;
        private bool _stopping;


        public AzureTraceController(DatabaseGateway gateway, string databaseName) : base(gateway, databaseName)
        {
        }


        private void Create()
        {
            RunScript(CreateTrace, "Error creating the extended events trace, error: {0}");
        }

        public override void Start()
        {
            Create();
            RunScript(StartTraceFormat, "Error starting the extended events trace, error: {0}");
            ThreadPool.QueueUserWorkItem(PollAzureForEvents);
        }

        private void PollAzureForEvents(object state)
        {
            var maxDateReceived = new DateTime(1980, 04, 01);

            while (true)
            {
                try
                {
                    var records = Gateway.GetRecords(string.Format(ReadTraceFormat, Name, maxDateReceived.ToString("yyyy-MM-dd HH:mm:ss.fff")));

                    foreach (DataRow row in records.Rows)
                    {
                        _events.Add(row.ItemArray[1].ToString());
                        var date = (DateTime)row.ItemArray[0];

                        if (date > maxDateReceived)
                            maxDateReceived = date;
                    }
                }
                catch (Exception)
                {
                }

                if (_stopping)
                {
                    //first time we set _stopped but this guarantees that we end up with a final call to get any pending events...
                    _stopped = true;
                    return;
                }

                if (_stop)
                {
                    _stopping = true;
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public void StopInternal()
        {
            _stop = true;
            while (!_stopped)
                Thread.Sleep(1000);

            RunScript(StopTraceFormat, "Error stopping the extended events trace, error: {0}");
        }


        public override void Stop()
        {
            //stop is called from readtrace so we can read the data before closing it when using a ring buffer
        }

        public override List<string> ReadTrace()
        {
            StopInternal();
            return _events;
        }

        public override void Drop()
        {
            RunScript(DropTraceFormat, "Error dropping the extended events trace, error: {0}");
        }
    }
}