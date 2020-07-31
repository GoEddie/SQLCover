using SQLCoverCore.Objects;

namespace SQLCoverCore
{
    public class CustomCoverageUpdateParameter
    {
        public Batch Batch { get; internal set; }
        public int LineCorrection { get; set; } = 0;
        public int OffsetCorrection { get; set; } = 0;
    }

}