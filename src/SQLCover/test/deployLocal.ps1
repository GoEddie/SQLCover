param(
    $configuration = 'Debug'
)
#when i run locally i want debug but from he build agent i want debug, if i default to debug then i dont need to pass it in locally and am happy for the yaml build definition to include it

Function Get-SqlPackagePath{

    #WHY? I could have a nuget package that downloads sqlpackage but I know it is on every dev machine and the build agents so just sniff it

    if(Test-Path "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"){
        "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"
        return
    }

    if(Test-Path "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"){
        "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"
        return
    }


    if(Test-Path "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"){
        "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\Microsoft\SQLDB\DAC\140\sqlpackage.exe"
        return
    }

}

&"$(Get-SqlPackagePath)" /Action:Publish /SourceFile:"$($PSCommandPath | Split-Path -Parent)\..\DatabaseProject\bin\$($configuration)\DatabaseProject.dacpac" /TargetServerName:"(localdb)\SQLCover" /TargetDatabaseName:DatabaseProject