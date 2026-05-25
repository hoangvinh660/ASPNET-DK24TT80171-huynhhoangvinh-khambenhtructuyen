# Chay website bang IIS Express (port 50000)
$project = Join-Path $PSScriptRoot "QuanLyDatLichKhamBenh"
$iis = "${env:ProgramFiles}\IIS Express\iisexpress.exe"

if (-not (Test-Path $iis)) {
    Write-Host "Chua co IIS Express. Cai bang: choco install iisexpress -y"
    exit 1
}

# Build truoc khi chay
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
& $msbuild (Join-Path $PSScriptRoot "QuanLyDatLichKhamBenh.sln") /t:Build /p:Configuration=Debug /v:minimal

Write-Host ""
Write-Host "Mo trinh duyet: http://localhost:50000"
Write-Host "Nhan Ctrl+C de dung server."
& $iis /path:$project /port:50000 /clr:v4.0
