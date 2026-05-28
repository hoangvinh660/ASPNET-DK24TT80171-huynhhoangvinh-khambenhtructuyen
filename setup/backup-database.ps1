#Requires -Version 5.1
<#
.SYNOPSIS
    Backup database DatLichKhamBenh ra file .bak (gui cho Thay / nop do an).
.DESCRIPTION
    Doc connection string tu appsettings.json, chay BACKUP DATABASE.
    File luu tai: backups\DatLichKhamBenh_yyyyMMdd_HHmmss.bak
.EXAMPLE
    .\backup-database.ps1
.EXAMPLE
    .\backup-database.ps1 -ThuMucOut "D:\NopDoAn"
#>
param(
    [string]$ThuMucOut = "",
    [string]$ConnectionString = ""
)

$ErrorActionPreference = "Stop"
$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

$AppSettingsCandidates = @(
    (Join-Path (Join-Path (Join-Path $Root "src") "DatLichKhamBenh") "appsettings.json"),
    (Join-Path (Join-Path $Root "DatLichKhamBenh") "appsettings.json")
)
$AppSettings = $AppSettingsCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
$DefaultOut = Join-Path $Root "backups"

function Write-Step([string]$Msg) {
    Write-Host ""
    Write-Host "==> $Msg" -ForegroundColor Cyan
}

function Get-ConnectionFromAppSettings {
    if (-not (Test-Path $AppSettings)) {
        throw "Khong tim thay $AppSettings"
    }
    $json = Get-Content $AppSettings -Raw | ConvertFrom-Json
    $cs = $json.ConnectionStrings.DatLichKhamBenh
    if ([string]::IsNullOrWhiteSpace($cs)) {
        throw "Khong tim thay ConnectionStrings:DatLichKhamBenh trong appsettings.json"
    }
    return $cs
}

function Parse-SqlConnection([string]$cs) {
    Add-Type -AssemblyName "System.Data" | Out-Null
    $b = New-Object System.Data.SqlClient.SqlConnectionStringBuilder $cs
    return @{
        Server   = $b.DataSource
        Database = $b.InitialCatalog
        User     = $b.UserID
        Password = $b.Password
        Integrated = $b.IntegratedSecurity
    }
}

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    $ConnectionString = Get-ConnectionFromAppSettings
}

$conn = Parse-SqlConnection $ConnectionString
$outDir = if ([string]::IsNullOrWhiteSpace($ThuMucOut)) { $DefaultOut } else { $ThuMucOut }
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$bakName = "DatLichKhamBenh_$stamp.bak"
$bakPath = Join-Path $outDir $bakName
# SQL Server can dung duong dan Windows; escape single quotes
$bakPathSql = $bakPath.Replace("'", "''")

$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd) {
    Write-Host "Khong tim thay sqlcmd." -ForegroundColor Red
    Write-Host "Cai SQL Server Command Line Tools hoac mo SSMS:" -ForegroundColor Yellow
    Write-Host "  Right-click database DatLichKhamBenh -> Tasks -> Back Up..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Hoac tai: https://learn.microsoft.com/sql/tools/sqlcmd/sqlcmd-utility" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "BACKUP DATABASE - DatLichKhamBenh" -ForegroundColor Green
Write-Host "  Server  : $($conn.Server)"
Write-Host "  Database: $($conn.Database)"
Write-Host "  File    : $bakPath"

# Tat app neu dang mo DB
$proc = Get-Process -Name "DatLichKhamBenh" -ErrorAction SilentlyContinue
if ($proc) {
    Write-Step "Dang tat ung dung (tranh lock database)..."
    $proc | Stop-Process -Force
    Start-Sleep -Seconds 1
}

$query = @"
BACKUP DATABASE [$($conn.Database)]
TO DISK = N'$bakPathSql'
WITH FORMAT, INIT, COMPRESSION,
NAME = N'$($conn.Database)-Full-$stamp',
STATS = 10;
"@

Write-Step "Dang backup..."
if ($conn.Integrated) {
    & sqlcmd -S $conn.Server -E -Q $query
} else {
    & sqlcmd -S $conn.Server -U $conn.User -P $conn.Password -C -Q $query
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Backup that bai. Kiem tra SQL Server dang chay va quyen sa." -ForegroundColor Red
    exit $LASTEXITCODE
}

if (-not (Test-Path $bakPath)) {
    Write-Host "Khong thay file .bak sau khi chay. Thu tat COMPRESSION neu SQL Express cu." -ForegroundColor Yellow
    $queryNoComp = $query -replace " COMPRESSION,", ""
    if ($conn.Integrated) {
        & sqlcmd -S $conn.Server -E -Q $queryNoComp
    } else {
        & sqlcmd -S $conn.Server -U $conn.User -P $conn.Password -C -Q $queryNoComp
    }
}

$sizeMb = [math]::Round((Get-Item $bakPath).Length / 1MB, 2)
Write-Host ""
Write-Host "Backup thanh cong!" -ForegroundColor Green
Write-Host "  File : $bakPath"
Write-Host "  Size : $sizeMb MB"
Write-Host ""
Write-Host "Gui cho Thay:" -ForegroundColor Cyan
Write-Host "  1. Nen file .bak bang ZIP (click phai -> Send to -> Compressed folder)"
Write-Host "  2. Kem README hoac huong dan restore (restore-database.ps1)"
Write-Host ""

# Goi y tao zip
$zipPath = "$bakPath.zip"
try {
  if (Get-Command Compress-Archive -ErrorAction SilentlyContinue) {
    Compress-Archive -Path $bakPath -DestinationPath $zipPath -Force
    Write-Host "Da tao them: $zipPath" -ForegroundColor Green
  }
} catch {
  Write-Host "Tu nen ZIP thu cong neu can." -ForegroundColor DarkGray
}
