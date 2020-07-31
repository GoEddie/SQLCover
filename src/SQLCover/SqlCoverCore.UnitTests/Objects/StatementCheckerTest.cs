using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SQLCoverCore;
using SQLCoverCore.Objects;

namespace SQLCover.UnitTests.Objects
{
    [TestFixture]
    public class StatementCheckerTests
    {
        [Test]
        public void Statement_Is_Covered_If_CoveredStatement_Has_No_OffsetEnd_And_Statement_Starts_After_CoveredStatement()
        {
            var statement = new Statement("", 100, 10, true);

            var coveredStatement = new CoveredStatement
            {
                Offset = 105,
                OffsetEnd = -1,
                ObjectId = 999
            };

            var checker = new StatementChecker();
            Assert.IsTrue(checker.Overlaps(statement, coveredStatement));
        }

        [Test]
        public void Statement_Is_Covered_If_Statement_Is_Starts_And_Stops_Within_CoveredStatement()
        {
            var statement = new Statement("", 100, 5, true);

            var coveredStatement = new CoveredStatement
            {
                Offset = 200,
                OffsetEnd = 212,
                ObjectId = 999
            };

            var checker = new StatementChecker();
            Assert.IsTrue(checker.Overlaps(statement, coveredStatement));
        }

        [Test]
        public void Statement_Is_Not_Covered_If_Statement_Starts_Before_Covered_Statement()
        {

            var statement = new Statement("", 10, 500, true);

            var coveredStatement = new CoveredStatement
            {
                Offset = 200,
                OffsetEnd = 212,
                ObjectId = 999
            };

            var checker = new StatementChecker();
            Assert.IsFalse(checker.Overlaps(statement, coveredStatement));


        }


        [Test]
        public void Statement_Is_Not_Covered_If_Statement_Starts_After_CoveredStatement_Ends()
        {

            var statement = new Statement("", 1000, 500, true);

            var coveredStatement = new CoveredStatement
            {
                Offset = 200,
                OffsetEnd = 212,
                ObjectId = 999
            };

            var checker = new StatementChecker();
            Assert.IsFalse(checker.Overlaps(statement, coveredStatement));


        }
    }
}
