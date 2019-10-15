﻿using System;
using System.Data.SqlClient;
using NUnit.Framework;
using TestLib;
using System.Threading;

namespace SQLCover.IntegrationTests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class CodeCoverage_IntegrationTests : SQLCoverTest
    {

        [Test]
        public void Can_Get_All_Batches()
        {            
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, null, true, false);
            var results = coverage.Cover("select 1");

            Assert.IsNotNull(results);

            Console.WriteLine(results.RawXml());
        }

        [Test]
        public void Code_Coverage_Filters_Statements()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, new[] {".*tSQLt.*", ".*proc.*"});
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
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
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, new[] { ".*tSQLt.*", ".*proc.*" });
            try
            {
                coverage.Cover("WAITFOR DELAY '1:00:00'", 1);
            }
            catch (SqlException e)
            {
                if (e.Number == -2)
                {
                    return;
                }
            }

            Assert.Fail("expected sql exception with -2 number");
        }

        [Test]
        public void Code_Coverage_Includes_Last_Statement_Of_Large_Procedure()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
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
        public void Code_Coverage_Excludes_Label_From_Lines_That_Can_Be_Covered()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[a_procedure_with_goto]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();
            
            
            Assert.That(result.CoveredStatementCount, Is.EqualTo(4));
            Assert.That(result.StatementCount, Is.EqualTo(24));
            var xml = result.OpenCoverXml();

        }

        [Test]
       public void Code_Coverage_Returns_All_Covered_Statements()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[a_procedure]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.That(result.RawXml().Contains("HitCount=\"1\""), Is.True);
        }

        [Test]
        public void Code_Coverage_Covers_Set_Statements()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "exec [dbo].[set_statements]";
                    cmd.ExecuteNonQuery();
                }
            }

            var result = coverage.Stop();

            Assert.AreEqual(5, result.CoveredStatementCount);   //not sure why SET QUOTED_IDENTIFIER ON is not covered - don't get xevent on 2008
        }

        [Test]
        public void Does_Not_Cover_Views()
        {

            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            coverage.Start();
            using (var con = new SqlConnection(ConnectionStringReader.GetIntegration()))
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
}