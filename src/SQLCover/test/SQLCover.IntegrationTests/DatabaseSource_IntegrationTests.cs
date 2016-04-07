using System;
using System.Linq;
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
            var databaseGateway = new DatabaseGateway(TestServerConnectionString, TestDatabaseName);

            var source = new DatabaseSourceGateway(databaseGateway);
            var batches = source.GetBatches(null);


            foreach (var batch in batches)
            {
                Console.WriteLine("batch: {0}", batch.Text);
            }

            Assert.AreEqual(2, batches.Count);

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_procedure]");

            Assert.IsNotNull(proc);
        }
    }
}