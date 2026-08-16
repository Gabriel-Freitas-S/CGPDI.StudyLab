# CGPDI.StudyLab — Regras do Projeto

Aplicativo WPF (.NET 10) de Computação Gráfica e Processamento Digital de Imagens.
Arquitetura: `MainWindow` (7 abas) + janelas/controles separados de Estúdio e Laboratório.

## Comandos obrigatórios
- Build: `dotnet build CGPDI.StudyLab.slnx`
- Testes: `dotnet test CGPDI.StudyLab.Tests/CGPDI.StudyLab.Tests.csproj`
- Após qualquer mudança de código, rode **build + testes** antes de concluir.
- Após modificar código-fonte, rode `graphify update .` para manter o grafo atual (AST-only, sem custo de API).

## Regra de testes (obrigatória)
- **Toda funcionalidade nova ou bug corrigido DEVE ter um teste** em `CGPDI.StudyLab.Tests` (unitário em `UnitTests/` e/ou de UI em `UiTests/`).
- Não conclua uma tarefa com teste faltando. Regressão sem teste = tarefa incompleta.
- Testes de UI usam `[UIFact]` / `[WpfFact]` (Xunit.StaFact) e liberam recursos com `Close()`/`Dispose()`.

## Consistência de estilo (NÃO quebrar o que funciona)
- Use a paleta escura existente (#181825, #182032, #252F46, texto #E2E8F0/#94A3B8) e os mesmos estilos de App.xaml. Reutilize recursos de `App.xaml`; não duplique cores inline.
- Janelas do app usam `WindowStyle="None"` + controles próprios de minimizar/maximizar/fechar. Mantenha esse padrão.
- Antes de mudar um estilo, confira as abas/Janelas que o compartilham (ex.: quiz, canvas, WPF) para não cortar/desalinhar elementos.
- Fórmulas matemáticas são renderizadas com `MathFormulaRenderer`; código com `CSharpSyntaxHighlighter`/`XamlSyntaxHighlighter`. Não substitua por texto puro.

## Fluxo de janelas / Estúdio / Laboratório
- O Estúdio de Projetos é aba própria na `MainWindow` e também abre em janela (`ProjectStudioWindow`) / tela cheia.
- O Laboratório abre em janela separada (`CodeStudioWindow`). Não recoloque o estúdio dentro do laboratório.
- Navegação: há um menu para Estúdio; respeite o vínculo entre Central de Estudos ↔ Laboratório ↔ Estúdio.
- Rotação 3D automática usa `WpfViewport3DManager.RotateCamera` via `_timer3D` no `MainWindow`. Não quebre esse fluxo.

## Convenções de código
- Namespaces: `CGPDI.StudyLab.Core` (utilidades), `.Graphics2D`, `.Graphics3D`, `.ImageProcessing`, `.Views`.
- Imagens usam `DirectBitmap` (acesso com `Lock()`/`Unlock()`); libere recursos com `using`.
- Código C#/XAML editável é executado via `LiveCodeCompiler`. Erros de compilação devem retornar mensagem amigável, não travar a UI.
- Sem comentários supérfluos no código novo; siga o estilo dos arquivos vizinhos.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

When the user types `/graphify`, use the installed graphify skill or instructions before doing anything else.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- Dirty graphify-out/ files are expected after hooks or incremental updates; dirty graph files are not a reason to skip graphify. Only skip graphify if the task is about stale or incorrect graph output, or the user explicitly says not to use it.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
