using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName);
            var results = coverage.Cover("select 1");

            Assert.IsNotNull(results);

            Console.WriteLine(results.RawXml());

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
            
            Assert.IsTrue(result.RawXml().Contains("HitCount=\"1\""));
        }

        [Test]
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

            Assert.IsTrue(result.CoveredStatementCount == 2);
        }

        [Test]
        public void Code_Coverage_Filters_Statements()
        {
            var coverage = new CodeCoverage(TestServerConnectionString, TestDatabaseName, new []{".*tSQLt.*", ".*proc.*"});
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
    }
}

