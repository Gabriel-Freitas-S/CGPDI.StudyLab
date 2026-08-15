---
title: Como Instalar o Visual Studio & Rodar o Projeto (Passo a Passo)
description: Guia completo para instalar o Visual Studio Community, configurar o ambiente .NET 10 e executar o CGPDI.StudyLab com 1 clique.
---

Se você nunca instalou o Visual Studio no seu computador, siga este guia passo a passo ilustrado. Em menos de 10 minutos você terá o projeto rodando na sua tela!

---

## 📋 Pré-Requisitos do Sistema

- **Sistema Operacional:** Windows 10 ou Windows 11 (64-bits).
- **Processador:** Qualquer CPU moderna (Intel Core i3/i5/i7/i9 ou AMD Ryzen).
- **Memória RAM:** 4 GB mínimo (8 GB ou mais recomendado).
- **Espaço em Disco:** Cerca de 4 a 6 GB livres.

---

## 🛠️ Passo 1: Baixar o Visual Studio Community (Gratuito)

1. Acesse o site oficial da Microsoft: [visualstudio.microsoft.com](https://visualstudio.microsoft.com/pt-br/vs/community/).
2. Clique no botão azul **"Baixar o Visual Studio Community"** (é 100% gratuito para estudantes, professores e código aberto).
3. Um arquivo chamado `VisualStudioSetup.exe` será baixado. Abra-o e clique em **Continuar**.

---

## 📦 Passo 2: Selecionar a Carga de Trabalho Obrigatória

Durante a instalação, o instalador do Visual Studio perguntará o que você deseja desenvolver.

:::caution[Atenção: Marque esta opção!]
Na aba **"Cargas de trabalho" (Workloads)**, localize e marque a opção:
✅ **"Desenvolvimento para desktop com .NET"** (*.NET desktop development*).
:::

Na coluna da direita, certifique-se de que estão marcados:
- Ferramentas de desenvolvimento do .NET
- Suporte a WPF / Windows Forms
- .NET 10 Runtime / SDK (ou superior)

Clique em **Instalar** (ou *Modificar*) no canto inferior direito e aguarde o download finalizar.

---

## 📂 Passo 3: Abrir o Projeto CGPDI.StudyLab

Após a instalação terminar:

1. Abra a pasta onde você baixou ou clonou este repositório:
   ```
   D:\source\repos\CGPDI.StudyLab
   ```
2. Localize o arquivo de solução **`CGPDI.StudyLab.slnx`** (ou `CGPDI.StudyLab.csproj`).
3. Dê **duplo clique** sobre ele. O Visual Studio será iniciado automaticamente com todos os arquivos do projeto carregados na barra lateral (*Gerenciador de Soluções* / *Solution Explorer*).

---

## ▶️ Passo 4: Compilar e Executar com 1 Clique

No topo da janela do Visual Studio, você verá uma barra de ferramentas com um botão verde com o símbolo de "Play" escrito **`CGPDI.StudyLab`**:

```
[ ▶ CGPDI.StudyLab ]  |  [ Debug ]  |  [ Any CPU ]
```

1. Clique no botão verde de **Play** (ou simplesmente aperte a tecla **`F5`** no seu teclado).
2. O Visual Studio irá compilar o código C# e em poucos segundos a janela do **CGPDI.StudyLab** se abrirá em tela cheia!

:::tip[Dica de Atalho]
Se quiser rodar o programa sem o depurador anexado (para máxima velocidade de processamento de imagem), pressione **`Ctrl + F5`** (*Iniciar Sem Depurar*).
:::

---

## ❓ Perguntas Frequentes (FAQ do Iniciante)

### 1. Apareceu um aviso sobre "Código Não Seguro" (`unsafe`)?
Não se preocupe! O projeto utiliza o modificador `unsafe` para manipular pixels diretamente na memória RAM com ponteiros `byte*`. O arquivo `.csproj` já vem com a configuração `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` habilitada por padrão.

### 2. O Visual Studio diz que o .NET 10 não foi encontrado?
Se você estiver em uma versão mais antiga do Visual Studio, baixe o [.NET 10 SDK Oficial da Microsoft](https://dotnet.microsoft.com/download/dotnet/10.0) e instale o arquivo `.exe`. Depois feche e reabra o Visual Studio.

### 3. Como vejo os arquivos do código no Visual Studio?
Na barra lateral direita, procure pela janela chamada **Gerenciador de Soluções** (*Solution Explorer*). Se ela estiver oculta, clique no menu superior **Exibir** $\to$ **Gerenciador de Soluções** (ou pressione `Ctrl + Alt + L`).

---

👉 **Próximo Passo:** Se você prefere usar o terminal sem abrir o Visual Studio, leia o [Guia da Linha de Comando (CLI)](/CGPDI.StudyLab/iniciantes/guia-linha-de-comando/).
