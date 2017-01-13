using System;
using System.Data.SqlClient;
using NUnit.Framework;
using TestLib;

namespace SQLCover.IntegrationTests
{
    [TestFixture]
    public class CodeCoverage_IntegrationTests : SQLCoverTest
    {
        [Test]
        public void Can_Get_All_Batches()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName);
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

            Assert.That(result.RawXml(), Is.StringContaining("HitCount=\"1\""));
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
        public void Code_Coverage_Covers_Last_Statement()
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