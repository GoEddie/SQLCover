using NUnit.Framework;
using SQLCoverCore.Objects;
using SQLCoverCore.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlCoverCore.UnitTests.Parser
{
    [TestFixture]
    public class StatementParserTests
    {
        [Test]
        public void ParsesSimpleSelectQuery()
        {
            const string query = "select 1";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.AreEqual(1, statements.Count);
            Assert.AreEqual(query, statements.First().Text);
        }

        [Test]
        public void ParsesSimpleSelectQueryAsAChild()
        {
            const string query = "begin\r\n select 1;\r\n end";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.AreEqual(1, statements.Count);
        }

        [Test]
        public void ParsesSimpleSelectQueryInProcedure()
        {
            const string query = "create procedure abc\r\n as\r\n begin\r\n select 1;\r\n end";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.AreEqual(1, statements.Count);
        }


        [Test]
        public void IfStatementIsBrokenIntoParts()
        {
            const string query = "create procedure abc\r\n as\r\n if\t   \t1 = 1 begin\r\n select 1;\r\n end";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.IsNotNull(statements.FirstOrDefault(p => p.Text == "if\t   \t1 = 1"));
        }

        [Test]
        public void IfThenStatementIsBrokenIntoParts()
        {
            const string query = "create procedure abc\r\n as\r\n if\t   \t1 = 1 begin\r\n select 1;\r\n end";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.IsNotNull(statements.FirstOrDefault(p => p.Text == "select 1;"));
        }

        [Test]
        public void IfThenElseStatementIsBrokenIntoParts()
        {
            const string query = "create procedure abc\r\n as\r\n if\t   \t1 = 1 begin\r\n select 1;\r\n end\r\nelse\r\nselect 99;";
            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.IsNotNull(statements.FirstOrDefault(p => p.Text == "select 99;"));
        }

        [Test]
        public void WhileStatementIsBrokenIntoParts()
        {
            const string query = @"create procedure abc
as while		1 = 1 begin
 select 1;
  end
  select 99    ;

  select * from a_table;";

            var parser = new StatementParser(SqlServerVersion.Sql120);
            var statements = parser.GetChildStatements(query, false);

            Assert.IsNotNull(statements.FirstOrDefault(p => p.Text == "while		1 = 1"));
        }
    }
}
