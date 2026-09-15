param([Parameter(Mandatory=$true)][string]$OutputPath)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$bitmap = [System.Drawing.Bitmap]::new(1600, 1560)
$g = [System.Drawing.Graphics]::FromImage($bitmap)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.Clear([System.Drawing.Color]::White)
$title = [System.Drawing.Font]::new('Malgun Gothic', 29, [System.Drawing.FontStyle]::Bold)
$heading = [System.Drawing.Font]::new('Malgun Gothic', 20, [System.Drawing.FontStyle]::Bold)
$body = [System.Drawing.Font]::new('Malgun Gothic', 16)
$small = [System.Drawing.Font]::new('Malgun Gothic', 14)
$ink = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml('#172C30'))
$muted = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml('#526269'))
$pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml('#83969B'), 2)
$arrow = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml('#217565'), 3)
$arrow.CustomEndCap = [System.Drawing.Drawing2D.AdjustableArrowCap]::new(5, 6)
function Text($text, $font, $x, $y, $w, $h) {
    $g.DrawString($text, $font, $ink, [System.Drawing.RectangleF]::new($x,$y,$w,$h))
}
function Box($x,$y,$w,$h,$label,$detail,$color='#EDF5F3') {
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($color))
    $g.FillRectangle($brush,$x,$y,$w,$h)
    $g.DrawRectangle($pen,$x,$y,$w,$h)
    Text $label $heading ($x+20) ($y+14) ($w-40) 44
    Text $detail $body ($x+20) ($y+60) ($w-40) ($h-62)
    $brush.Dispose()
}
Text '5축 FRONT I-Mark / 그리퍼축 + 실린더 동작' $title 60 32 1480 64
Text 'X1135 → TapeLoadingServo (AxisNo=1) / REAR BandRollerServo (AxisNo=4)는 기존 기능 복원' $body 65 102 1470 42
Box 60 175 950 110 '1. 원단 파지 준비' 'TapeLoadGrip 잡기 → TapeHoldGrip 풀기. 기존 실린더 IO 유지.'
Box 60 320 950 110 '2. FRONT I-Mark 사용 및 상태 확인' '입력, 그리퍼 상태, 위치, 시간, 초기화 필요 여부를 확인.'
Box 60 465 950 135 '3. 고속 접근 완료 대기' "고속 목표 = max(0, 원단 피치 - 저속 탐색 거리)`nTapeLoadingServo.MoveTapeLoadingPos 사용."
Box 60 635 950 160 '4. X1135 저속 탐색 / 정지' "저속 목표 = 원단 피치. OFF 상태에서 탐색 지령 1회.`n새 ON 감지 위치 기록 → 정지 요청 → 정지 완료 대기."
Box 60 830 950 135 '5. 설치 보정 이동 완료 대기' "목표 = 감지 위치 + FrontIMarkSensorOffset`n0이면 추가 이동 생략. 한계 초과 / 정지 오버런은 알람."
Box 60 1000 950 135 '6. 실린더 인계' "TapeHoldGrip 잡기 완료 → TapeLoadGrip 풀기 완료`n보정 이동이 끝나기 전에는 이 단계로 넘어가지 않음."
Box 60 1170 950 140 '7. 후속 복귀 / 다음 이송 준비' "그리퍼축 HomePos 이동 → TapeLoadGrip 잡기 → TapeHoldGrip 풀기`n원점 좌표를 임의로 0 설정하지 않음. 5축 기존 후속 공정 유지."
foreach($ends in @(@(285,320),@(430,465),@(600,635),@(795,830),@(965,1000),@(1135,1170))) {
    $g.DrawLine($arrow,535,$ends[0]+4,535,$ends[1]-5)
}
Box 1060 175 480 150 'FRONT 미사용' "기존 원단 피치만큼 이동.`n감지·보정 없이 실린더 인계." '#F0F2F5'
Box 1060 385 480 190 '이미 처리한 ON 유지' "고속 접근 이후 같은 ON이면`n중복 탐색·보정을 생략.`nOFF 확인 후 다음 ON을 수용." '#FFF5D9'
Box 1060 635 480 295 '오류 / 중단' "미감지 시간 초과, 입력 이상,`n그리퍼 상태 이상, 범위 초과:`n정지 + FRONT 알람.`n자동 재탐색 없음.`n원인 확인 후 초기화 필요." '#FCEDEE'
Box 1060 1000 480 310 'REAR 기존 기능 복원' "BandRollerServo AxisNo=4`nBlackMarkCheckSensor X1140`n기존 UseIMark 설정 유지.`n기존 피딩·재시도·컷팅·수량 처리.`nX1135 및 신규 Offset는 사용 안 함.`n메인: REAR 마크찾기로 구분." '#EDF1F6'
Text '로그: Flow=FrontGripperX1135V1 / CycleId + RunId / X1135 입력 / 모터 위치 / 그리퍼 입출력 / 정지·재개·오류' $small 65 1380 1470 60
Text '소스 및 장비 없는 시험 기준. 실제 센서 극성·정지거리·그리퍼 파지·Offset 방향은 저속 시운전 확인 필요. 2026-09-09' $small 65 1470 1470 60
$bitmap.Save([System.IO.Path]::GetFullPath($OutputPath), [System.Drawing.Imaging.ImageFormat]::Png)
$arrow.Dispose()
$pen.Dispose()
$ink.Dispose()
$muted.Dispose()
$title.Dispose()
$heading.Dispose()
$body.Dispose()
$small.Dispose()
$g.Dispose()
$bitmap.Dispose()
Write-Output ([System.IO.Path]::GetFullPath($OutputPath))
