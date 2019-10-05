using SQLCover.Gateway;
using System.IO;

namespace SQLCover.Trace
{
    class SqlLocalDbTraceController : SqlTraceController
    {
        public SqlLocalDbTraceController(DatabaseGateway gateway, string databaseName) : base(gateway, databaseName)
        {
        }

        protected override void Create()
        {
            var logDir = Path.Combine(Path.GetTempPath(), nameof(SqlLocalDbTraceController));
            if (!Directory.Exists(logDir)) { Directory.CreateDirectory(logDir); }

            this.FileName = Path.Combine(logDir, Name);
        }
    }
}