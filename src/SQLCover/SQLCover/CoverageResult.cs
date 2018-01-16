using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms.VisualStyles;
using SQLCover.Objects;
using SQLCover.Parsers;

namespace SQLCover
{
    public class CoverageResult : CoverageSummary
    {
        private readonly IEnumerable<Batch> _batches;

        public string DatabaseName { get; }
        public string DataSource { get; }

        private readonly StatementChecker _statementChecker = new StatementChecker();

        public CoverageResult(IEnumerable<Batch> batches, List<string> xml, string database, string dataSource)
        {
            _batches = batches;
            DatabaseName = database;
            DataSource = dataSource;
            var parser = new EventsParser(xml);

            var statement = parser.GetNextStatement();

            while (statement != null)
            {
                var batch = _batches.FirstOrDefault(p => p.ObjectId == statement.ObjectId);
                if (batch != null)
                {
                    var item = batch.Statements.FirstOrDefault(p => _statementChecker.Overlaps(p, statement));
                    if (item != null)
                    {
                        item.HitCount++;
                    }
                } //254084

                statement = parser.GetNextStatement();
            }

            foreach (var batch in _batches)
            {
                batch.CoveredStatementCount = batch.Statements.Count(p => p.HitCount > 0);
                batch.HitCount = batch.Statements.Sum(p => p.HitCount);
            }

            CoveredStatementCount = _batches.Sum(p => p.CoveredStatementCount);
            StatementCount = _batches.Sum(p => p.StatementCount);
            HitCount = _batches.Sum(p => p.HitCount);
            

        }

        public string RawXml()
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            var builder = new StringBuilder();
            builder.AppendFormat("<CodeCoverage StatementCount=\"{0}\" CoveredStatementCount=\"{1}\">\r\n", statements,
                coveredStatements);

            foreach (var batch in _batches)
            {
                builder.AppendFormat("<Batch Object=\"{0}\" StatementCount=\"{1}\" CoveredStatementCount=\"{2}\">",
                    SecurityElement.Escape(batch.ObjectName), batch.StatementCount, batch.CoveredStatementCount);
                builder.AppendFormat("<Text>\r\n![CDATA[{0}]]</Text>", XmlTextEncoder.Encode(batch.Text));
                foreach (var statement in batch.Statements)
                {
                    builder.AppendFormat(
                        "\t<Statement HitCount=\"{0}\" Offset=\"{1}\" Length=\"{2}\" CanBeCovered=\"{3}\"></Statement>",
                        statement.HitCount, statement.Offset, statement.Length, statement.IsCoverable);
                }

                builder.Append("</Batch>");
            }
            builder.Append("\r\n</CodeCoverage>");
            var s = builder.ToString();

            return s;
        }

        public string Html()
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            var builder = new StringBuilder();

            builder.Append("<html><style>body{font - family:verdana,arial,sans-serif}</style><title>SQLCover Code Coverage Results</title></head>\r\n<body>");
            builder.Append(
                "<table><thead><td>Object name</td><td>Statement count</td><td>Covered statement count</td><td>Coverage %</td></thead>");

