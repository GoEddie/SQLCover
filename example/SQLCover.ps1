function Get-CoverTSql{
    param(
            [string]$coverDllPath
            ,[string]$connectionString
            ,[string]$databaseName
            ,[string]$query
        )

    if(!(Test-Path $coverDllPath)){
        Write-Error "SQLCover.dll path was not found ($coverDllPath)"
        return
    }
    
    Add-Type -Path $coverDllPath
    
    $coverage = new-object SQLCover.CodeCoverage ($connectionString, $databaseName)
	$coverage.Cover($query)
  
}

function Get-CoverExe{
    param(
            [string]$coverDllPath
            ,[string]$connectionString
            ,[string]$databaseName
            ,[string]$exeName
            ,[string]$args
        )

    if(!(Test-Path $coverDllPath)){
        Write-Error "SQLCover.dll path was not found ($coverDllPath)"
        return
    }
    
    Add-Type -Path $coverDllPath
    
    $coverage = new-object SQLCover.CodeCoverage ($connectionString, $databaseName)
	$coverage.CoverExe($exeName, $args)
  
}

function Get-CoverRedgateCITest{
    param(
        [string]$coverDllPath
        ,[string]$connectionString
        ,[string]$nugetPackage
        ,[string]$server
        ,[string]$database
        )
    
        if(!(Get-Module -ListAvailable -Name "SQLRelease"))
        { 
            Write-Error "Redgate SQLRelease Module missing." 
            return    
        }
        
        Import-Module -Name "SQLRelease"
        
        if(!(Test-Path $coverDllPath)){
            Write-Error "SQLCover.dll path was not found ($coverDllPath)"
            return
        }      
        
        Add-Type -Path $coverDllPath
        
        $coverage = new-object SQLCover.CodeCoverage($connectionString, $database)
        $coverage.Start()

        $temporaryDatabase = New-DlmDatabaseConnection -ServerInstance $server -Database $database
        $testResults = $nugetPackage | Invoke-DlmDatabaseTests  -TemporaryDatabase $temporaryDatabase
        $coverageResults = $coverage.Stop()

        $testResults, $coverageResults
}
    
function Export-OpenXml{
    param(        
        [SQLCover.CoverageResult] $result
        ,[string]$outputPath
    )

    $xmlPath = Join-Path -Path $outputPath -ChildPath "Coverage.opencoverxml"
    $result.OpenCoverXml() | Out-File $xmlPath
    $result.SaveSourceFiles($outputPath)    
}

function Start-ReportGenerator{
    param(        
        [string]$outputPath
        ,[string]$reportGeneratorPath
    )
    
    $xmlPath = Join-Path -Path $outputPath -ChildPath "Coverage.opencoverxml"
    $sourcePath = $outputPath
    
    if(!(Test-Path $sourcePath)){
        Write-Error "Cannot find source path to convert into html report. Path = $sourcePath"
    }
        
    $outputPath = Join-Path $outputPath -ChildPath "out"
    New-Item -Type Directory -Force -Path $outputPath | out-Null
        
    $report = "-reports:$xmlPath"
    $targetDir = "-targetDir:$outputPath" 
    $args = $report, $targetDir
    Start-Process -FilePath $reportGeneratorPath -ArgumentList $args -WorkingDirectory $sourcePath -Wait
    Write-Verbose "Coverage Report Written to: $outputPath"
}

function Export-Html{
    param(        
        [SQLCover.CoverageResult] $result
        ,[string]$outputPath
    )

    $xmlPath = Join-Path -Path $outputPath -ChildPath "Coverage.html"
    $result.Html() | Out-File $xmlPath
    $result.SaveSourceFiles($outputPath)    
}


#EXAMPLE:
<#
    #Use the Redgate DLM suite to deploy a nuget package and run the tSQLt tests
    $results = Get-CoverRedgateCITest "path\to\SQLCover.dll" "server=.;integrated security=sspi;initial catalog=tSQLt_Example" "tSQLt_Example"
    Export-DlmDatabaseTestResults $results[0] -OutputFile c:\temp\junit.xml -force
    Export-OpenXml $results[1] "c:\output\path\for\xml\results"
    Start-ReportGenerator "c:\output\path\for\xml\results" "c:\path\to\reportgenerator.exe"
    
    #Cover whatever sql query you would like to run, this example runs tSQLt.RunAll
    $result = Get-CoverTSql "path\to\SQLCover.dll" "server=.;integrated security=sspi;initial catalog=tSQLt_Example" "tSQLt_Example" "tSQLt.RunAll"
    #Output the results as a basic html file
    Export-Html $result "path\to\write\Log"

    $result = Get-CoverTSql "path\to\SQLCover.dll" "server=.;integrated security=sspi;initial catalog=tSQLt_Example" "tSQLt_Example" "tSQLt.RunAll"
    Export-OpenXml $result "output\path\for\xml\results"
    Start-ReportGenerator "output\path\for\xml\results" "path\to\reportgenerator.exe"


#>



