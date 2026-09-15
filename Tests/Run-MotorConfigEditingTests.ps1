param([string]$ReportPath, [Parameter(Mandatory=$true)][string]$InputPartList)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path ([IO.Path]::GetTempPath()) ('MotorConfigTests-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot | Out-Null
$runtime = Join-Path $root 'FAFramework/bin/Debug'
Copy-Item -LiteralPath (Join-Path $runtime 'FAFramework.exe') -Destination $testRoot
Get-ChildItem -LiteralPath $runtime -Filter '*.dll' -File | Copy-Item -Destination $testRoot
Copy-Item -LiteralPath $InputPartList -Destination (Join-Path $testRoot 'InputPartList.xml')
Copy-Item -LiteralPath (Join-Path $root 'FAFramework/App.xaml') -Destination (Join-Path $testRoot 'AppStyles.xml')
Copy-Item -LiteralPath (Join-Path $runtime 'config/language/StringResource_ko-kr.xaml') -Destination (Join-Path $testRoot 'KoreanResources.xaml')
Copy-Item -LiteralPath (Join-Path $root 'FAFramework/VT3500/GUI/ConfigBaseControl.xaml') -Destination (Join-Path $testRoot 'ConfigBaseSource.xml')
$compiler = 'C:/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/Roslyn/csc.exe'
$framework = 'C:/Program Files (x86)/Reference Assemblies/Microsoft/Framework/.NETFramework/v4.6.1'
$testExe = Join-Path $testRoot 'MotorConfigEditingTests.exe'
$arguments = @('/nologo', '/target:exe', '/platform:x86', '/langversion:latest', "/out:$testExe",
    ('/reference:' + (Join-Path $testRoot 'FALibrary.dll')),
    ('/reference:' + (Join-Path $testRoot 'FAFramework.exe')),
    ('/reference:' + (Join-Path $framework 'WindowsBase.dll')),
    ('/reference:' + (Join-Path $framework 'PresentationFramework.dll')),
    ('/reference:' + (Join-Path $framework 'PresentationCore.dll')),
    ('/reference:' + (Join-Path $framework 'System.Xaml.dll')),
    (Join-Path $PSScriptRoot 'MotorConfigEditingTests.cs'))
& $compiler @arguments
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
Write-Output "Isolated test directory: $testRoot"
if ($ReportPath) { $ReportPath = [IO.Path]::GetFullPath($ReportPath) }
Push-Location $testRoot
try {
    if ($ReportPath) { & $testExe | Tee-Object -FilePath $ReportPath } else { & $testExe }
    if ($LASTEXITCODE -ne 0) { throw 'Motor config tests failed.' }
} finally { Pop-Location }
