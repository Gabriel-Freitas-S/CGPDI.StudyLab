---
title: Execução pela Linha de Comando (PowerShell / Terminal)
description: Como compilar e executar o CGPDI.StudyLab sem abrir o Visual Studio utilizando os comandos do .NET CLI.
---

Para quem utiliza editores leves como VS Code, Cursor ou prefere automatizar tarefas pelo terminal do Windows (PowerShell ou Prompt de Comando), o **.NET SDK** oferece uma interface de linha de comando (*CLI*) simples e direta.

---

## 1. Abrindo o Terminal

1. Abra o menu Iniciar do Windows e digite **PowerShell** (ou pressione as teclas `Win + X` e selecione **Terminal**).
2. Navegue até o diretório do projeto:

```powershell
cd D:\source\repos\CGPDI.StudyLab\CGPDI.StudyLab
```

---

## 2. Verificação do Ambiente .NET

Antes da compilação, verifique se o compilador está acessível no sistema:

```powershell
dotnet --version
```

A saída deverá exibir a versão instalada (por exemplo, `10.0.xxx`).

---

## 3. Compilando o Projeto (dotnet build)

Para verificar se o código não possui erros de sintaxe e gerar os arquivos binários:

```powershell
dotnet build
```

**Resultado esperado:**
```
Determinando os projetos a serem restaurados...
Todos os projetos estao atualizados para restauracao.
CGPDI.StudyLab -> D:\source\repos\CGPDI.StudyLab\CGPDI.StudyLab\bin\Debug\net10.0-windows\CGPDI.StudyLab.dll

Compilacao com exito.
    0 Aviso(s)
    0 Erro(s)
```

---

## 4. Executando a Aplicação (dotnet run)

Para iniciar o aplicativo imediatamente:

```powershell
dotnet run
```

A janela gráfica do **CGPDI.StudyLab** será aberta na área de trabalho.

:::tip[Modo Release para Máximo Desempenho]
Por padrão, `dotnet run` utiliza o modo `Debug`. Para avaliar algoritmos pesados (como o Ray Tracer) com máxima otimização do compilador:
```powershell
dotnet run -c Release
```
:::

---

## 5. Limpeza de Arquivos Temporários (dotnet clean)

Para limpar os arquivos temporários de compilação das pastas `bin/` e `obj/`:

```powershell
dotnet clean
```

---

👉 **Próximo Passo:** Aprenda a [Depurar e Navegar pelo Código com Pontos de Interrupção](/CGPDI.StudyLab/iniciantes/depuracao-e-truques/).
