param([string]$ReportPath)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
function Check([bool]$condition, [string]$message) {
    if (-not $condition) { throw "FAIL: $message" }
    "PASS: $message"
}
function ReadXml([string]$relative) {
    [xml](Get-Content -LiteralPath (Join-Path $root $relative) -Raw -Encoding UTF8)
}
function PropertyValue($part, [string]$name) {
    ($part.Parameters.Property.Item | Where-Object Name -eq $name).Value
}
$results = & {
    $front = ReadXml 'FAFramework/bin/Debug/config/VT3500/FrontLoadingUnit/PartList.xml'
    $rear = ReadXml 'FAFramework/bin/Debug/config/VT3500/RearLoadingUnit/PartList.xml'
    $frontServo = $front.PartList.Part | Where-Object Name -eq 'TapeLoadingServo'
    $rearServo = $rear.PartList.Part | Where-Object Name -eq 'BandRollerServo'
    Check ((PropertyValue $frontServo 'AxisNo') -eq '1') 'FRONT TapeLoadingServo AxisNo remains 1'
    Check ((PropertyValue $rearServo 'AxisNo') -eq '4') 'REAR BandRollerServo AxisNo remains 4'
    $sensor = @($front.PartList.Part | Where-Object Name -eq 'FrontIMarkCheckSensor')
    Check ($sensor.Count -eq 1 -and $sensor[0].Parameters.InputIOList.IOIndex -eq 'X1135') 'FRONT sensor maps X1135 exactly once'
    Check (@($rear.PartList.Part | Where-Object Name -eq 'IMarkCheckSensor').Count -eq 0) 'Added X1135 sensor removed from REAR'
    $black = $rear.PartList.Part | Where-Object Name -eq 'BlackMarkCheckSensor'
    Check ($black.Parameters.InputIOList.IOIndex -eq 'X1140') 'Legacy REAR BlackMark retains X1140'
    $load = $front.PartList.Part | Where-Object Name -eq 'TapeLoadGrip'
    $hold = $front.PartList.Part | Where-Object Name -eq 'TapeHoldGrip'
    Check (($load.Parameters.InputIOList.IOIndex -join ',') -eq 'X0230,X0231' -and
        ($load.Parameters.OutputIOList.IOIndex -join ',') -eq 'Y0324,Y0325') 'Loading-gripper IO unchanged'
    Check (($hold.Parameters.InputIOList.IOIndex -join ',') -eq 'X0232,X0233' -and
        ($hold.Parameters.OutputIOList.IOIndex -join ',') -eq 'Y0326,Y0327') 'Holding-gripper IO unchanged'
    $slow = $frontServo.Parameters.PositionDefineList.Item | Where-Object Name -eq 'TapeLoadingSlowPos'
    Check ($slow.DriveSpeed -eq '50' -and $slow.AccelTime -eq '100' -and $slow.DecelTime -eq '100') 'New FRONT slow speed 50; existing FRONT acceleration/deceleration retained'
    $module = ReadXml 'FAFramework/bin/Debug/config/VT3500/ModuleParameters.xml'
    $time = $module.Parameters.FrontModule.Time.Item | Where-Object Name -eq 'TimeFrontIMarkTimeout'
    Check ($time.Type -eq 'second' -and $time.TypeValue -eq '1') 'FRONT search timeout defaults to one second'
    $source = Get-Content -LiteralPath (Join-Path $root 'FAFramework/VT3500/Modules/FAFrontLoadingModule.cs') -Raw -Encoding UTF8
    Check (-not $source.Contains('seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence')) 'No production FRONT feed path bypasses the I-Mark wrapper'
    Check (([regex]::Matches($source, 'seq.AddItem\(TapeLoadingMoveWithIMark')).Count -eq 9) 'Nine required FRONT feed call sites remain after removing the duplicate first-feed call'
    $rearSource = Get-Content -LiteralPath (Join-Path $root 'FAFramework/VT3500/Modules/FARearLoadingModule.cs') -Raw -Encoding UTF8
    $baseline = (& git -C $root show 'a3ad897:FAFramework/VT3500/Modules/FARearLoadingModule.cs') -join "`n"
    Check ($rearSource.Replace("`r`n", "`n").TrimEnd() -eq $baseline.TrimEnd()) 'REAR module equals pre-X1135 commit a3ad897 except final newline'
    $partial = Get-Content -LiteralPath (Join-Path $root 'FAFramework/VT3500/Modules/FAFrontLoadingModule.IMark.cs') -Raw -Encoding UTF8
    Check (-not $partial.Contains('BandRollerServo')) 'FRONT I-Mark implementation contains no REAR motor commands'
    $ui = Get-Content -LiteralPath (Join-Path $root 'FAFramework/VT3500/GUI/PositionControl.xaml') -Raw -Encoding UTF8
    Check ($ui.Contains('JobInstance.UseFrontIMark') -and $ui.Contains('JobInstance.UseIMark') -and
        $ui.Contains('JobInstance.FrontIMarkSlowDistance') -and $ui.Contains('JobInstance.FrontIMarkSensorOffset')) 'UI exposes distinct FRONT/REAR options and FRONT distance/offset'
    $switch = Get-Content -LiteralPath (Join-Path $root 'FAFramework/VT3500/Modules/IMarkUsageSelection.cs') -Raw -Encoding UTF8
    Check (-not $switch.Contains('RearModule.UseIMark =')) 'Main FRONT selector never writes legacy REAR mode'
}
if ($ReportPath) { $results | Tee-Object -FilePath $ReportPath } else { $results }
