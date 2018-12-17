using System;
using System.Xml;

namespace SQLCover.IntegrationTests
{
    class ConnectionStringReader
    {
        private static string GetPipeName()
        {

            var processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.Arguments = "-Command \"&{ (sqllocaldb i SQLCover |%{ if ($_.Contains('pipe')){$_} }).Split(':')[2]}\"";
            
            processInfo.FileName = "powershell.exe";
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            var process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd().Replace("\r", "").Replace("\n", "");

        }

   
        public static string GetIntegration()
        {
            var connectionString = $"Server=np:{GetPipeName()};integrated security=sspi;initial catalog=DatabaseProject";
            Console.WriteLine("Connection String = " + connectionString);
            return connectionString;
        }
        
        
    }
}
