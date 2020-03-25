using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLCover
{
    class Program
    {
        static void Main(string[] args)
        {
            //CodeCoverage cover = new CodeCoverage("Server=localhost;Trusted_Connection=True", "MBITEST", new string[] { ".*MSins.*", ".*MSDel.*", ".*MSupd.*", ".*MSdel.*", ".*_AFT.*", ".*FLTR_AUDIT.*", ".*FLTR_ODS.*", ".*tr_MSTran_.*" }, false,false, 0, "SELECT team_name sql_group, '[dbo].[' + object_name + ']' object_name FROM dbo.TEAM_OWNERSHIP T1 INNER JOIN dbo.TEAM T on T1.team_key = T.team_key ");

            //CodeCoverage cover = new CodeCoverage("Server=hcs-dev1-dbs1;Trusted_Connection=True", "MBITIME401", new string[] { ".*MSins.*", ".*MSDel.*", ".*MSupd.*", ".*MSdel.*", ".*_AFT.*", ".*FLTR_AUDIT.*", ".*FLTR_ODS.*", ".*tr_MSTran_.*" }, false, false, 0, " USE MBITEST; SELECT OBJECT_SCHEMA_NAME(sm.object_id) sql_group, ISNULL('[' + OBJECT_SCHEMA_NAME(sm.object_id) + '].[' + OBJECT_NAME(sm.object_id) + ']', '[' + st.name + ']')  object_name FROM sys.sql_modules sm LEFT JOIN sys.triggers st ON st.object_id = sm.object_id WHERE sm.object_id NOT IN(SELECT object_id FROM sys.objects WHERE type = 'IF') AND OBJECT_SCHEMA_NAME(sm.object_id)  is not null");
            CodeCoverage cover = new CodeCoverage("Server=hcs-dev-dbs1;Trusted_Connection=True", "MBITEST", new string[] { ".*MSins.*", ".*MSDel.*", ".*MSupd.*", ".*MSdel.*", ".*_AFT.*", ".*FLTR_AUDIT.*", ".*FLTR_ODS.*", ".*tr_MSTran_.*" }, false, false, 0, "SELECT team_name sql_group, '[dbo].[' + object_name + ']' object_name FROM TEAM_OWNERSHIP T1 INNER JOIN TEAM T on T1.team_key = T.team_key ");

            cover.Start();


            cover.Stop();

            File.WriteAllText(@"C:\Users\tgruetzmacher\OneDrive - Alegeus Technologies LLC\7.15\Unit Testing\CoverageResults\cobertura.xml", cover.Results().Cobertura());
            cover.Results().SaveSourceFiles(@"C: \Users\tgruetzmacher\OneDrive - Alegeus Technologies LLC\7.15\Unit Testing\CoverageResults\sources");
            //File.WriteAllText(@"C:\Users\tgruetzmacher\OneDrive - Alegeus Technologies LLC\7.15\Unit Testing\openCover.xml", cover.Results().OpenCoverXml());



            return;
        }

    }
}
