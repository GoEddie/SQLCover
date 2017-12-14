using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using NUnit.Framework;

namespace SQLCover.IntegrationTests
{
    [TestFixture]
    public class CodeCoverage_IntegrationTests : SQLCoverTest
    {
        [Test]
        public void SQL2005NotSupported()
        {
            var coverage = new CodeCoverage(TestServer2005ConnectionString, TestDatabaseName);
            coverage.Start();
            Assert.That(coverage.IsStarted == false);
            Assert.That(coverage.Exception is SqlCoverException);
        }

        [Test]
        public void Can_Get_All_Batches()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            var results = coverage.Cover("select 1");

            Assert.IsNotNull(results);

            Console.WriteLine(results.RawXml());
        }

        [Test]
        public void Logging_Works()
        {
            var debugListener = new DebugListener();
            Debug.Listeners.Add(debugListener);
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName, null, true);
            var results = coverage.Cover("SELECT 1");
            Assert.True(debugListener.Messages.Count > 0);
        }

        [Test]
        public void Code_Coverage_Filters_Statements()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName,
                new[] {".*tSQLt.*", ".*proc.*"});
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[a_procedure]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.IsFalse(result.RawXml().Contains("HitCount=\"1\""));
            Assert.IsFalse(result.RawXml().Contains("a_procedure"));
        }

        [Test]
        public void TimeoutCalling_Cover_Fails_With_Timeout_Exception()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName,
                new[] {".*tSQLt.*", ".*proc.*"});
            try
            {
                coverage.Cover("WAITFOR DELAY '1:00:00'", 1);
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == -2)
                {
                    return;
                }
            }

            Assert.Fail("expected sql exception with -2 number");
        }

        [Test]
        //  [Ignore("Not sure why failing. Feedback from GoEddie needed.")]
        public void Code_Coverage_Includes_Last_Statement_Of_Large_Procedure()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[a_large_procedure] 1, 1";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.That(result.CoveredStatementCount, Is.EqualTo(2));

            var xml = result.OpenCoverXml();
        }

        [Test]
        public void Code_Coverage_Returns_All_Covered_Statements()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[a_procedure]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.That(result.RawXml(), Is.StringContaining("HitCount=\"1\""));
        }

        [Test]
        public void Code_Coverage_Covers_Set_Statements()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[set_statements]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.AreEqual(5,
                result.CoveredStatementCount); //not sure why SET QUOTED_IDENTIFIER ON is not covered - don't get xevent on 2008
        }

        [Test]
        public void Code_Coverage_Covers_Last_Statement()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[set_statements]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.AreEqual(5,
                result.CoveredStatementCount); //not sure why SET QUOTED_IDENTIFIER ON is not covered - don't get xevent on 2008
        }

        [Test]
        public void Does_Not_Cover_Views()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(TestServerConnectionString))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select * from a_view";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();
            Assert.IsFalse(result.Html().Contains("a_view"));
        }
    }

    public class DebugListener : TraceListener
    {
        readonly List<string> _messages = new List<string>();

        public List<string> Messages { get { return _messages; } }

        public override void Write(string message)
        {
            _messages.Add(message);
        }

        public override void WriteLine(string message)
        {
            _messages.Add(message);
        }
    }
}