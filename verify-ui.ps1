$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force -Path (Join-Path $PSScriptRoot 'artifacts') | Out-Null
$taskFramework = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$taskRefs = @('System.dll','System.Core.dll','System.Xaml.dll') | ForEach-Object { '/reference:' + (Join-Path $taskFramework $_) }
$taskRefs += @('PresentationCore.dll','PresentationFramework.dll','WindowsBase.dll') | ForEach-Object { '/reference:' + (Join-Path $taskFramework ('WPF\' + $_)) }
$taskRefs += '/reference:' + (Join-Path $PSScriptRoot 'dist\Aster.exe')
& (Join-Path $taskFramework 'csc.exe') /nologo /target:exe ('/out:' + (Join-Path $PSScriptRoot 'dist\UITests.exe')) @taskRefs (Join-Path $PSScriptRoot 'tests\UITests.cs')
if ($LASTEXITCODE -ne 0) { throw 'UI test compilation failed.' }
& (Join-Path $PSScriptRoot 'dist\UITests.exe') (Join-Path $PSScriptRoot 'artifacts')
if ($LASTEXITCODE -ne 0) { throw 'UI verification failed.' }

