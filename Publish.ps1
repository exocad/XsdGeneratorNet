param ([Parameter(Mandatory)][String] $BuildScriptsDir)

$configuration = "Release"
$tfm = "net6.0-windows"
$publishDir = "./artifacts/publish/XsdGeneratorTool/$($configuration)_$($tfm)"
$xsdGeneratorToolPath = [System.IO.Path]::Combine($publishDir, "XsdGeneratorTool.exe")
$exocertFilePath = [System.IO.Path]::Combine($BuildScriptsDir, "exocert_file.bat")

dotnet publish source/XsdGeneratorTool/XsdGeneratorTool.csproj -c $configuration -f $tfm --sc false -p:PublishTrimmed=false -p:PublishSingleFile=true

Write-Host ""
Write-Host "Signing XsdGeneratorTool.exe"
Write-Host ""

Start-Process -FilePath $exocertFilePath $xsdGeneratorToolPath -Wait -NoNewWindow
