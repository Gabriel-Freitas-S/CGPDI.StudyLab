---
name: sonarqube-quality
description: Run the SonarQube code quality analysis (scanner + tests + coverage) and report findings
---

# Workflow: sonarqube-quality

Roda a análise de qualidade do código com SonarQube local e reporta as métricas.

## Passos

1. **Verificar servidor**: `Invoke-WebRequest http://localhost:9000/api/system/status` deve retornar `status: UP`.
   - Se offline, iniciar: `C:\tools\sonarqube\sonarqube-26.8.0.126808\bin\windows-x86-64\StartSonar.bat` (requer JDK 21) e aguardar UP.
2. **Rodar análise**: `powershell -ExecutionPolicy Bypass -File sonar-scan.ps1`
   - Faz: SonarScanner begin → `dotnet build` → `dotnet test` (cobertura opencover) → SonarScanner end.
   - Token lido de `.sonar-token` ou `SONAR_TOKEN`.
3. **Consultar métricas** (API, token via Basic Auth):
   - `GET /api/measures/component?component=cgpdi-studylab&metricKeys=alert_status,bugs,vulnerabilities,code_smells,coverage,duplicated_lines_density,ncloc`
4. **Interpretar**:
   - `alert_status` deve ser `OK` (Quality Gate).
   - Novos bugs/vulnerabilidades → corrigir antes de concluir.
   - Code smells/duplicação → dívida técnica: reduzir ao tocar no arquivo.
   - Cobertura baixa → adicionar testes.
5. **Reportar** resumo: gate, bugs, vulnerabilidades, code smells, cobertura %, duplicação %, ncloc.