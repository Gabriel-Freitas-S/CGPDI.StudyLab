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
@"
============================================================
  CGPDI StudyLab v$Version - Guia de Execução e Instalação
============================================================

1. Executável Portátil Direto:
   Basta dar dois cliques em CGPDI.StudyLab.exe para iniciar!
   Não requer instalação nem direitos de administrador.

2. Instalador Velopack (Setup.exe):
   Instala na pasta do usuário (%LocalAppData%) e cria atalhos na
   Área de Trabalho e Menu Iniciar, com atualizações automáticas delta.

3. Instalador Machine-Wide (.msi):
   Ideal para laboratórios e uso institucional em computadores compartilhados.

Em caso de qualquer falha na inicialização, consulte o log de diagnóstico em:
%LocalAppData%\CGPDI.StudyLab\logs\crash.log
"@ | Out-File -FilePath "$publishDir\COMO-USAR.txt" -Encoding utf8

$zipPath = "$distDir\CGPDI-StudyLab-v$Version-Portable-win-x64.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
[System.IO.Compression.ZipFile]::CreateFromDirectory($publishDir, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
Write-Host "✔ Pacote Portatil ZIP gerado: $zipPath" -ForegroundColor Green

# 4. Empacotamento com Velopack (instalador + delta) se o vpk estiver instalado
Write-Host ""
Write-Host "[4/4] Verificando Velopack CLI (vpk) para empacotar instalador..." -ForegroundColor Yellow
$vpk = Get-Command vpk -ErrorAction SilentlyContinue
if ($vpk) {
    $assetsPath = "$PSScriptRoot\CGPDI.StudyLab\Assets"

    $packArgs = @(
        "pack",
        "-u", "CGPDIStudyLab",
        "-v", $Version,
        "-p", "$publishDir",
        "-e", "CGPDI.StudyLab.exe",
        "--packTitle", "CGPDI StudyLab",
        "--icon", "$assetsPath\app_icon.ico",
        "--shortcuts", "Desktop,StartMenuRoot",
        "--instLocation", "Either"
    )
    if (Test-Path "$assetsPath\installer_splash.png") {
        $packArgs += @("--splashImage", "$assetsPath\installer_splash.png", "--splashProgressColor", "#38BDF8")
    }
    if (Test-Path "$assetsPath\installer_welcome.txt") {
        $packArgs += @("--instWelcome", "$assetsPath\installer_welcome.txt")
    }
    if (Test-Path "$assetsPath\installer_readme.md") {
        $packArgs += @("--instReadme", "$assetsPath\installer_readme.md")
    }
    if (Test-Path "$assetsPath\installer_conclusion.txt") {
        $packArgs += @("--instConclusion", "$assetsPath\installer_conclusion.txt")
    }
    $packArgs += @("-o", "$PSScriptRoot\Releases")

    & vpk @packArgs

    if (Test-Path "$PSScriptRoot\Releases\CGPDIStudyLab-win-Setup.exe") {
        Copy-Item "$PSScriptRoot\Releases\CGPDIStudyLab-win-Setup.exe" "$distDir\CGPDI-StudyLab-v$Version-Setup.exe" -Force
        Copy-Item "$PSScriptRoot\Releases\CGPDIStudyLab-win-Setup.exe" "$distDir\CGPDIStudyLab-win-Setup.exe" -Force
        Copy-Item "$PSScriptRoot\Releases\CGPDIStudyLab-win-Setup.exe" "$distDir\CGPDI-StudyLab-Setup.exe" -Force
        Write-Host "✔ Instalador Velopack gerado: $distDir\CGPDIStudyLab-win-Setup.exe (e aliases)" -ForegroundColor Green
    } else {
        Write-Host "ℹ O vpk não gerou o Setup.exe em 'Releases'." -ForegroundColor DarkGray
    }

    $msiArgs = @(
        "pack",
        "-u", "CGPDIStudyLab",
        "-v", $Version,
        "-p", "$publishDir",
        "-e", "CGPDI.StudyLab.exe",
        "--packTitle", "CGPDI StudyLab",
        "--icon", "$assetsPath\app_icon.ico",
        "--msi", "true"
    )
    if (Test-Path "$assetsPath\msi_banner.png") {
        $msiArgs += @("--msiBanner", "$assetsPath\msi_banner.png")
    }
    if (Test-Path "$assetsPath\msi_dialog_logo.png") {
        $msiArgs += @("--msiLogo", "$assetsPath\msi_dialog_logo.png")
    }
    $msiArgs += @("-o", "$PSScriptRoot\Releases-msi")

    & vpk @msiArgs

    if (Test-Path "$PSScriptRoot\Releases-msi\CGPDIStudyLab-win.msi") {
        Copy-Item "$PSScriptRoot\Releases-msi\CGPDIStudyLab-win.msi" "$distDir\CGPDI-StudyLab-MachineWide.msi" -Force
        Write-Host "✔ Instalador Machine-Wide (MSI) gerado: $distDir\CGPDI-StudyLab-MachineWide.msi" -ForegroundColor Green
    }
} else {
    Write-Host "ℹ Velopack CLI (vpk) nao detectado localmente. O instalador sera gerado automaticamente via GitHub Actions no CI/CD!" -ForegroundColor DarkGray
    Write-Host "  Instale com: dotnet tool install -g vpk" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "🎉 Concluido com sucesso! Arquivos prontos na pasta: $distDir" -ForegroundColor Cyan
