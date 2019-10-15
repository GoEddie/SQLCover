using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using SQLCover.Objects;
using SQLCover.Parsers;

namespace SQLCover
{
    public class CoverageResult : CoverageSummary
    {
        private readonly IEnumerable<Batch> _batches;
        private readonly List<string> _sqlExceptions;
        private readonly string _commandDetail;

        public string DatabaseName { get; }
        public string DataSource { get; }

        public List<string> SqlExceptions
        {
            get { return _sqlExceptions; }
        }
        private readonly StatementChecker _statementChecker = new StatementChecker();

        public CoverageResult(IEnumerable<Batch> batches, List<string> xml, string database, string dataSource, List<string> sqlExceptions, string commandDetail)
        {
            _batches = batches;
            _sqlExceptions = sqlExceptions;
            _commandDetail = $"{commandDetail} at {DateTime.Now}";
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
                }

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

            if (_sqlExceptions.Count > 0)
            {
                builder.Append("<SqlExceptions>");

                foreach (var e in _sqlExceptions)
                {
                    builder.AppendFormat("\t<SqlException>{0}</SqlException>", XmlTextEncoder.Encode(e));
                }

                builder.Append("</SqlExceptions>");
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

            builder.Append(@"<html>
<head>
    <title>SQLCover Code Coverage Results</title>
    <style>
     
        html{
            font-family: ""Roboto"",""Helvetica Neue"",Arial,Sans-serif;
            font-size: 100%;
            line-height: 26px;
            word-break: break-word;
        }
        
        i{
            border: solid black;
            border-width: 0 3px 3px 0;
            display: inline-block;
            padding: 3px;
        }

        .up {
            transform: rotate(-135deg);
            -webkit-transform: rotate(-135deg);
        }

        .covered-statement{
            background-color: greenyellow;
        }
            
        table{
            background-color: lightgrey;
        }

        thead{
            background-color: gray;
            color: whitesmoke;
        }
    </style>
    <link media=""all"" rel=""stylesheet"" type=""text/css"" href=""sqlcover.css"" />
  
</ head>
<body id=""top"">");
            builder.Append($"<h2 class=\"header\">{_commandDetail}</h2>");
            builder.Append(
                "<table><thead><td>object name</td><td>statement count</td><td>covered statement count</td><td>coverage %</td></thead>");

            builder.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3:0.00}</td></tr>", "<b>Total</b>",
                statements, coveredStatements, (float)coveredStatements / (float)statements * 100.0);

            foreach (
                var batch in
                _batches.Where(p => !p.ObjectName.Contains("tSQLt"))
                    .OrderByDescending(p => (float)p.CoveredStatementCount / (float)p.StatementCount))
            {
                builder.AppendFormat(
                    "<tr><td><a href=\"#{0}\">{0}</a></td><td>{1}</td><td>{2}</td><td>{3:0.00}</td></tr>",
                    batch.ObjectName, batch.StatementCount, batch.CoveredStatementCount,
                    (float)batch.CoveredStatementCount / (float)batch.StatementCount * 100.0);
            }

            builder.Append("</table>");

            if (_sqlExceptions.Count > 0)
            {
                builder.Append("<div class=\"sql-exceptions\">There were sql exceptions running the batch, see <a href=\"#sql-exceptions\">here</a></div>");
            }

            foreach (var b in _batches)
            {
                builder.AppendFormat("<a name=\"{0}\"><div class=\"batch\">", b.ObjectName);
                builder.AppendFormat("<div><p class=\"batch-summary\">'{3}' summary: statement count: {0}, covered statement count: {1}, coverage %: {2}</p></div>", b.StatementCount, b.CoveredStatementCount,
                    (float) b.CoveredStatementCount / (float) b.StatementCount, b.ObjectName);
                builder.Append("<pre>");
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
                        tempBuffer = start + "<span class=\"covered-statement\">" + end;
                    }
                }

                builder.Append(tempBuffer + "</div></a></pre><a href=\"#top\"><i class=\"up\"></i></a>");
            }

            if (_sqlExceptions.Count > 0)
            {
                builder.Append("<a name=\"sql-exceptions\"><div class=\"sql-exceptions\">");

                foreach (var e in _sqlExceptions)
                {
                    builder.AppendFormat("\t<pre class=\"sql-exception\">{0}</pre>", e);
                }

                builder.Append("</div></a><a href=\"#top\"><i class=\"up sql-exceptions\"></i></a>");
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
        public string Cobertura(string packageName = "sql", Action<CustomCoverageUpdateParameter> customCoverageUpdater = null)
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            // gen coverage header
            var builder = new StringBuilder();
            builder.AppendLine(Unquote($@"<?xml version='1.0'?>
<!--DOCTYPE coverage SYSTEM 'http://cobertura.sourceforge.net/xml/coverage-03.dtd'-->
<coverage lines-valid='{statements}' lines-covered='{coveredStatements}' line-rate='{coveredStatements / (float)statements}' version='1.9' timestamp='{(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds}'>
 <packages>
  <package name='{packageName}'>
    <classes>"));

            var fileMap = _batches.GroupBy(b => b.ObjectName, StringComparer.OrdinalIgnoreCase);
            foreach (var file in fileMap)
            {
                var lines = file.Sum(b => b.StatementCount);
                var coveredLines = file.Sum(b => b.CoveredStatementCount);

                var anyBatch = file.First();
                var coverageUpdateParam = new CustomCoverageUpdateParameter() { Batch = anyBatch };
                customCoverageUpdater?.Invoke(coverageUpdateParam);

                var objectName = anyBatch.ObjectName;
                var filename = anyBatch.FileName;

                // gen file header
                builder.AppendLine(Unquote($"    <class name='{objectName}' filename='{filename}' lines-valid='{lines}' lines-covered='{coveredLines}' line-rate='{coveredLines / (float)lines}' >"));
                builder.AppendLine("     <methods/>");
                builder.AppendLine("     <lines>");

                // gen lines info
                foreach (var line in file.SelectMany(batch => batch.Statements))
                {
                    var offsetInfo = GetOffsets(line.Offset + coverageUpdateParam.OffsetCorrection, line.Length, anyBatch.Text, lineStart: 1 + coverageUpdateParam.LineCorrection);
                    builder.AppendLine(Unquote($"      <line number='{offsetInfo.StartLine}' hits='{line.HitCount}' branch='false' />"));
                }

                // gen file footer
                builder.AppendLine("     </lines>");
                builder.AppendLine("    </class>");
            }

            // gen coverage footer
            builder.AppendLine(@"
   </classes>
  </package>
 </packages>
</coverage>");

            return builder.ToString();
        }

        private static string Unquote(string quotedStr) => quotedStr.Replace("'", "\"");

        public string OpenCoverXml()
        {
            var statements = _batches.Sum(p => p.StatementCount);
            var coveredStatements = _batches.Sum(p => p.CoveredStatementCount);

            var builder = new StringBuilder();

            builder.AppendFormat(
                "<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "<Summary numSequencePoints=\"{0}\" visitedSequencePoints=\"{1}\" numBranchPoints=\"0\" visitedBranchPoints=\"0\" sequenceCoverage=\"{2}\" branchCoverage=\"0.0\" maxCyclomaticComplexity=\"0\" minCyclomaticComplexity=\"0\" /><Modules>\r\n"
                , statements, coveredStatements, coveredStatements / (float)statements * 100.0);

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
                    (float)batch.CoveredStatementCount / (float)batch.StatementCount * 100.0
                    , batch.ObjectName);


                builder.AppendFormat(
                    "\t\t<Method visited=\"{0}\" cyclomaticComplexity=\"0\" sequenceCoverage=\"{1}\" branchCoverage=\"0\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"true\" isSetter=\"false\">\r\n",
                    batch.CoveredStatementCount > 0 ? "true" : "false",
                    batch.CoveredStatementCount / (float)batch.StatementCount * 100.0);

                builder.AppendFormat(
                    "\t\t<Summary numSequencePoints=\"{1}\" visitedSequencePoints=\"0\" numBranchPoints=\"0\" visitedBranchPoints=\"0\" sequenceCoverage=\"{2}\" branchCoverage=\"0\" maxCyclomaticComplexity=\"0\" minCyclomaticComplexity=\"0\" />\r\n",
                    batch.StatementCount, batch.CoveredStatementCount,
                    batch.CoveredStatementCount / (float)batch.StatementCount * 100.0);


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
            => GetOffsets(statement.Offset, statement.Length, text);

        private OpenCoverOffsets GetOffsets(int offset, int length, string text, int lineStart = 1)
        {
            var offsets = new OpenCoverOffsets();

            var column = 1;
            var line = lineStart;
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

                        if (index == offset)
                        {
                            offsets.StartLine = line;
                            offsets.StartColumn = column;
                        }

                        if (index == offset + length)
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

    public class CustomCoverageUpdateParameter
    {
        public Batch Batch { get; internal set; }
        public int LineCorrection { get; set; } = 0;
        public int OffsetCorrection { get; set; } = 0;
    }
}
