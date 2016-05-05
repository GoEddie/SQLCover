using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using SQLCover.Gateway;
using SQLCover.Objects;
using SQLCover.Parsers;

namespace SQLCover.Source
{
    public class DatabaseSourceGateway : SourceGateway
    {
        private readonly DatabaseGateway _databaseGateway;

        public DatabaseSourceGateway(DatabaseGateway databaseGateway)
        {
            _databaseGateway = databaseGateway;
        }

        public SqlServerVersion GetVersion()
        {
            var compatibilityString = _databaseGateway.GetString("select compatibility_level from sys.databases where database_id = db_id();");
            SqlServerVersion res;
            if (Enum.TryParse(string.Format("Sql{0}", compatibilityString), out res))
            {
                return res;
            }

            return SqlServerVersion.Sql130;
        }


        public bool IsAzure()
        {
            var versionString = _databaseGateway.GetString("select @@version");
            return versionString.Contains("Azure");
        }

        public IEnumerable<Batch> GetBatches(List<string> objectFilter)
        {
            var table =
                _databaseGateway.GetRecords(
                    "select object_id, \'[\' + object_schema_name(object_id) + \'].[\' + object_name(object_id) + \']\' as object_name, definition, uses_quoted_identifier from sys.sql_modules where object_id not in (select object_id from sys.objects where type = 'IF')");

            var batches = new List<Batch>();
            
            var version = GetVersion();
            var excludedObjects = GetExcludedObjects();
            if(objectFilter == null)
                objectFilter = new List<string>();

            objectFilter.Add(".*tSQLt.*");

            foreach (DataRow row in table.Rows)
            {
                var quoted = (bool) row["uses_quoted_identifier"];
                
                var name = (string) row["object_name"];
                    
                if (DoesNotMatchFilter(name, objectFilter, excludedObjects))
                {
                    batches.Add(
                        new Batch(new StatementParser(version), quoted, EndDefinitionWithNewLine((string)row["definition"]), null, name, (int) row["object_id"]));
                }
                
            }

            table.Dispose();

            foreach (var batch in batches)
            {
                batch.StatementCount = batch.Statements.Count(p => p.IsCoverable);
            }

            return batches.Where(p=>p.StatementCount > 0);
        }

        private static string EndDefinitionWithNewLine(string definition)
        {
            if (definition.EndsWith("\r\n\r\n"))
                return definition;

            return definition + "\r\n\r\n";
        }

        private List<string> GetExcludedObjects()
        {
            var tSQLtObjects =
                _databaseGateway.GetRecords(
                    @"select  '[' + object_schema_name(object_id) + '].[' + object_name(object_id) + ']' as object_name from sys.procedures
	where schema_id in (
select major_id from sys.extended_properties ep
	where class_desc = 'SCHEMA' and name = 'tSQLt.TestClass' )");

            var excludedObjects = new List<string>();

            foreach (DataRow row in tSQLtObjects.Rows)
            {
                excludedObjects.Add(row[0].ToString().ToLowerInvariant());
            }

            return excludedObjects;

        }

        private bool DoesNotMatchFilter(string name, List<string> objectFilter, List<string> excludedObjects)
        {
            var lowerName = name.ToLowerInvariant();

            foreach (var filter in objectFilter)
            {
                if (Regex.IsMatch(name, (string) (filter ?? ".*")))
                    return false;
            }

            foreach (var filter in excludedObjects)
            {
                if (filter == lowerName)
                    return false;
            }
            
            return true;
        }
    }
}