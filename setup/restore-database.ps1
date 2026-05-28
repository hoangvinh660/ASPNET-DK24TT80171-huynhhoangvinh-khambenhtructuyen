#Requires -Version 5.1
<#
.SYNOPSIS
    Khoi phuc database tu file .bak (Thay / may khac dung de xem du lieu).
.PARAMETER DuongDanBak
    Duong dan file .bak. Neu bo trong, chon file moi nhat trong thu muc backups\
.EXAMPLE
    .\restore-database.ps1 -DuongDanBak ".\backups\DatLichKhamBenh_20260526_120000.bak"
#>
param(
    [string]$DuongDanBak = ""
)

$ErrorActionPreference = "Stop"
$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

$AppSettingsCandidates = @(
    (Join-Path (Join-Path (Join-Path $Root "src") "DatLichKhamBenh") "appsettings.json"),
    (Join-Path (Join-Path $Root "DatLichKhamBenh") "appsettings.json")
)
$AppSettings = $AppSettingsCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

function Write-Step([string]$Msg) {
    Write-Host ""
    Write-Host "==> $Msg" -ForegroundColor Cyan
}

function Get-ConnectionFromAppSettings {
    $json = Get-Content $AppSettings -Raw | ConvertFrom-Json
    return $json.ConnectionStrings.DatLichKhamBenh
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

if ([string]::IsNullOrWhiteSpace($DuongDanBak)) {
    $backupDir = Join-Path $Root "backups"
    $latest = Get-ChildItem $backupDir -Filter "*.bak" -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if (-not $latest) {
        Write-Host "Khong co file .bak trong $backupDir" -ForegroundColor Red
        Write-Host "Chay: .\restore-database.ps1 -DuongDanBak `"duong\dan\file.bak`"" -ForegroundColor Yellow
        exit 1
    }
    $DuongDanBak = $latest.FullName
}

$DuongDanBak = (Resolve-Path $DuongDanBak).Path
$conn = Parse-SqlConnection (Get-ConnectionFromAppSettings)

$sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmd) {
    Write-Host "Can sqlcmd hoac dung SSMS: Restore Database..." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "RESTORE DATABASE" -ForegroundColor Yellow
Write-Host "  File .bak : $DuongDanBak"
Write-Host "  Database  : $($conn.Database) tren $($conn.Server)"
Write-Host ""
Write-Host "Canh bao: Du lieu hien tai trong DB se bi ghi de!" -ForegroundColor Red

$ok = Read-Host "Go 'yes' de tiep tuc"
if ($ok -ne "yes") { Write-Host "Da huy."; exit 0 }

$bakSql = $DuongDanBak.Replace("'", "''")
$db = $conn.Database

# Dat SINGLE_USER de giai phong ket noi
$queries = @"
ALTER DATABASE [$db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [$db]
FROM DISK = N'$bakSql'
WITH REPLACE, STATS = 10;
ALTER DATABASE [$db] SET MULTI_USER;
"@

Write-Step "Dang restore..."
if ($conn.Integrated) {
    & sqlcmd -S $conn.Server -E -Q $queries
} else {
    & sqlcmd -S $conn.Server -U $conn.User -P $conn.Password -C -Q $queries
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Restore that bai. Kiem tra duong dan file .bak va quyen SQL." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Restore thanh cong! Chay .\chay-web.ps1 de mo web." -ForegroundColor Green
