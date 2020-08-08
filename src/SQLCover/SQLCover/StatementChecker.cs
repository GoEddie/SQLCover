using SQLCover.Objects;

namespace SQLCover
{
    public class StatementChecker
    {
        public bool Overlaps(Statement statement, CoveredStatement coveredStatement)
        {
            var coveredOffsetStart = coveredStatement.Offset / 2;
            var coveredOffsetEnd = coveredStatement.OffsetEnd;

            if (coveredOffsetEnd == -1)
            {
                // Last statement in the batch, so only covered if the 'start' is equal to or less than the statement start
                return (statement.Offset >= coveredOffsetStart);
            }

            var statementStart = statement.Offset;
            var statementEnd = statementStart + statement.Length;
            coveredOffsetEnd = coveredStatement.OffsetEnd / 2;

            if (statementStart >= coveredOffsetStart && statementEnd <= coveredOffsetEnd)
            {
                return true;
            }

            //this is a little painful:
            // https://connect.microsoft.com/SQLServer/feedback/details/3124768
            /*
                i don't think this is an actual problem because on the offsetEnd is wrong, the offsetStart is right so even if there was something like:
                    exec a;b; 
                    which would execute proc a and b, we wouldn't mark b as executed when a was executed because the start would be before b
             */
            coveredOffsetEnd = coveredOffsetEnd +2;

            if (statementStart >= coveredOffsetStart && statementEnd <= coveredOffsetEnd)
            {
                return true;
            }

            return false;

        }

        public bool Overlaps(Statement statement, int offsetStart, int offsetEnd)
        {
            var statementStart = statement.Offset;
            var statementEnd = statementStart + statement.Length;

            if (statementStart >= offsetStart && statementEnd <= offsetEnd)
            {
                return true;
            }

            return false;
        }
    }
}