            builder.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3:0.00}</td></tr>", "<b>Total</b>",
                statements, coveredStatements, (float) coveredStatements / (float) statements * 100.0);

            foreach (
                var batch in
                _batches.Where(p => !p.ObjectName.Contains("tSQLt"))
                    .OrderByDescending(p => (float) p.CoveredStatementCount / (float) p.StatementCount))
            {
                builder.AppendFormat(
                    "<tr><td><a href=\"#{0}\">{0}</a></td><td>{1}</td><td>{2}</td><td>{3:0.00}</td></tr>",
                    batch.ObjectName, batch.StatementCount, batch.CoveredStatementCount,
                    (float) batch.CoveredStatementCount / (float) batch.StatementCount * 100.0);
            }

            builder.Append("</table>");

            foreach (var b in _batches)
            {
                builder.AppendFormat("<pre><a name=\"{0}\"><div class=\"batch\">", b.ObjectName);

                var tempBuffer = b.Text;
                foreach (var statement in b.Statements.OrderByDescending(p => p.Offset))
                {
                    if (statement.HitCount > 0)
                    {
                        var start = tempBuffer.Substring(0, statement.Offset + statement.Length);
                        var end = tempBuffer.Substring(statement.Offset + statement.Length);
                        tempBuffer = start + "</span>" + end;

                        start = tempBuffer.Substring(0, statement.Offset);
                        end = tempBuffer.Substring(statement.Offset);
                        tempBuffer = start + "<span style=\"background-color: greenyellow\">" + end;
                    }
                }

                builder.Append(tempBuffer + "</div></a></pre>");
            }


            builder.AppendFormat("</body></html>");

            return builder.ToString();
        }

        public void SaveSourceFiles(string path)
        {
            foreach (var batch in _batches)
            {
                File.WriteAllText(Path.Combine(path, batch.ObjectName), batch.Text);
            }
        }

        /// <summary>
        /// https://raw.githubusercontent.com/jenkinsci/cobertura-plugin/master/src/test/resources/hudson/plugins/cobertura/coverage-with-data.xml
        /// http://cobertura.sourceforge.net/xml/coverage-03.dtd
        /// </summary>
        /// <returns></returns>
        public string Cobertura()
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            var builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\"?>");
            builder.Append("<!--DOCTYPE coverage SYSTEM \"http://cobertura.sourceforge.net/xml/coverage-03.dtd\"-->");
            builder.AppendFormat("<coverage lines-valid=\"{0}\" lines-covered=\"{1}\" line-rate=\"{2}\" branch-rate=\"0.0\" version=\"1.9\" timestamp=\"{3}\">", statements, coveredStatements,coveredStatements / (float)statements, (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            builder.Append("<coverage>\r\n");

            return builder.ToString();

        }

        public string OpenCoverXml()
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            var builder = new StringBuilder();

            builder.AppendFormat(
                "<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "<Summary numSequencePoints=\"{0}\" visitedSequencePoints=\"{1}\" numBranchPoints=\"0\" visitedBranchPoints=\"0\" sequenceCoverage=\"{2}\" branchCoverage=\"0.0\" maxCyclomaticComplexity=\"0\" minCyclomaticComplexity=\"0\" /><Modules>\r\n"
                , statements, coveredStatements, coveredStatements / (float) statements * 100.0);

            builder.Append("<Module hash=\"ED-DE-ED-DE-ED-DE-ED-DE-ED-DE-ED-DE-ED-DE-ED-DE-ED-DE-ED-DE\">");
            builder.AppendFormat("<FullName>{0}</FullName>", DatabaseName);
            builder.AppendFormat("<ModuleName>{0}</ModuleName>", DatabaseName);

            var fileMap = new Dictionary<string, int>();
            var i = 1;
            foreach (var batch in _batches)
            {
                fileMap[batch.ObjectName] = i++;
            }

            builder.Append("<Files>\r\n");
            foreach (var pair in fileMap)
            {
                builder.AppendFormat("\t<File uid=\"{0}\" fullPath=\"{1}\" />\r\n", pair.Value, pair.Key);
            }

            builder.Append("</Files>\r\n<Classes>\r\n");

            i = 1;
            foreach (var batch in _batches)
            {
                builder.AppendFormat(
                    "<Class><Summary numSequencePoints=\"{0}\" visitedSequencePoints=\"{1}\" numBranchPoints=\"0\" visitedBranchPoints=\"0\" sequenceCoverage=\"{2}\" branchCoverage=\"0\" maxCyclomaticComplexity=\"0\" minCyclomaticComplexity=\"0\" /><FullName>{3}</FullName><Methods>"
                    , batch.Statements.Count, batch.CoveredStatementCount,
                    (float) batch.CoveredStatementCount / (float) batch.StatementCount * 100.0
                    , batch.ObjectName);


                builder.AppendFormat(
                    "\t\t<Method visited=\"{0}\" cyclomaticComplexity=\"0\" sequenceCoverage=\"{1}\" branchCoverage=\"0\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"true\" isSetter=\"false\">\r\n",
                    batch.CoveredStatementCount > 0 ? "true" : "false",
                    batch.CoveredStatementCount / (float) batch.StatementCount * 100.0);

                builder.AppendFormat(
                    "\t\t<Summary numSequencePoints=\"{1}\" visitedSequencePoints=\"0\" numBranchPoints=\"0\" visitedBranchPoints=\"0\" sequenceCoverage=\"{2}\" branchCoverage=\"0\" maxCyclomaticComplexity=\"0\" minCyclomaticComplexity=\"0\" />\r\n",
                    batch.StatementCount, batch.CoveredStatementCount,
                    batch.CoveredStatementCount / (float) batch.StatementCount * 100.0);


                builder.AppendFormat(
                    "\t\t<MetadataToken>01041980</MetadataToken><Name>{0}</Name><FileRef uid=\"{1}\" />\r\n",
                    batch.ObjectName, fileMap[batch.ObjectName]);
                builder.Append("\t\t<SequencePoints>\r\n");
                var j = 1;
                foreach (var statement in batch.Statements)
                {
                    // if (statement.HitCount > 0)
                    // {

                    var offsets = GetOffsets(statement, batch.Text);

                    builder.AppendFormat(
                        "\t\t\t<SequencePoint vc=\"{0}\" uspid=\"{1}\" ordinal=\"{2}\" offset=\"{3}\" sl=\"{4}\" sc=\"{5}\" el=\"{6}\" ec=\"{7}\" />\r\n",
                        statement.HitCount
                        , i++
                        , j++
                        , statement.Offset
                        , offsets.StartLine, offsets.StartColumn, offsets.EndLine, offsets.EndColumn);
                    // }
                }

                builder.Append("\t\t</SequencePoints>\r\n");
                builder.Append("\t\t</Method>\r\n");
                builder.Append("</Methods>\r\n");
                builder.Append("</Class>\r\n");
            }


            builder.Append("</Classes></Module>\r\n");
            builder.Append("</Modules>\r\n");
            builder.Append("</CoverageSession>");
            var s = builder.ToString();

            return s;
        }

        private OpenCoverOffsets GetOffsets(Statement statement, string text)
        {
            var offsets = new OpenCoverOffsets();

            var column = 1;
            var line = 1;
            var index = 0;

            while (index < text.Length)
            {
                switch (text[index])
                {
                    case '\n':
                        line++;
                        column = 0;
                        break;
                    default:

                        if (index == statement.Offset)
                        {
                            offsets.StartLine = line;
                            offsets.StartColumn = column;
                        }

                        if (index == statement.Offset + statement.Length)
                        {
                            offsets.EndLine = line;
                            offsets.EndColumn = column;
                            return offsets;
                        }
                        column++;
                        break;
                }

                index++;
            }

            return offsets;
        }


        public string NCoverXml()
        {
            return "";
        }
    }

    struct OpenCoverOffsets
    {
        public int StartLine;
        public int EndLine;
        public int StartColumn;
        public int EndColumn;
    }
}