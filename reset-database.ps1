#Requires -Version 5.1
<#
.SYNOPSIS
    Xoa toan bo database va tao lai tu dau (mat het du lieu).
.DESCRIPTION
    1) dotnet ef database drop --force
    2) dotnet ef database update
    Lan chay dotnet run tiep theo: SeedData se nap lai du lieu mau.
.PARAMETER Force
    Bo qua xac nhan (dung trong CI/script tu dong).
.EXAMPLE
    .\reset-database.ps1
.EXAMPLE
    .\reset-database.ps1 -Force
#>
param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$Project = Join-Path $Root "DatLichKhamBenh\DatLichKhamBenh.csproj"

function Write-Step([string]$Msg) {
    Write-Host ""
    Write-Host "==> $Msg" -ForegroundColor Cyan
}

function Ensure-DotNetEf {
    $null = dotnet ef --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Chua co dotnet-ef. Dang cai global tool..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -ne 0) { exit 1 }
    }
}

if (-not (Test-Path $Project)) {
    Write-Host "Khong tim thay project: $Project" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "RESET DATABASE - DatLichKhamBenh" -ForegroundColor Red
Write-Host "Canh bao: Toan bo du lieu trong DB se bi XOA!" -ForegroundColor Yellow
Write-Host ""
Write-Host "Kiem tra connection string trong:" -ForegroundColor DarkGray
Write-Host "  DatLichKhamBenh\appsettings.json" -ForegroundColor DarkGray
Write-Host ""

if (-not $Force) {
    $xacNhan = Read-Host "Ban co chac chan? Go 'yes' de tiep tuc"
    if ($xacNhan -ne "yes") {
        Write-Host "Da huy." -ForegroundColor Yellow
        exit 0
    }
}

# Tat app neu dang chay (tranh lock DB)
$proc = Get-Process -Name "DatLichKhamBenh" -ErrorAction SilentlyContinue
if ($proc) {
    Write-Step "Dang tat ung dung dang chay..."
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 1
}

Ensure-DotNetEf

Push-Location $Root
try {
    Write-Step "Xoa database (drop --force)..."
    dotnet ef database drop --force --project $Project --startup-project $Project
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Drop database that bai. Kiem tra SQL Server va connection string." -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Step "Tao lai schema (database update)..."
    dotnet ef database update --project $Project --startup-project $Project
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Host ""
    Write-Host "Database da duoc tao lai tu dau." -ForegroundColor Green
    Write-Host ""
    Write-Host "Buoc tiep theo:" -ForegroundColor Cyan
    Write-Host "  1. Chay: .\chay-web.ps1" -ForegroundColor White
    Write-Host "  2. SeedData se tu nap du lieu mau khi app khoi dong (neu bang NguoiDung trong)" -ForegroundColor White
    Write-Host ""
    Write-Host "  Tai khoan demo: admin/Admin@123, bs.an/Bacsi@123, bn.hoa/Benhnhan@123" -ForegroundColor DarkGray
}
finally {
    Pop-Location
}
