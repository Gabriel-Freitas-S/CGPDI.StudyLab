# Script Local para Compilacao e Geracao do Executavel Portatil e Instalador do CGPDI StudyLab

param (
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.IO.Compression.FileSystem

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  CGPDI StudyLab - Gerador de Release Local v$Version" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

$projectDir = "$PSScriptRoot\CGPDI.StudyLab"
$publishDir = "$PSScriptRoot\publish\win-x64"
$distDir = "$PSScriptRoot\dist"

# 1. Limpeza de diretorios anteriores
Write-Host ""
Write-Host "[1/4] Limpando pastas de saida..." -ForegroundColor Yellow
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
if (Test-Path $distDir) { Remove-Item $distDir -Recurse -Force }
New-Item -ItemType Directory -Path $distDir -Force | Out-Null

# 2. Publicacao Single-File Self-Contained x64
Write-Host ""
Write-Host "[2/4] Publicando executavel unico .NET 10 (win-x64 single-file)..." -ForegroundColor Yellow
dotnet publish "$projectDir\CGPDI.StudyLab.csproj" `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o "$publishDir" `
    /p:Version=$Version `
    /p:AssemblyVersion="$Version.0" `
    /p:FileVersion="$Version.0"

# Remove arquivos de debug
Get-ChildItem -Path "$publishDir" -Filter "*.pdb" | Remove-Item -Force

# 3. Empacotamento do Executavel Direto e ZIP Portatil Limpo
Write-Host ""
Write-Host "[3/4] Gerando executavel direto e arquivo ZIP portatil..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Executável portátil direto (1 arquivo único)
Copy-Item "$publishDir\CGPDI.StudyLab.exe" "$distDir\CGPDI-StudyLab-v$Version-Portable.exe" -Force
Write-Host "✔ Executavel Portatil Direto gerado: $distDir\CGPDI-StudyLab-v$Version-Portable.exe" -ForegroundColor Green

# Arquivo de instruções
"CGPDI StudyLab v$Version - Executavel Unico Portatil`r`n`r`nBasta dar dois cliques em CGPDI.StudyLab.exe para iniciar!`r`nNao requer instalacao nem direitos de administrador." | Out-File -FilePath "$publishDir\COMO-USAR.txt" -Encoding utf8

$zipPath = "$distDir\CGPDI-StudyLab-v$Version-Portable-win-x64.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
[System.IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
Write-Host "✔ Pacote Portatil ZIP gerado: $zipPath" -ForegroundColor Green

# 4. Compilacao do Instalador (se o Inno Setup estiver instalado)
Write-Host ""
Write-Host "[4/4] Verificando Inno Setup para compilar instalador (.exe)..." -ForegroundColor Yellow
$isccPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (Test-Path $isccPath) {
    & $isccPath "/DMyAppVersion=$Version" "$PSScriptRoot\installer\setup_script.iss"
    if (Test-Path "$PSScriptRoot\dist-installer\CGPDI-StudyLab-Setup.exe") {
        Move-Item "$PSScriptRoot\dist-installer\CGPDI-StudyLab-Setup.exe" "$distDir\CGPDI-StudyLab-v$Version-Setup.exe" -Force
        if (Test-Path "$PSScriptRoot\dist-installer") { Remove-Item "$PSScriptRoot\dist-installer" -Recurse -Force }
        Write-Host "✔ Instalador gerado: $distDir\CGPDI-StudyLab-v$Version-Setup.exe" -ForegroundColor Green
    }
} else {
    Write-Host "ℹ Inno Setup nao detectado localmente. O instalador sera compilado automaticamente via GitHub Actions no CI/CD!" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "🎉 Concluido com sucesso! Arquivos prontos na pasta: $distDir" -ForegroundColor Cyan
