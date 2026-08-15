---
title: Executando pela Linha de Comando (PowerShell / Terminal)
description: Como compilar e executar o CGPDI.StudyLab sem abrir o Visual Studio usando apenas comandos do .NET CLI.
---

Se você prefere programar pelo VS Code, Cursor, Neovim ou simplesmente quer rodar o projeto rapidamente através do terminal do Windows (PowerShell ou Prompt de Comando CMD), o **.NET SDK** oferece comandos de linha de comando (*CLI*) extremamente fáceis.

---

## 💻 1. Abrindo o Terminal

1. Pressione as teclas `Win + X` no teclado e escolha **Terminal** ou **PowerShell** (ou abra o menu Iniciar e digite `PowerShell`).
2. Navegue até a pasta do projeto com o comando `cd`:

```powershell
cd D:\source\repos\CGPDI.StudyLab\CGPDI.StudyLab
```

---

## 🔍 2. Verificando o .NET SDK Instalado

Antes de compilar, teste se o .NET está acessível no seu computador digitando:

```powershell
dotnet --version
```

A saída esperada deve ser algo como `10.0.xxx` (ou superior). Se o comando não for reconhecido, certifique-se de instalar o [.NET SDK](https://dotnet.microsoft.com/download).

---

## 🔨 3. Compilando o Projeto (`dotnet build`)

Para verificar se há algum erro no código e compilar todos os módulos em binários executáveis:

```powershell
dotnet build
```

**Saída esperada:**
```
Determinando os projetos a serem restaurados...
Todos os projetos estão atualizados para restauração.
CGPDI.StudyLab -> D:\source\repos\CGPDI.StudyLab\CGPDI.StudyLab\bin\Debug\net10.0-windows\CGPDI.StudyLab.dll

Compilação com êxito.
    0 Aviso(s)
    0 Erro(s)
```

---

## 🚀 4. Executando a Aplicação (`dotnet run`)

Para iniciar a aplicação gráfica imediatamente:

```powershell
dotnet run
```

A janela do **CGPDI.StudyLab** se abrirá imediatamente na sua área de trabalho!

:::tip[Executando em Modo Release (Máxima Performance)]
Por padrão, `dotnet run` compila no modo `Debug` (com verificações adicionais). Se você quiser testar algoritmos pesados como Ray Tracing em alta resolução com a velocidade máxima do processador, use:
```powershell
dotnet run -c Release
```
Isso ativa todas as otimizações do compilador do .NET 10 (vetorização SIMD e *inlining* agressivo).
:::

---

## 🧹 5. Limpando Arquivos Temporários (`dotnet clean`)

Se algum dia você quiser limpar caches antigos de compilação da pasta `bin/` e `obj/`:

```powershell
dotnet clean
```

---

👉 **Próximo Passo:** Aprenda a [Depurar e Navegar pelo Código com Breakpoints](/CGPDI.StudyLab/iniciantes/depuracao-e-truques/).
