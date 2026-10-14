$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force -Path (Join-Path $PSScriptRoot 'artifacts') | Out-Null
$taskFramework = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$taskRefs = @('System.dll','System.Core.dll','System.Web.Extensions.dll','System.IO.Compression.dll','System.IO.Compression.FileSystem.dll') | ForEach-Object { '/reference:' + (Join-Path $taskFramework $_) }
& (Join-Path $taskFramework 'csc.exe') /nologo /target:exe ('/out:' + (Join-Path $PSScriptRoot 'artifacts\CoreTests.exe')) @taskRefs (Join-Path $PSScriptRoot 'src\Core.cs') (Join-Path $PSScriptRoot 'src\Catalog.cs') (Join-Path $PSScriptRoot 'tests\CoreTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& (Join-Path $PSScriptRoot 'artifacts\CoreTests.exe') (Join-Path $PSScriptRoot 'artifacts')
if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }

