---
name: snyk-security
description: Run Snyk security scans (open-source dependencies + Snyk Code) on the project and report findings
---

# Workflow: snyk-security

Roda a varredura de segurança do projeto com o Snyk CLI e reporta o resultado.

## Passos

1. **Verificar CLI**: `snyk version` — deve ser >= 1.1298.0. Caminho: `C:\tools\snyk\snyk.exe`.
2. **Scan de dependências**: `snyk test --all-projects`
   - Cobre NuGet (`CGPDI.StudyLab`, `CGPDI.StudyLab.Tests`) e npm (`.opencode`, `docs`).
3. **Scan de código**: `snyk code test`
   - Cobre `.cs` (WPF/ImageProcessing/Graphics), `.js`, `.ts`.
4. **Interpretar**:
   - `ok: true` e 0 vulnerabilidades → concluir com sucesso.
   - Achados `high/critical` → corrigir (atualizar pacote) ou documentar exceção em `.snyk` com justificativa. Não concluir com high/critical pendente.
   - Achados `low/medium` → reportar como pendência, não bloqueiam.
   - Falha de autenticação/org → `snyk auth` ou `SNYK_TOKEN`/`SNYK_CFG_ORG`; não silenciar o erro.
5. **Reportar** o resumo (nº de projetos, nº de achados por severidade, arquivos cobertos).

Se invocado via MCP, use as tools do servidor `snyk` em vez do CLI direto.