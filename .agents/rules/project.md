---
trigger: always_on
description: Enforce CGPDI.StudyLab project conventions — dark palette, WindowStyle=None windows, mandatory tests, and build/test commands.
---

# Regras do Projeto CGPDI.StudyLab

Aplicativo WPF (.NET 10) de Computação Gráfica e Processamento Digital de Imagens.

## Comandos obrigatórios
- Build: `dotnet build CGPDI.StudyLab.slnx`
- Testes: `dotnet test CGPDI.StudyLab.Tests/CGPDI.StudyLab.Tests.csproj`
- Após qualquer mudança de código, rode **build + testes** antes de concluir.
- Após modificar código-fonte, rode `graphify update .` para manter o grafo atual.

## Regra de qualidade (obrigatória — SonarQube)
- Após mudanças em código (`*.cs`, `*.xaml`, `*.csproj`), rode `powershell -ExecutionPolicy Bypass -File sonar-scan.ps1` (begin → build → testes com cobertura → end).
- Servidor local `http://localhost:9000` (CE 26.8, JDK 21), dashboard `id=cgpdi-studylab`, token em `.sonar-token`/`SONAR_TOKEN`.
- Não conclua com novos bugs/vulnerabilidades. Code smells/duplicação: reduza ao tocar no arquivo. Cobertura exige testes.
- Servidor offline: iniciar `StartSonar.bat` em `C:\tools\sonarqube\sonarqube-26.8.0.126808\bin\windows-x86-64` e aguardar `UP`.

## Regra de segurança (obrigatória)
- Após mudanças em dependências (`*.csproj`, `package.json`) ou código (`*.cs`, `*.xaml`), rode **`snyk test --all-projects`** (dependências) e **`snyk code test`** (análise estática) antes de concluir.
- Não conclua tarefa com achados `high/critical` não resolvidos — corrija ou documente exceção no `.snyk`. Achados `low/medium` são pendências.
- Falhas de auth → `snyk auth` ou `SNYK_TOKEN`/`SNYK_CFG_ORG`.
- Servidor MCP do Snyk configurado em `.opencode/opencode.json` (servidor `snyk`).

## Regra de testes (obrigatória)
- Toda funcionalidade nova ou bug corrigido DEVE ter um teste em `CGPDI.StudyLab.Tests` (unitário em `UnitTests/` e/ou de UI em `UiTests/`).
- Não conclua tarefa com teste faltando. Regressão sem teste = tarefa incompleta.
- Testes de UI usam `[UIFact]` / `[WpfFact]` (Xunit.StaFact) e liberam recursos com `Close()`/`Dispose()`.

## Regra do Changelog (obrigatória)
- Toda mudança visível ao usuário DEVE entrar no `CHANGELOG.md` em `## [Unreleased]` (seções `### Adicionado`, `### Corrigido`, `### Alterado`, `### Removido`, `### Segurança`, em português).
- A seção do `[Unreleased]` vira o corpo da release no GitHub Releases (extraída pelo workflow `release-app.yml`, que falha se a seção da versão não existir) e é exibida dentro do app no diálogo de atualização.
- Ao criar release, renomeie `## [Unreleased]` para `## [vX.Y.Z] - AAAA-MM-DD` antes de gerar a tag.
- Mudanças internas (refatoração sem efeito visível, docs de código) NÃO precisam de entrada.

## Consistência de estilo (NÃO quebrar o que funciona)
- Use a paleta escura existente (#181825, #182032, #252F46, texto #E2E8F0/#94A3B8) e os estilos de App.xaml. Reutilize recursos; não duplique cores inline.
- Janelas usam `WindowStyle="None"` + controles próprios de minimizar/maximizar/fechar. Mantenha esse padrão.
- Antes de mudar um estilo, confira as abas/Janelas que o compartilham (ex.: quiz, canvas, WPF) para não cortar/desalinhar.
- Fórmulas matemáticas com `MathFormulaRenderer`; código com `CSharpSyntaxHighlighter`/`XamlSyntaxHighlighter`. Não substitua por texto puro.

## Fluxo de janelas / Estúdio / Laboratório
- Estúdio de Projetos é aba própria na `MainWindow` e também abre em janela (`ProjectStudioWindow`) / tela cheia.
- Laboratório abre em janela separada (`CodeStudioWindow`). Não recoloque o estúdio dentro do laboratório.
- Rotação 3D automática usa `WpfViewport3DManager.RotateCamera` via `_timer3D` no `MainWindow`. Não quebre esse fluxo.

## Convenções de código
- Namespaces: `CGPDI.StudyLab.Core` (utilidades), `.Graphics2D`, `.Graphics3D`, `.ImageProcessing`, `.Views`.
- Imagens usam `DirectBitmap` (acesso com `Lock()`/`Unlock()`); libere recursos com `using`.
- Código C#/XAML editável é executado via `LiveCodeCompiler`. Erros de compilação devem retornar mensagem amigável, não travar a UI.
- Sem comentários supérfluos no código novo.
