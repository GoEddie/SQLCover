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

            Assert.AreEqual(6, batches.Count());

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_procedure]");

            Assert.IsNotNull(proc);
        }

        [Test]
        public void Retrieves_Last_Statement_In_Large_Procedure()
        {
            var databaseGateway = new DatabaseGateway(ConnectionStringReader.GetIntegration(), TestDatabaseName);

            var source = new DatabaseSourceGateway(databaseGateway);
            var batches = source.GetBatches(null);

            Assert.AreEqual(6, batches.Count());

            var proc = batches.FirstOrDefault(p => p.ObjectName == "[dbo].[a_large_procedure]");
            
            Assert.AreEqual(2, proc.StatementCount);
        }


        [Test]
        public void Doesnt_Die_When_Finding_Encrypted_Stored_Procedures()
        {
            var databaseGateway = new DatabaseGateway(ConnectionStringReader.GetIntegration(), TestDatabaseName);
            databaseGateway.Execute(@"if not exists (select * from sys.procedures where name = 'enc')
begin
	exec sp_executesql N'create procedure enc with encryption 
	as
	select 100;'
end", 15);

            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, null, true, false);
            var results = coverage.Cover("exec enc");
            //if we dont die we are good
        }
        [Test]
        public void Doesnt_Die_When_Table_Trigger_Code()
        {
             var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, null, true, false);
            var results = coverage.Cover("insert into a_table select 100");
            //if we dont die we are good
        }


        [Test]
        public void Doesnt_Die_When_Database_Trigger_Code()
        {
            var coverage = new CodeCoverage(ConnectionStringReader.GetIntegration(), TestDatabaseName, null, true, false);
            var results = coverage.Cover("create synonym abd for a_table");
            //if we dont die we are good
        }


        [Test]
        public void Shows_Warnings_When_Definition_Not_Available()
        {
            var databaseGateway = new DatabaseGateway(ConnectionStringReader.GetIntegration(), TestDatabaseName);
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