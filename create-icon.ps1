$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$taskBitmap = New-Object System.Drawing.Bitmap 128,128
$taskGraphics = [System.Drawing.Graphics]::FromImage($taskBitmap)
$taskGraphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$taskGraphics.Clear([System.Drawing.Color]::Transparent)
$taskShape = New-Object System.Drawing.Drawing2D.GraphicsPath
$taskShape.AddArc(4,4,36,36,180,90)
$taskShape.AddArc(88,4,36,36,270,90)
$taskShape.AddArc(88,88,36,36,0,90)
$taskShape.AddArc(4,88,36,36,90,90)
$taskShape.CloseFigure()
$taskRect = New-Object System.Drawing.Rectangle 0,0,128,128
$taskFill = New-Object System.Drawing.Drawing2D.LinearGradientBrush $taskRect,([System.Drawing.Color]::FromArgb(131,189,255)),([System.Drawing.Color]::FromArgb(140,120,231)),45
$taskGraphics.FillPath($taskFill,$taskShape)
$taskPoints = [System.Drawing.PointF[]]@([System.Drawing.PointF]::new(64,24),[System.Drawing.PointF]::new(76,51),[System.Drawing.PointF]::new(104,64),[System.Drawing.PointF]::new(76,77),[System.Drawing.PointF]::new(64,104),[System.Drawing.PointF]::new(51,77),[System.Drawing.PointF]::new(24,64),[System.Drawing.PointF]::new(51,51))
$taskInk = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(15,25,45))
$taskGraphics.FillPolygon($taskInk,$taskPoints)
$taskIcon = [System.Drawing.Icon]::FromHandle($taskBitmap.GetHicon())
$taskOutput = [System.IO.File]::Create((Join-Path $PSScriptRoot 'src\Aster.ico'))
$taskIcon.Save($taskOutput)
$taskOutput.Dispose()
$taskIcon.Dispose()
$taskGraphics.Dispose()
$taskBitmap.Dispose()
$taskShape.Dispose()
$taskFill.Dispose()
$taskInk.Dispose()

