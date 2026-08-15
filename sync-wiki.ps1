# Script de Sincronizacao Automatica da Wiki do GitHub com GH CLI
$ErrorActionPreference = "Stop"

# Garante PATH atualizado
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")
Remove-Item env:GITHUB_TOKEN -ErrorAction SilentlyContinue

Write-Host "🔍 Obtendo token de autenticacao via GitHub CLI (gh)..." -ForegroundColor Cyan
$token = (gh auth token).Trim()

if (-not $token) {
    Write-Error "Nao foi possivel obter o token do GitHub CLI. Execute 'gh auth login' primeiro."
    exit 1
}

$repoOwner = "Gabriel-Freitas-S"
$repoName = "CGPDI.StudyLab"
$wikiUrl = "https://x-access-token:$token@github.com/$repoOwner/$repoName.wiki.git"

$wikiDir = Join-Path $PSScriptRoot "wiki"
if (-not (Test-Path $wikiDir)) {
    Write-Error "Diretorio 'wiki' nao encontrado em $wikiDir"
    exit 1
}

Set-Location $wikiDir

Write-Host "📦 Preparando arquivos da Wiki..." -ForegroundColor Cyan
if (-not (Test-Path ".git")) {
    git init
}

git config user.name "Gabriel-Freitas-S"
git config user.email "Gabriel-Freitas-S@users.noreply.github.com"
git remote remove origin -ErrorAction SilentlyContinue
git remote add origin $wikiUrl
git add .
git commit -m "docs: sync complete 7 chapters of CGPDI.StudyLab wiki" -ErrorAction SilentlyContinue
git branch -M master

Write-Host "🚀 Enviando capitulos da Wiki para o GitHub..." -ForegroundColor Green
try {
    git push -u origin master --force
    Write-Host "✅ Wiki sincronizada com sucesso no GitHub!" -ForegroundColor Green
    Write-Host "👉 Acesse: https://github.com/$repoOwner/$repoName/wiki" -ForegroundColor Yellow
} catch {
    Write-Host "`n⚠️ O repositorio da Wiki ainda nao foi ativado pelo GitHub." -ForegroundColor Yellow
    Write-Host "1. Abra no navegador: https://github.com/$repoOwner/$repoName/wiki" -ForegroundColor White
    Write-Host "2. Clique no botao verde 'Create the first page' e depois em 'Save page'." -ForegroundColor White
    Write-Host "3. Execute este script novamente: .\sync-wiki.ps1" -ForegroundColor Cyan
}
