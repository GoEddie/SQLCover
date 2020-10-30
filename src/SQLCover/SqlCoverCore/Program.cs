using CommandLine;
using System;

namespace SQLCover.Core
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('c', "command", Required = true, HelpText = "Choose command to run from:Get-CoverTSql, Get-CoverExe, Get-CoverRedgateCITest, Export-OpenXml, Start-ReportGenerator, Export-Html.")]
            public string Command { get; set; }
            [Option('p', "requiredParams", Required = false, HelpText = "Get required parameters for a command")]
            public bool GetRequiredParameters { get; set; }
            [Option('k', "connectionString", Required = false, HelpText = "Connection String to the sql server")]
            public string ConnectionString { get; set; }
            [Option('d', "databaseName", Required = false, HelpText = "Default Database")]
            public string databaseName { get; set; }
            [Option('q', "query", Required = false, HelpText = "Sql Query, try tSQLt.runAll")]
            public string Query { get; set; }
            [Option('r', "result", Required = false, HelpText = "Result string")]
            public string Result { get; set; }
            [Option('p', "outputPath", Required = false, HelpText = "Output Path")]
            public string OutputPath { get; set; }
        }
        private enum CommandType
        {
            GetCoverTSql,
            GetCoverExe,
            GetCoverRedgateCITest,
            ExportOpenXml,
            StartReportGenerator,
            ExportHtml,
            Unknown
        }
        /// <summary>
        /// should minic arguments from example\SQLCover.ps1
        /// run by `dotnet run -- -c Get-CoverTSql -r`
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.Verbose)
                       {
                           Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                           Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                       }
                       else
                       {
                           Console.WriteLine($"Current Arguments: -v {o.Verbose} -c {o.Command}");
                           Console.WriteLine(":::Running SqlCoverCore:::");
                       }
                       var cType = CommandType.Unknown;
                       string[] requiredParameters = null;
                       switch (o.Command)
                       {
                           case "Get-CoverTSql":
                               cType = CommandType.GetCoverTSql;
                               requiredParameters = new string[]{"coverDllPath",
                                "connectionString",
                                "databaseName",
                                "query"};
                               break;
                           case "Get-CoverExe":
                               cType = CommandType.GetCoverExe;
                               requiredParameters = new string[]{"coverDllPath",
                                "connectionString",
                                "databaseName",
                                "exeName",
                                "args"};
                               break;
                           case "Get-CoverRedgateCITest":
                               cType = CommandType.GetCoverRedgateCITest;
                               requiredParameters = new string[]{"result",
                                "outputPath"};
                               break;
                           case "Export-OpenXml":
                               cType = CommandType.ExportOpenXml;
                               requiredParameters = new string[]{"coverDllPath",
                                "connectionString",
                                "databaseName",
                                "exeName",
                                "args"};
                               break;
                           case "Start-ReportGenerator":
                               cType = CommandType.StartReportGenerator;
                               requiredParameters = new string[]{"outputPath",
                                "reportGeneratorPath"};
                               break;
                           case "Export-Html":
                               cType = CommandType.ExportHtml;
                               requiredParameters = new string[]{"result",
                                "outputPath"};
                               break;
                           default:
                               Console.WriteLine(o.Command + " is not supported");
                               break;
                       }

                       var validParams = cType != CommandType.Unknown ? validateRequired(o, requiredParameters) : false;

                       if (validParams)
                       {
                           if (o.GetRequiredParameters)
                           {
                               Console.WriteLine(o.Command + " requiredParameters are:" + string.Join(',', requiredParameters));
                           }
                           else
                           {
                               // run command
                               switch (cType)
                               {
                                   case CommandType.GetCoverTSql:
                                   case CommandType.GetCoverExe:
                                   case CommandType.GetCoverRedgateCITest:
                                   case CommandType.ExportOpenXml:
                                   case CommandType.StartReportGenerator:
                                   case CommandType.ExportHtml:
                                       Console.WriteLine(cType.ToString() + " is not YET supported");
                                       break;
                               }
                           }
                       }
                   });
        }

        private static bool validateRequired(Options o, string[] requiredParameters)
        {
            var valid = true;
            foreach (var param in requiredParameters)
            {
                switch (param)
                {
                    case "connectionString":
                        if (string.IsNullOrWhiteSpace(o.ConnectionString))
                        {
                            Console.WriteLine("connectionString is required for this command");
                            valid = false;
                        }
                        break;
                    case "databaseName":
                        if (string.IsNullOrWhiteSpace(o.databaseName))
                        {
                            Console.WriteLine("databaseName is required for this command");
                            valid = false;
                        }
                        break;
                    case "query":
                        if (string.IsNullOrWhiteSpace(o.Query))
                        {
                            Console.WriteLine("query is required for this command");
                            valid = false;
                        }
                        break;
                    case "result":
                        if (string.IsNullOrWhiteSpace(o.Result))
                        {
                            Console.WriteLine("result is required for this command");
                            valid = false;
                        }
                        break;
                    case "outputPath":
                        if (string.IsNullOrWhiteSpace(o.OutputPath))
                        {
                            Console.WriteLine("outputPath is required for this command");
                            valid = false;
                        }
                        break;
                    default:
                        Console.WriteLine("Required check on:" + param + " ignored");
                        // will always be invalid for commands not validated on a required param
                        valid = false;
                        break;
                }
            }
            return valid;
        }
    }
}
