#Requires -Version 5.1
<#
.SYNOPSIS
    Build va chay nhanh project DatLichKhamBenh.
.DESCRIPTION
    Restore (lan dau), build, roi dotnet run.
    Mac dinh: http://localhost:5231
.EXAMPLE
    .\chay-web.ps1
.EXAMPLE
    .\chay-web.ps1 -NoBuild
#>
param(
    [switch]$NoBuild,
    [string]$Url = "http://localhost:5231"
)

$ErrorActionPreference = "Stop"
$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

$ProjectCandidates = @(
    (Join-Path (Join-Path (Join-Path $Root "src") "DatLichKhamBenh") "DatLichKhamBenh.csproj"),
    (Join-Path (Join-Path $Root "DatLichKhamBenh") "DatLichKhamBenh.csproj")
)
$Project = $ProjectCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

function Write-Step([string]$Msg) {
    Write-Host ""
    Write-Host "==> $Msg" -ForegroundColor Cyan
}

if (-not (Test-Path $Project)) {
    Write-Host "Khong tim thay project: $Project" -ForegroundColor Red
    exit 1
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Chua cai .NET SDK. Tai tai: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Dat lich kham benh - chay web" -ForegroundColor Green
Write-Host "Thu muc: $Root"

# Giai phong file DLL neu app dang chay (tranh loi build)
$proc = Get-Process -Name "DatLichKhamBenh" -ErrorAction SilentlyContinue
if ($proc) {
    Write-Step "Dang tat tien trinh DatLichKhamBenh cu..."
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 1
}

Push-Location $Root
try {
    if (-not $NoBuild) {
        Write-Step "dotnet restore..."
        dotnet restore $Project

        Write-Step "dotnet build..."
        dotnet build $Project --configuration Debug
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }

    Write-Step "Khoi dong ung dung..."
    Write-Host ""
    Write-Host "  Mo trinh duyet: $Url" -ForegroundColor Yellow
    Write-Host "  Nhan Ctrl+C de dung server." -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "  Tai khoan demo (xem README.md):" -ForegroundColor DarkGray
    Write-Host "    admin / Admin@123" -ForegroundColor DarkGray
    Write-Host "    bs.an / Bacsi@123  |  bn.hoa / Benhnhan@123" -ForegroundColor DarkGray
    Write-Host ""

    dotnet run --project $Project --launch-profile http
}
finally {
    Pop-Location
}
