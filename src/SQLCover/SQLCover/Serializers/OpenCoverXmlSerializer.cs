using SQLCover.Objects;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SQLCover.Serializers
{
    public class OpenCoverXmlSerializer
    {
        private long uniqueId = 1;

        #region " XNames "

        private static readonly XName BranchPointElementName = XName.Get("BranchPoint");
        private static readonly XName BranchPointsElementName = XName.Get("BranchPoints");
        private static readonly XName ClassesElementName = XName.Get("Classes");
        private static readonly XName ClassElementName = XName.Get("Class");
        private static readonly XName CoverageSessionElementName = XName.Get("CoverageSession");
        private static readonly XName FileRefName = XName.Get("FileRef");
        private static readonly XName FileElementName = XName.Get("File");
        private static readonly XName FilesElementName = XName.Get("Files");
        private static readonly XName FullNameElementName = XName.Get("FullName");
        private static readonly XName MetadataTokenElementName = XName.Get("MetadataToken");
        private static readonly XName MethodNameElementName = XName.Get("Method");
        private static readonly XName MethodsNameElementName = XName.Get("Methods");
        private static readonly XName ModuleElementName = XName.Get("Module");
        private static readonly XName ModuleNameElementName = XName.Get("ModuleName");
        private static readonly XName ModulesElementName = XName.Get("Modules");
        private static readonly XName NameElementName = XName.Get("Name");
        private static readonly XName SequencePointName = XName.Get("SequencePoint");
        private static readonly XName SequencePointsName = XName.Get("SequencePoints");
        private static readonly XName SummaryElementName = XName.Get("Summary");

        private static readonly XName XmlnsXsdAttributeName = XNamespace.Xmlns + "xsd";
        private static readonly XName XmlnsXsiAttributeName = XNamespace.Xmlns + "xsi";

        private static readonly XName branchCoverageAttributeName = XName.Get("branchCoverage");
        private static readonly XName cyclomaticComplexityAttributeName = XName.Get("cyclomaticComplexity");
        private static readonly XName ecAttributeName = XName.Get("ec");
        private static readonly XName elAttributeName = XName.Get("el");
        private static readonly XName fullPathAttributeName = XName.Get("fullPath");
        private static readonly XName isConstructorAttributeName = XName.Get("isConstructor");
        private static readonly XName isGetterAttributeName = XName.Get("isGetter");
        private static readonly XName isSetterAttributeName = XName.Get("isSetter");
        private static readonly XName isStaticAttributeName = XName.Get("isStatic");
        private static readonly XName maxCyclomaticComplexityAttributeName = XName.Get("maxCyclomaticComplexity");
        private static readonly XName minCyclomaticComplexityAttributeName = XName.Get("minCyclomaticComplexity");
        private static readonly XName numBranchPointsAttributeName = XName.Get("numBranchPoints");
        private static readonly XName numSequencePointsAttributeName = XName.Get("numSequencePoints");
        private static readonly XName offsetAttributeName = XName.Get("offset");
        private static readonly XName offsetchainAttributeName = XName.Get("offsetchain");
        private static readonly XName offsetendAttributeName = XName.Get("offsetend");
        private static readonly XName ordinalAttributeName = XName.Get("ordinal");
        private static readonly XName pathAttributeName = XName.Get("path");
        private static readonly XName sequenceCoverageAttributeName = XName.Get("sequenceCoverage");
        private static readonly XName scAttributeName = XName.Get("sc");
        private static readonly XName slAttributeName = XName.Get("sl");
        private static readonly XName uidAttributeName = XName.Get("uid");
        private static readonly XName uspidAttributeName = XName.Get("uspid");
        private static readonly XName vcAttributeName = XName.Get("vc");
        private static readonly XName visitedAttributeName = XName.Get("visited");
        private static readonly XName visitedBranchPointsAttributeName = XName.Get("visitedBranchPoints");
        private static readonly XName visitedSequencePointsAttributeName = XName.Get("visitedSequencePoints");

        #endregion

        public string Serialize(CoverageResult result)
        {
            var document = new XDocument(CreateCoverageSessionElement(result));

            using (var ms = new MemoryStream())
            {
                var xmlSettings = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Encoding = new UTF8Encoding(false),
                    Indent = true,
                    IndentChars = "  "
                };

                using (var writer = XmlWriter.Create(ms, xmlSettings))
                {
                    document.WriteTo(writer);

                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private XElement CreateCoverageSessionElement(CoverageResult result)
            => new XElement(CoverageSessionElementName,
                new XAttribute(XmlnsXsdAttributeName, "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XmlnsXsiAttributeName, "http://www.w3.org/2001/XMLSchema-instance"),
                CreateSummaryElement(result),
                CreateModulesElement(result)
            );

        private XElement CreateSummaryElement(CoverageResult result)
            => CreateSummaryElement(
                numSequencePoints: result.Batches.Sum(p => p.StatementCount),
                visitedSequencePoints: result.Batches.Sum(p => p.CoveredStatementCount),
                numBranchPoints: result.Batches.Sum(p => p.BranchesCount),
                visitedBranchPoints: result.Batches.Sum(p => p.CoveredBranchesCount)
            );

        private XElement CreateSummaryElement(Batch batch)
            => CreateSummaryElement(
                numSequencePoints: batch.StatementCount,
                visitedSequencePoints: batch.CoveredStatementCount,
                numBranchPoints: batch.BranchesCount,
                visitedBranchPoints: batch.CoveredBranchesCount
            );

        private XElement CreateSummaryElement(
            long numSequencePoints,
            long visitedSequencePoints,
            long numBranchPoints,
            long visitedBranchPoints)
            => new XElement(
                SummaryElementName,
                new XAttribute(numSequencePointsAttributeName, numSequencePoints),
                new XAttribute(visitedSequencePointsAttributeName, visitedSequencePoints),
                new XAttribute(numBranchPointsAttributeName, numBranchPoints),
                new XAttribute(visitedBranchPointsAttributeName, visitedBranchPoints),
                new XAttribute(sequenceCoverageAttributeName, numSequencePoints == 0 ? 0 : visitedSequencePoints / (double)numSequencePoints * 100.0),
                new XAttribute(branchCoverageAttributeName, numBranchPoints == 0 ? 0 : visitedBranchPoints / (double)numBranchPoints * 100.0),
                new XAttribute(maxCyclomaticComplexityAttributeName, "0"),
                new XAttribute(minCyclomaticComplexityAttributeName, "0")
            );

        private XElement CreateModulesElement(CoverageResult result)
        {
            var databaseName = result.DatabaseName;

            var hash = string.Join(
                "-",
                SHA1.Create().ComputeHash(
                    Encoding.UTF8.GetBytes(databaseName)
                ).Select(x => x.ToString("X2")).ToArray()
            );

            return new XElement(
                ModulesElementName,
                new XElement(
                    ModuleElementName,
                    new XAttribute("hash", hash),
                    new XElement(FullNameElementName, databaseName),
                    new XElement(ModuleNameElementName, databaseName),
                    CreateFilesElement(result.Batches),
                    CreateClassesElement(result.Batches)
                )
            );
        }

        private XElement CreateFilesElement(IEnumerable<Batch> batches)
            => new XElement(
                FilesElementName,
                batches.Select((x, i) => CreateFileElement(x.ObjectId, x.ObjectName))
            );

        private XElement CreateFileElement(int uid, string fullPath)
            => new XElement(
                FileElementName,
                new XAttribute(uidAttributeName, uid),
                new XAttribute(fullPathAttributeName, fullPath)
            );

        private XElement CreateClassesElement(IEnumerable<Batch> batches)
            => new XElement(
                ClassesElementName,
                batches.Select((x, i) => CreateClassElement(x))
            );

        private XElement CreateClassElement(Batch batch)
        {
            return new XElement(
                ClassElementName,
                CreateSummaryElement(batch),
                new XElement(FullNameElementName, batch.ObjectName),
                CreateMethodsElement(batch)
            );
        }

        private XElement CreateMethodsElement(Batch batch)
            => new XElement(
                MethodsNameElementName,
                new XElement(
                    MethodNameElementName,
                    new XAttribute(visitedAttributeName, batch.CoveredStatementCount > 0),
                    new XAttribute(cyclomaticComplexityAttributeName, "0"),
                    new XAttribute(sequenceCoverageAttributeName, batch.StatementCount == 0 ? 0 : batch.CoveredStatementCount / (double)batch.StatementCount * 100.0),
                    new XAttribute(branchCoverageAttributeName, batch.BranchesCount == 0 ? 0 : batch.CoveredBranchesCount / (double)batch.BranchesCount * 100.0),
                    new XAttribute(isConstructorAttributeName, "false"),
                    new XAttribute(isStaticAttributeName, "false"),
                    new XAttribute(isGetterAttributeName, "true"),
                    new XAttribute(isSetterAttributeName, "false"),
                    CreateSummaryElement(batch),
                    new XElement(MetadataTokenElementName, "01041980"),
                    new XElement(NameElementName, batch.ObjectName),
                    new XElement(
                        FileRefName,
                        new XAttribute(uidAttributeName, batch.ObjectId)
                    ),
                    CreateSequencePointsElement(batch),
                    CreateBranchPointsElement(batch)
                )
            );

        private XElement CreateSequencePointsElement(Batch batch)
            => new XElement(
                SequencePointsName,
                batch.Statements.Select((x, i) => CreateSequencePointElement(batch, x, i + 1))
            );

        private XElement CreateSequencePointElement(Batch batch, Statement statement, int ordinal)
        {
            var offsets = CoverageResult.GetOffsets(statement, batch.Text);

            return new XElement(
                SequencePointName,
                new XAttribute(vcAttributeName, statement.HitCount),
                new XAttribute(uspidAttributeName, uniqueId++),
                new XAttribute(ordinalAttributeName, ordinal),
                new XAttribute(offsetAttributeName, statement.Offset),
                new XAttribute(slAttributeName, offsets.StartLine),
                new XAttribute(scAttributeName, offsets.StartColumn),
                new XAttribute(elAttributeName, offsets.EndLine),
                new XAttribute(ecAttributeName, offsets.EndColumn)
            );
        }


        private XElement CreateBranchPointsElement(Batch batch)
        {
            var ordinal = 1;

            return new XElement(
                BranchPointsElementName,
                batch.Statements
                    .Where(x => x.Branches.Any())
                    .SelectMany((statement, i) =>
                        statement.Branches.Select((branch, j) => CreateBranchPointElement(batch, statement, branch, ordinal++, j))
                    )
            );
        }

        private XElement CreateBranchPointElement(Batch batch, Statement statement, Branch branch, int ordinal, int path)
        {
            var offsets = CoverageResult.GetOffsets(statement, batch.Text);
            var offsetEnd = branch.Offset + branch.Length;

            return new XElement(
                BranchPointElementName,
                new XAttribute(vcAttributeName, branch.HitCount),
                new XAttribute(uspidAttributeName, uniqueId++),
                new XAttribute(ordinalAttributeName, ordinal),
                new XAttribute(offsetAttributeName, branch.Offset),
                new XAttribute(slAttributeName, offsets.StartLine),
                new XAttribute(pathAttributeName, path),
                new XAttribute(offsetchainAttributeName, ""),
                new XAttribute(offsetendAttributeName, branch.Offset + branch.Length)
            );
        }
    }
}
