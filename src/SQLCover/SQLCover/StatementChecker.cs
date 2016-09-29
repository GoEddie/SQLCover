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

            return (statementStart >= coveredOffsetStart && statementEnd <= coveredOffsetEnd);
        }
    }
}