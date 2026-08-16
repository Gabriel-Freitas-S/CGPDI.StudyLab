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

## Regra de testes (obrigatória)
- Toda funcionalidade nova ou bug corrigido DEVE ter um teste em `CGPDI.StudyLab.Tests` (unitário em `UnitTests/` e/ou de UI em `UiTests/`).
- Não conclua tarefa com teste faltando. Regressão sem teste = tarefa incompleta.
- Testes de UI usam `[UIFact]` / `[WpfFact]` (Xunit.StaFact) e liberam recursos com `Close()`/`Dispose()`.

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
