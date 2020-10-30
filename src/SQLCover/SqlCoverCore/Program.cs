﻿using CommandLine;
using SQLCoverCore;
using System;

namespace SQLCover.Core
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('c', "command", Required = true, HelpText = "Choose command to run from:Get-CoverTSql, Get-CoverExe, Get-CoverRedgateCITest.")]
            public string Command { get; set; }
            [Option('e', "exportCommand", Required = true, HelpText = "Choose command to run from:Export-OpenXml, Start-ReportGenerator, Export-Html.")]
            public string ExportCommand { get; set; }
            [Option('b', "debug", Required = false, HelpText = "Prints out more output.")]
            public bool Debug { get; set; }
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
                       var eType = CommandType.Unknown;
                       string[] requiredParameters = null;
                       string[] requiredExportParameters = null;
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
                           default:
                               Console.WriteLine("Command:" + o.Command + " is not supported");
                               break;
                       }

                       switch (o.ExportCommand)
                       {
                           case "Export-OpenXml":
                               eType = CommandType.ExportOpenXml;
                               requiredExportParameters = new string[]{"coverDllPath",
                                "connectionString",
                                "databaseName",
                                "exeName",
                                "args"};
                               break;
                           case "Start-ReportGenerator":
                               eType = CommandType.StartReportGenerator;
                               requiredExportParameters = new string[]{"outputPath",
                                "reportGeneratorPath"};
                               break;
                           case "Export-Html":
                               eType = CommandType.ExportHtml;
                               requiredExportParameters = new string[]{"result",
                                "outputPath"};
                               break;
                           default:
                               Console.WriteLine("ExportCommand:" + o.Command + " is not supported");
                               break;
                       }

                       var validParams = eType != CommandType.Unknown && cType != CommandType.Unknown ? validateRequired(o, requiredParameters) : false;
                       validParams = validParams ? validateRequired(o, requiredExportParameters) : validParams;

                       if (validParams)
                       {
                           if (o.GetRequiredParameters)
                           {
                               Console.WriteLine(o.Command + " requiredParameters are:" + string.Join(',', requiredParameters));
                               Console.WriteLine(o.ExportCommand + " requiredParameters are:" + string.Join(',', requiredExportParameters));
                           }
                           else
                           {
                               Console.WriteLine(":::Running command" + cType.ToString() + ":::");
                               CodeCoverage coverage = null;
                               CoverageResult results = null;
                               // run command
                               switch (cType)
                               {
                                   case CommandType.GetCoverTSql:

                                       coverage = new CodeCoverage(o.ConnectionString, o.databaseName, null, true, o.Debug);
                                       results = coverage.Cover(o.Query);

                                       break;
                                   case CommandType.GetCoverExe:
                                   case CommandType.GetCoverRedgateCITest:
                                       Console.WriteLine(cType.ToString() + " is not YET supported");
                                       break;
                               }
                               if (coverage != null)
                               {
                                   Console.WriteLine(":::Running exportCommand" + eType.ToString() + ":::");
                                   var resultString = "";
                                   switch (eType)
                                   {
                                       case CommandType.ExportOpenXml:
                                           resultString = results.OpenCoverXml();
                                           break;
                                       case CommandType.ExportHtml:
                                           resultString = results.Html();
                                           break;
                                       case CommandType.StartReportGenerator:
                                           Console.WriteLine(eType.ToString() + " is not YET supported");
                                           break;
                                   }
                               }
                           }
                       }
                   });
        }

        private static bool validateRequired(Options o, string[] requiredParameters, bool export = false)
        {
            var valid = true;
            var requiredString = export ? "is required for this exportCommand" : "is required for this command";
            foreach (var param in requiredParameters)
            {
                switch (param)
                {
                    case "connectionString":
                        if (string.IsNullOrWhiteSpace(o.ConnectionString))
                        {
                            Console.WriteLine("connectionString" + requiredString);
                            valid = false;
                        }
                        break;
                    case "databaseName":
                        if (string.IsNullOrWhiteSpace(o.databaseName))
                        {
                            Console.WriteLine("databaseName" + requiredString);
                            valid = false;
                        }
                        break;
                    case "query":
                        if (string.IsNullOrWhiteSpace(o.Query))
                        {
                            Console.WriteLine("query" + requiredString);
                            valid = false;
                        }
                        break;
                    case "result":
                        if (string.IsNullOrWhiteSpace(o.Result))
                        {
                            Console.WriteLine("result" + requiredString);
                            valid = false;
                        }
                        break;
                    case "outputPath":
                        if (string.IsNullOrWhiteSpace(o.OutputPath))
                        {
                            Console.WriteLine("outputPath" + requiredString);
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
