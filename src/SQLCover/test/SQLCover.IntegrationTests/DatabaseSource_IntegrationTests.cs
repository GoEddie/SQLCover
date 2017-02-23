using System;
using System.Linq;
using System.Runtime.Remoting;
using NUnit.Framework;
using SQLCover.Gateway;
using SQLCover.Source;
using TestLib;

namespace SQLCover.IntegrationTests
{
    [TestFixture]
    public class DatabaseSource_IntegrationTests : SQLCoverTest
    {
        [Test]
        public void Retrieves_Correct_SqlVersion()
        {
        }

        [Test]
        public void Retrives_All_Batches()
        {
            var databaseGateway = new DatabaseGateway(ConnectionStringReader.GetIntegration(), TestDatabaseName);

            var source = new DatabaseSourceGateway(databaseGateway);
            var batches = source.GetBatches(null);


            foreach (var batch in batches)
            {
                Console.WriteLine("batch: {0}", batch.Text);
            }

            Assert.AreEqual(4, batches.Count());

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_procedure]");

            Assert.IsNotNull(proc);
        }

        [Test]
        public void Retrieves_Last_Statement_In_Large_Procedure()
        {
            var databaseGateway = new DatabaseGateway(ConnectionStringReader.GetIntegration(), TestDatabaseName);

            var source = new DatabaseSourceGateway(databaseGateway);
            var batches = source.GetBatches(null);


            foreach (var batch in batches)
            {
                Console.WriteLine("batch: {0}", batch.Text);
            }

            Assert.AreEqual(4, batches.Count());

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_large_procedure]");
            
            Assert.AreEqual(2, proc.StatementCount);
        }

    }
}