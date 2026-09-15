param([Parameter(Mandatory=$true)][string]$OutputPath)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$bitmap = [System.Drawing.Bitmap]::new(1600, 1440)
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
Text '5축 Rear I-Mark 시퀀스 / 4축 동작 흐름 적용' $title 60 32 1480 64
Text '입력 X1135 · BandRollerServo AxisNo=4 유지 · JOB별 설치 보정(mm)' $body 65 102 1470 42
Box 60 175 950 110 '1. I-Mark 사용 확인 / 설정 검증' '입력 상태, 탐색 시간, 위치 및 보정값 확인. 오류 후에는 초기화 필요.'
Box 60 320 950 110 '2. 빠른 피딩 이동 완료 대기' '5축의 기존 TapeLoadingPos와 모터 설정 사용.'
Box 60 465 950 160 '3. 저속 탐색 / 새 ON 판정' "OFF: 저속 이동 1회 지령 후 감지 대기. OFF 확인 시 재감지 허용.`n새 ON: 즉시 정지 요청. Unknown / 미연결은 감지로 처리하지 않음."
Box 60 660 950 110 '4. 감지 위치 기록 / 정지 완료 확인' '정지 요청 직후 ActualPos를 기록하고 Stop.Sequence 완료를 기다림.'
Box 60 805 950 135 '5. 보정 목표 계산 및 이동 완료 대기' "목표 = 감지 위치 + IMarkSensorOffset`n양수: 기존 저속 속도·가감속 적용. 0: 추가 이동 생략."
Box 60 975 950 110 '6. 5축 피딩 완료 처리' '횟수 반영 → 롤러 좌표 0 설정 → 기존 롤러 지연.'
Box 60 1120 950 110 '7. 기존 후속 공정 진행' '자동운전의 컷팅 조건을 그대로 유지. 보정 완료 전에 컷팅하지 않음.'
foreach($ends in @(@(285,320),@(430,465),@(625,660),@(770,805),@(940,975),@(1085,1120))) {
    $g.DrawLine($arrow,535,$ends[0]+4,535,$ends[1]-5)
}
Box 1060 175 480 150 '미사용' "기존 5축 빠른 + 느린 위치 이동.`nI-Mark 감지·보정은 생략." '#F0F2F5'
Box 1060 385 480 190 '이미 처리한 ON 유지' "빠른 이동 후에도 같은 ON이면`n저속 탐색과 보정을 생략하고`n피딩 완료 단계로 진행." '#FFF5D9'
Box 1060 660 480 270 '오류 분기: 정지 + 알람' "탐색 시간 초과 / 입력 이상 /`n잘못된 목표 / 위치 한계 초과 /`n감속 후 목표를 허용오차보다 초과`n`n자동 재탐색 없음.`n원인 조치 후 초기화 필요." '#FCEDEE'
Box 1060 975 480 255 '적용 경로' "자동·1사이클·수동 피딩: 전체 흐름`n수동 연속: 피딩 + 보정 후 컷팅`nI-Mark 찾기: 빠른 이동 없이 보정`n초기화: 마크 기준 설정, 보정 없음`n수동 단계 동작: 기존 동작 유지" '#EDF1F6'
Text '정상 일시정지·재개: 진행 중인 단계와 보정 목표 유지. 강제 중단·I-Mark 오류: 초기화 후 재시작.' $small 65 1290 1470 45
Text '소스 기준 동작 설명도 · 실제 센서 타이밍/정지거리/컷팅 위치는 실기 검증 필요 · 2026-09-09' $small 65 1350 1470 45
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
