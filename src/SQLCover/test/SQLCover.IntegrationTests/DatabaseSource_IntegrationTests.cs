using System;
using System.Linq;
using NUnit.Framework;
using SQLCover.Gateway;
using SQLCover.Source;

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

            Assert.AreEqual(4, batches.Count());

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_procedure]");

            Assert.IsNotNull(proc);
        }

        [Test]
        public void Retrieves_Last_Statement_In_Large_Procedure()
        {
            var databaseGateway = new DatabaseGateway(TestServerConnectionString, TestDatabaseName);

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


        [Test]
        public void Doesnt_Die_When_Finding_Encrypted_Stored_Procedures()
        {
            var databaseGateway = new DatabaseGateway(TestServerConnectionString, TestDatabaseName);
            databaseGateway.Execute(@"if not exists (select * from sys.procedures where name = 'enc')
begin
	exec sp_executesql N'create procedure enc with encryption 
	as
	select 100;'
end", 15);
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


        [Test]
        public void Shows_Warnings_When_Definition_Not_Available()
        {
            var databaseGateway = new DatabaseGateway(TestServerConnectionString, TestDatabaseName);
            databaseGateway.Execute(@"if not exists (select * from sys.procedures where name = 'enc')
begin
	exec sp_executesql N'create procedure enc with encryption 
	as
	select 100;'
end", 15);
            var source = new DatabaseSourceGateway(databaseGateway);
            var warnings = source.GetWarnings();
            Assert.IsTrue(
                warnings.Contains("enc")
                );

        }


    }
}