using System.Collections.Generic;
using SQLCover.Parsers;

namespace SQLCover.Objects
{
    public class Batch : CoverageSummary
    {
        public Batch(StatementParser parser, bool quotedIdentifier, string text, string fileName, string objectName, int objectId)
        {
            QuotedIdentifier = quotedIdentifier;
            Text = text;
            FileName = fileName;
            ObjectName = objectName;
            ObjectId = objectId;
            
            Statements = parser.GetChildStatements(text, quotedIdentifier);
        }
            
        public bool QuotedIdentifier;
        public string Text;
        public string FileName;
        public string ObjectName;
        public int ObjectId;

        public readonly  List<Statement> Statements;
    }
}