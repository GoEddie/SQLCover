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
            [Option('r', "requiredParams", Required = false, HelpText = "Get required parameters for a command")]
            public bool GetRequiredParameters { get; set; }
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

                       if (cType != CommandType.Unknown)
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
    }
}
