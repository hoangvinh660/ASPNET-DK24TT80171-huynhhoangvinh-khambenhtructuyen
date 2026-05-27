#Requires -Version 5.1
<#
.SYNOPSIS
    Tao migration EF Core moi va cap nhat database.
.DESCRIPTION
    1) dotnet ef migrations add <TenMigration>
    2) dotnet ef database update
.PARAMETER TenMigration
    Ten migration (vi du: ThemBangXyz). Neu bo trong se hoi khi chay.
.EXAMPLE
    .\cap-nhat-database.ps1 -TenMigration "ThemCotMoi"
.EXAMPLE
    .\cap-nhat-database.ps1
#>
param(
    [Parameter(Mandatory = $false)]
    [string]$TenMigration
)

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$Project = Join-Path $Root "DatLichKhamBenh\DatLichKhamBenh.csproj"

function Write-Step([string]$Msg) {
    Write-Host ""
    Write-Host "==> $Msg" -ForegroundColor Cyan
}

function Ensure-DotNetEf {
    $ef = dotnet ef --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Chua co dotnet-ef. Dang cai global tool..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Cai dotnet-ef that bai. Thu chay:" -ForegroundColor Red
            Write-Host "  dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
            exit 1
        }
    }
}

if (-not (Test-Path $Project)) {
    Write-Host "Khong tim thay project: $Project" -ForegroundColor Red
    exit 1
}

if ([string]::IsNullOrWhiteSpace($TenMigration)) {
    Write-Host ""
    Write-Host "Cap nhat database (EF Core migration)" -ForegroundColor Green
    $TenMigration = Read-Host "Nhap ten migration (vi du: ThemBangMoi, SuaCotX)"
    if ([string]::IsNullOrWhiteSpace($TenMigration)) {
        Write-Host "Da huy: chua nhap ten migration." -ForegroundColor Yellow
        exit 0
    }
}

# Loai bo ky tu khong hop le trong ten migration
$TenMigration = $TenMigration.Trim() -replace '\s+', ''

Write-Host ""
Write-Host "Ten migration: $TenMigration" -ForegroundColor Green
Write-Host "Project: $Project"

Ensure-DotNetEf

Push-Location $Root
try {
    Write-Step "Tao migration '$TenMigration'..."
    dotnet ef migrations add $TenMigration --project $Project --startup-project $Project
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Step "Ap dung migration len database (database update)..."
    dotnet ef database update --project $Project --startup-project $Project
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Host ""
    Write-Host "Hoan tat! Database da duoc cap nhat." -ForegroundColor Green
    Write-Host "Chay .\chay-web.ps1 de khoi dong lai ung dung." -ForegroundColor DarkGray
}
finally {
    Pop-Location
}
