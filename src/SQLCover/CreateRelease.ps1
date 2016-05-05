C:\"Program Files (x86)"\"Windows Kits"\8.1\bin\x86\signtool.exe sign /t http://timestamp.globalsign.com/scripts/timstamp.dll /a "$(pwd)\SQLCover\bin\release\SQLCover.dll"
cp  "$(pwd)\SQLCover\bin\release\SQLCover.dll"  "$(pwd)\releases\template"


C:\"Program Files (x86)"\"Windows Kits"\8.1\bin\x86\signtool.exe verify /v /a "$(pwd)\releases\template\SQLCover.dll"


function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

if(Test-Path("$(pwd)\releases\SQLCover.zip")){
	del "$(pwd)\releases\SQLCover.zip"
}

ZipFiles "$(pwd)\releases\SQLCover.zip" "$(pwd)\releases\template"

ls "$(pwd)\releases\SQLCover.zip"

