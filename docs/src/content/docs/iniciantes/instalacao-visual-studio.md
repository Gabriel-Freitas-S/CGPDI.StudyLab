---
title: Instalação do Visual Studio & Execução do Projeto
description: Guia passo a passo para instalar o Visual Studio Community, configurar o ambiente .NET 10 e executar o CGPDI.StudyLab.
---

Se você nunca instalou o Visual Studio no seu computador, siga este roteiro ilustrado. Em poucos minutos o projeto estará em execução na sua tela.

---

## 1. Pré-Requisitos do Sistema

- **Sistema Operacional:** Windows 10 ou Windows 11 (64-bits).
- **Processador:** Qualquer CPU moderna (Intel Core i3/i5/i7/i9 ou AMD Ryzen).
- **Memória RAM:** 4 GB mínimo (8 GB recomendado).
- **Espaço em Disco:** Cerca de 4 a 6 GB livres.

---

## 2. Passo 1: Baixar o Visual Studio Community (Gratuito)

1. Acesse a página oficial da Microsoft: [visualstudio.microsoft.com](https://visualstudio.microsoft.com/pt-br/vs/community/).
2. Clique no botão **"Baixar o Visual Studio Community"** (licença gratuita para estudantes, professores e código aberto).
3. Abra o arquivo executável baixado (`VisualStudioSetup.exe`) e clique em **Continuar**.

---

## 3. Passo 2: Selecionar a Carga de Trabalho

Durante a instalação, o instalador perguntará quais ferramentas você deseja instalar.

:::caution[Seleção Obrigatória]
Na aba **"Cargas de trabalho" (Workloads)**, localize e marque a opção:
- **"Desenvolvimento para desktop com .NET"** (*.NET desktop development*).
:::

No painel lateral direito, certifique-se de que estejam marcados:
- Ferramentas de desenvolvimento do .NET
- Suporte a WPF / Windows Forms
- .NET 10 Runtime / SDK (ou versão mais recente)

Clique em **Instalar** no canto inferior direito e aguarde a conclusão do download.

---

## 4. Passo 3: Abrir o Projeto CGPDI.StudyLab

Após a conclusão da instalação:

1. Abra a pasta do repositório no seu computador:
   ```
   D:\source\repos\CGPDI.StudyLab
   ```
2. Localize o arquivo de solução **`CGPDI.StudyLab.slnx`** (ou `CGPDI.StudyLab.csproj`).
3. Dê **duplo clique** sobre o arquivo. O Visual Studio será aberto com a estrutura completa do projeto no *Gerenciador de Soluções* (*Solution Explorer*).

---

## 5. Passo 4: Compilar e Executar

No topo da janela do Visual Studio, localize a barra de ferramentas principal com o botão de início:

```
[ Iniciar: CGPDI.StudyLab ]  |  [ Debug ]  |  [ Any CPU ]
```

1. Clique no botão de **Iniciar** (ou pressione a tecla **`F5`**).
2. O compilador processará o código C# e abrirá a janela do **CGPDI.StudyLab** em tela cheia.

:::tip[Execução em Alta Performance]
Para rodar a aplicação com velocidade máxima de processamento de imagem sem o depurador anexado, pressione **`Ctrl + F5`** (*Iniciar Sem Depurar*).
:::

---

## 6. Perguntas Frequentes

### 1. Apareceu um aviso sobre "Código Não Seguro" (`unsafe`)?
Isso é esperado. O projeto utiliza blocos `unsafe` para acessar os pixels diretamente na memória RAM em alta velocidade. O arquivo de configuração `.csproj` já possui a opção `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` habilitada por padrão.

### 2. O Visual Studio informou que o .NET 10 não foi localizado?
Se sua instalação do Visual Studio for anterior à disponibilização do .NET 10, instale o [.NET 10 SDK Oficial](https://dotnet.microsoft.com/download/dotnet/10.0) e reinicie o Visual Studio.

---

👉 **Próximo Passo:** Se você prefere trabalhar pelo terminal sem a interface do Visual Studio, consulte o [Guia de Linha de Comando (CLI)](/iniciantes/guia-linha-de-comando/).
