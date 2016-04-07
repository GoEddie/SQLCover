SQLCover
========

 

Code coverage for SQL Server T-SQL
----------------------------------

 

This is a code coverage tool for SQL Server 2008+, it was designed to be generic
to work with any build server and tests but includes specific filters to mean
that it is well suited to running tSQLt tests using the Redgate DLM Automation
Suite.

 

Installation
------------

You will either need to build the project and grab the output SQLCover.dll or
you can download the pre-built binary from:

 

http://the.agilesql.club/SQLCover/download.php

 

Usage
-----

There are three basic ways to use it:

 

### 1. Redgate DLM Automation Suite

If you have the DLM automation suite then create a nuget package of your
database, deploy the project to a test database and then use the example
powershell script
(https://github.com/GoEddie/SQLCover/blob/master/example/SQLCover.ps1 and
included in the download above):

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Get-CoverRedgateCITest "SQLCover-path.dll" "server=servername;integrated security=sspi;" "nuget-package-path.nupkg" "servername" "database-name"
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

To create the nupkg of your database you can use sqlci.exe or create a zip of
your .sql files see:

 

https://www.simple-talk.com/blogs/2014/12/18/using\_sql\_release\_with\_powershell/

 

The Get-CoverRedgateCITest will return an array with two objects in, the first
object is a:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
RedGate.SQLRelease.Compare.SchemaTesting.TestResults
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

The second object is a:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
SQLCover.CoverageResult
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

This has two public properties:

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public long StatementCount;
    public long CoveredStatementCount;
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

It also has two public methods:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public string Html()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

This creates a basic html report to view the code coverage, highlighting the
lines of code in the database which have been covered and:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public string OpenCoverXml()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

which creates an xml file in the OpenCoverageXml format which can be converted
into a very pretty looking report using reportgenerator:

 

https://github.com/danielpalme/ReportGenerator

 

For a complete example see:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
$results = Get-CoverRedgateCITest "path\to\SQLCover.dll" "server=.;integrated security=sspi;initial catalog=tSQLt_Example" "tSQLt_Example"
    Export-DlmDatabaseTestResults $results[0] -OutputFile c:\temp\junit.xml -force
    Export-OpenXml $results[1] "c:\output\path\for\xml\results"
    Start-ReportGenerator "c:\output\path\for\xml\results" "c:\path\to\reportgenerator.exe"
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

### 2. Cover T-SQL Script

If you have a script you want to cover then you can call:

 

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Get-CoverTSql  "SQLCover-path.dll" "server=servername;integrated security=sspi;"  "database-name" "exec tSQLt.RunAll
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

This will give you a CoverageResults where you can either examine the amount of
statement covered or output the full html or xml report.

 

### 3. Cover anything else

If you want to have more control over what is covered, you can start a coverage
session, run whatever queries you like from whatever application and then stop
the coverage trace and get the CoverageResults which you can then use to
generate a report.

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
$coverage = new-object SQLCover.CodeCoverage($connectionString, $database)
$coverage.Start()
#DO SOMETHING HERE
$coverageResults = $coverage.Stop()
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

 

 

A final word...
---------------

Thanks to Redgate for sponsoring the open source project
