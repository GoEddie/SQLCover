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
            return process.StandardOutput.ReadToEnd();

        }

        private static string Get(string name)
        {
            var connectionString = $"Data Source={GetPipeName()};integrated security=sspi;initial catalog=DatabaseProject";
            Console.WriteLine("Connection String = " + connectionString);
            return connectionString;
         }


        public static string GetIntegration()
        {
            return Get("Integration");
        }
        
        private static string GetFilename()
        {
            //If available use the user file
            if (System.IO.File.Exists("ConnectionString.user.config"))
            {
                return "ConnectionString.user.config";
            }
            else if (System.IO.File.Exists("ConnectionString.config"))
            {
                return "ConnectionString.config";
            }
            return "";
        }
        
    }
}
