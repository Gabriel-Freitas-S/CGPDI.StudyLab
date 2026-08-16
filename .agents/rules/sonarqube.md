---
trigger: always_on
description: Enforce SonarQube code quality analysis (local server + dotnet-sonarscanner) after code changes, before concluding tasks.
---

## Qualidade com SonarQube (obrigatória)

O projeto usa **SonarQube Community Edition 26.8** local (http://localhost:9000) com o **SonarScanner for .NET** (`dotnet-sonarscanner`) para medir qualidade do código: bugs, vulnerabilidades, code smells, duplicação e cobertura.

### Comandos
- Análise completa: `powershell -ExecutionPolicy Bypass -File sonar-scan.ps1` (begin → build → testes com cobertura opencover → end).
- O script lê o token de `.sonar-token` (arquivo local, não versionado) ou `SONAR_TOKEN`.
- Credenciais do servidor: `admin` (senha definida localmente, fora do repo).
- Dashboard: http://localhost:9000/dashboard?id=cgpdi-studylab

### Regras
- Após mudanças em código (`*.cs`, `*.xaml`, `*.csproj`), rode `sonar-scan.ps1` antes de concluir.
- **Não conclua tarefa que introduza novos bugs ou vulnerabilidades** (compare com a análise anterior). Corrija ou justifique.
- Code smells e duplicação são dívida técnica: reduza quando tocar no arquivo, sem bloquear.
- Cobertura: rode os testes com o scanner (o script já gera `coverage.opencover.xml`); não publique feature sem teste.
- Se o servidor estiver offline, tente iniciar com: `StartSonar.bat` em `C:\tools\sonarqube\sonarqube-26.8.0.126808\bin\windows-x86-64` (requer JDK 21) e aguarde status `UP` em `http://localhost:9000/api/system/status`.
- Exclusões padrão da análise: `obj/`, `bin/`, `dist/`, `publish/`, `graphify-out/`, `node_modules/` (configuradas no `sonar-scan.ps1`).

### Métricas monitoradas
- `bugs`, `vulnerabilities`, `code_smells`, `coverage`, `duplicated_lines_density`, `ncloc`, `alert_status` (Quality Gate).