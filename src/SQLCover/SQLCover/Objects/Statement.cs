namespace SQLCover.Objects
{

    public class CoverageInformation
    {
        public long HitCount;

    }

    public class CoverageSummary : CoverageInformation
    {
        public long StatementCount;
        public long CoveredStatementCount;
    }

    public class Statement : CoverageInformation
    {


        public string Text;
        public int Offset;
        public int Length;

        public bool IsCoverable;
    }
}
