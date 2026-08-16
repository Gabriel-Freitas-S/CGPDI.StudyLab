---
trigger: always_on
description: Enforce Snyk security scans (open-source dependencies + Snyk Code) after any code change, before concluding tasks.
---

## Segurança com Snyk (obrigatória)

O projeto usa o **Snyk CLI** para varredura de segurança de dependências (open-source) e análise estática de código (Snyk Code). O servidor MCP está configurado em `.opencode/opencode.json` (`snyk`).

### Comandos
- CLI: `snyk` (em `C:\tools\snyk\snyk.exe`), versão >= 1.1298.0.
- Dependências: `snyk test --all-projects`
- Código: `snyk code test`
- Via MCP: use as tools do servidor `snyk` (ex.: `snyk_test_code`, `snyk_test`).

### Regras
- Após qualquer mudança em `*.csproj`, `package.json`/`package-lock.json` ou `*.cs`/`*.xaml`, rode **`snyk test --all-projects`** e **`snyk code test`** antes de concluir.
- Não conclua tarefa com achados de severidade **high/critical** não resolvidos. Em caso de achado, corrija (ex.: `dotnet add package`, `npm audit fix`) ou documente a exceção no `.snyk` com justificativa.
- Snyk Code retorna SARIF; 0 achados = `ok: true`. Achados `low/medium` podem ser reportados como pendências, não bloqueiam.
- Se o scan falhar por autenticação/org, rode `snyk auth` (browser) ou use `SNYK_TOKEN`/`SNYK_CFG_ORG` como fallback; nunca trave a tarefa silenciosamente.
- Em CI, os mesmos comandos rodam no workflow GitHub Actions (Snyk + CodeQL).

### Troubleshooting (docs.snyk.io)
- Verifique `snyk version` (>= 1.1298.0).
- Scan direto do terminal antes de suspeitar do MCP.
- Transport alternativo SSE: `snyk mcp -t sse --experimental` (se o host bloquear stdio).
- Logs: `snyk test -d`, `snyk code test -d`, `SNYK_LOG_LEVEL=trace`.