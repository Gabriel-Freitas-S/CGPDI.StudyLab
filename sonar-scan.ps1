param(
    [string]$Server = "http://localhost:9000",
    [string]$ProjectKey = "cgpdi-studylab",
    [string]$Token = $env:SONAR_TOKEN
)

$ErrorActionPreference = "Stop"

if (-not $Token) {
    $tokenFile = Join-Path $PSScriptRoot ".sonar-token"
    if (Test-Path $tokenFile) {
        $Token = (Get-Content $tokenFile -Raw).Trim()
    }
}

if (-not $Token) {
    Write-Host "Token do SonarQube nao fornecido. Use -Token, a variavel SONAR_TOKEN ou o arquivo .sonar-token." -ForegroundColor Red
    exit 1
}

if (-not $env:JAVA_HOME -and (Test-Path "C:\Program Files\Eclipse Adoptium")) {
    $jdk = Get-ChildItem "C:\Program Files\Eclipse Adoptium" -Directory | Sort-Object Name -Descending | Select-Object -First 1
    $env:JAVA_HOME = $jdk.FullName
    $env:PATH = "$($jdk.FullName)\bin;$env:PATH"
}

$exclusions = "**/obj/**,**/bin/**,dist/**,publish/**,graphify-out/**,**/node_modules/**,**/*.g.cs,**/*.g.i.cs,.opencode/**,TestResults/**"

function Run-Step([string]$Name, [scriptblock]$Action) {
    Write-Host "== $Name ==" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FALHOU: $Name" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

Run-Step "SonarScanner begin" {
    dotnet-sonarscanner begin /k:$ProjectKey /d:sonar.host.url=$Server /d:sonar.token=$Token `
        /d:sonar.exclusions=$exclusions `
        /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" `
        /d:sonar.coverage.exclusions=$exclusions
}

Run-Step "dotnet build" {
    dotnet build CGPDI.StudyLab.slnx
}

Run-Step "dotnet test (com cobertura opencover)" {
    dotnet test CGPDI.StudyLab.Tests/CGPDI.StudyLab.Tests.csproj `
        --collect:"XPlat Code Coverage;Format=opencover" `
        --results-directory "TestResults"
}

Run-Step "SonarScanner end" {
    dotnet-sonarscanner end /d:sonar.token=$Token
}

Write-Host "Dashboard: $Server/dashboard?id=$ProjectKey" -ForegroundColor Green