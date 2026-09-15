param([string]$ReportPath, [string]$RuntimeRoot)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$testRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('IMarkDiagnostics-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot | Out-Null
if (-not $RuntimeRoot) { $RuntimeRoot = Join-Path $repoRoot 'FAFramework\bin\Debug' }
Copy-Item -LiteralPath (Join-Path $runtimeRoot 'FAFramework.exe') -Destination $testRoot
Copy-Item -LiteralPath (Join-Path $repoRoot 'FAFramework\App.xaml') -Destination (Join-Path $testRoot 'AppStyles.xml')
Copy-Item -LiteralPath (Join-Path $repoRoot 'FAFramework\VT3500\GUI\MainStatusControl.xaml') -Destination (Join-Path $testRoot 'MainStatusSource.xml')
Get-ChildItem -LiteralPath $runtimeRoot -Filter '*.dll' -File | Copy-Item -Destination $testRoot
$compiler = 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
$framework = 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1'
$testExe = Join-Path $testRoot 'IMarkDiagnosticsTests.exe'
$arguments = @('/nologo', '/target:exe', '/platform:x86', '/langversion:latest', "/out:$testExe",
    ('/reference:' + (Join-Path $testRoot 'FALibrary.dll')),
    ('/reference:' + (Join-Path $testRoot 'FAFramework.exe')),
    ('/reference:' + (Join-Path $framework 'WindowsBase.dll')),
    ('/reference:' + (Join-Path $framework 'PresentationFramework.dll')),
    ('/reference:' + (Join-Path $framework 'PresentationCore.dll')),
    ('/reference:' + (Join-Path $framework 'System.Xaml.dll')),
    (Join-Path $repoRoot 'FAFramework\VT3500\Modules\IMarkSequenceDiagnostics.cs'),
    (Join-Path $PSScriptRoot 'IMarkPositioningTests.cs'),
    (Join-Path $PSScriptRoot 'IMarkFrontIntegrationTests.cs'),
    (Join-Path $repoRoot 'FAFramework\VT3500\Modules\IMarkUsageSelection.cs'),
    (Join-Path $PSScriptRoot 'IMarkUsageSelectionTests.cs'),
    (Join-Path $PSScriptRoot 'IMarkDiagnosticsTests.cs'))
& $compiler @arguments
if ($LASTEXITCODE -ne 0) { throw 'Diagnostic test compilation failed.' }
Write-Output "Isolated test directory: $testRoot"
if ($ReportPath) {
    & $testExe | Tee-Object -FilePath $ReportPath
} else {
    & $testExe
}
if ($LASTEXITCODE -ne 0) { throw 'Diagnostic verification failed.' }
