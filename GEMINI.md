# CGPDI.StudyLab — Regras específicas do Antigravity

Este arquivo é lido pelo Google Antigravity e tem precedência sobre `AGENTS.md`
para regras conflitantes. Mantenha regras universais em `AGENTS.md` e aqui apenas
as específicas deste agente.

## Antes de qualquer mudança
- Leia o `AGENTS.md` e o `fix e melhorias.md` para entender o contexto e evitar
  repetir erros já corrigidos.
- Confira o estilo compartilhado entre abas/janelas (quiz, canvas, WPF, estúdio)
  antes de alterar qualquer XAML — a causa mais comum de regressão é desalinhar
  um elemento que várias telas usam.

## Consistência de estilo (prioridade máxima)
- **Nunca** introduza cores, padding ou tamanhos inline diferentes da paleta
  (#181825, #182032, #252F46, #E2E8F0, #94A3B8). Reutilize recursos de `App.xaml`.
- Mantenha o padrão `WindowStyle="None"` + botões próprios de janela.
- Sempre verifique se uma mudança de estilo afeta: quiz, canvas 2D, WPF/Viewbox,
  estúdio e laboratório antes de concluir.

## Gate de qualidade
- Toda mudança precisa passar por `dotnet build` + `dotnet test` antes de concluir.
- Toda feature nova ou bug corrigido precisa de teste em `CGPDI.StudyLab.Tests`.
- Após alterar código, rode `graphify update .`.

## Regra do Changelog (obrigatória)
- Toda mudança visível ao usuário DEVE entrar no `CHANGELOG.md` em `## [Unreleased]`
  (seções `### Adicionado`, `### Corrigido`, `### Alterado`, `### Removido`, `### Segurança`,
  em português). A seção vira o corpo da release no GitHub e é exibida no diálogo de
  atualização do app — o workflow `release-app.yml` falha se a seção da versão não existir.
- Ao criar release, renomeie `## [Unreleased]` para `## [vX.Y.Z] - AAAA-MM-DD`.
- Mudanças internas (refatoração sem efeito visível, docs de código) não precisam de entrada.
