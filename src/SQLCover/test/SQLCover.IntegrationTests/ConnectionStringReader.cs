using System;
using System.IO;
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

        private static string GetContainerIP()
        {

            var processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.Arguments = "-Command \"&{ (docker inspect SQLCover | ConvertFrom-Json).NetworkSettings.Networks.nat.IPAddress}\"";

            processInfo.FileName = "powershell.exe";
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            var process = System.Diagnostics.Process.Start(processInfo);
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd().Replace("\r", "").Replace("\n", "");

        }
        



        public static string GetIntegration()
        {
            ///This is a text file with a single line of text with the connection string
            ///   I know it is weird but simple :)
            var localOverrideFile =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConnectionString.user.config");


            if (File.Exists(localOverrideFile))
            {
                return File.ReadAllText(localOverrideFile);
            }
            else
            {
                // var connectionString = $"Server=np:{GetPipeName()};integrated security=sspi;initial catalog=DatabaseProject";
                //var connectionString = $"Server=tcp:{GetContainerIP()};uid=sa;pwd=Psgsgsfsfs!!!!!;initial catalog=DatabaseProject";
                var connectionString = "Server=(localdb)\\SQLCover;integrated security=SSPI;initial catalog=DatabaseProject";
                return connectionString;
            }
        }
        
        
    }
}
