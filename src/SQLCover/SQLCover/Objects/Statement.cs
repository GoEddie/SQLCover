using System;
using System.Collections;
using System.Collections.Generic;

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
        public long BranchesCount;
        public long CoveredBranchesCount;
    }

    public class Branch : CoverageInformation
    {
        public Branch(string text, int offset, int length)
        {
            Text = text;
            Offset = offset;
            Length = length;
        }

        public string Text;
        public int Offset;
        public int Length;
    }

    public class Statement : CoverageInformation
    {
        public Statement(string text, int offset, int length, bool isCoverable)
            : this(text, offset, length, isCoverable, Array.Empty<Branch>())
        { }

        public Statement(string text, int offset, int length, bool isCoverable, IEnumerable<Branch> branches)
        {
            Text = text;
            Offset = offset;
            Length = length;
            IsCoverable = isCoverable;
            Branches = branches;

            NormalizeStatement();
        }

        private void NormalizeStatement()
        {
            int chopOff = 0;
            var index = Length-1;
            if (index <= 0)
                return;
            if (Text.Length == 0)
                return;

            var c = Text[index];

            while (shouldChopOff(c))
            {
                chopOff++;

                if (index <= 0)
                    break;

                c = Text[--index];
            }

            Length = Length - chopOff;
        }

        private bool shouldChopOff(char c)
        {
            if (Char.IsWhiteSpace(c))
                return true;

            if (Char.IsControl(c))
                return true;

            if (c == ';')
                return true;

            return false;
        }

        public string Text;
        public int Offset;
        public int Length;

        public bool IsCoverable;
        public IEnumerable<Branch> Branches;
    }
}
