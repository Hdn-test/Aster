$ErrorActionPreference = 'Stop'
$taskRoot = $PSScriptRoot
$taskFramework = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$taskCompiler = Join-Path $taskFramework 'csc.exe'
New-Item -ItemType Directory -Force -Path (Join-Path $taskRoot 'dist') | Out-Null
if (-not (Test-Path -LiteralPath (Join-Path $taskRoot 'src\Aster.ico'))) { & (Join-Path $taskRoot 'create-icon.ps1') }
$taskReferences = @('System.dll','System.Core.dll','System.Xaml.dll','System.Web.Extensions.dll','System.IO.Compression.dll','System.IO.Compression.FileSystem.dll','System.Windows.Forms.dll','System.Drawing.dll','Microsoft.VisualBasic.dll') | ForEach-Object { '/reference:' + (Join-Path $taskFramework $_) }
$taskReferences += @('WindowsBase.dll','PresentationCore.dll','PresentationFramework.dll') | ForEach-Object { '/reference:' + (Join-Path $taskFramework ('WPF\' + $_)) }
& $taskCompiler /nologo /target:winexe /optimize+ /platform:x64 ('/win32icon:' + (Join-Path $taskRoot 'src\Aster.ico')) ('/win32manifest:' + (Join-Path $taskRoot 'src\app.manifest')) ('/resource:' + (Join-Path $taskRoot 'src\Aster.ico') + ',Aster.ico') ('/resource:' + (Join-Path $taskRoot 'src\Main.xaml') + ',Main.xaml') ('/resource:' + (Join-Path $taskRoot 'src\Library.xaml') + ',Library.xaml') ('/out:' + (Join-Path $taskRoot 'dist\Aster.exe')) @taskReferences (Join-Path $taskRoot 'src\Core.cs') (Join-Path $taskRoot 'src\Catalog.cs') (Join-Path $taskRoot 'src\Main.cs') (Join-Path $taskRoot 'src\Enhancements.cs') (Join-Path $taskRoot 'src\Vault.cs') (Join-Path $taskRoot 'src\LibraryState.cs') (Join-Path $taskRoot 'src\Revision.cs')
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Copy-Item -LiteralPath (Join-Path $taskRoot 'dist\Aster.exe') -Destination (Join-Path $taskRoot 'Aster-0.3.exe') -Force
try { Copy-Item -LiteralPath (Join-Path $taskRoot 'dist\Aster.exe') -Destination (Join-Path $taskRoot 'Aster.exe') -Force -ErrorAction Stop } catch { Write-Output 'Aster.exe is in use; new version is Aster-0.3.exe.' }
Write-Output 'Built standalone Aster-0.3.exe'



