---
title: Publicação no GitHub Pages & Automação CI/CD
description: Como configurar o repositório no GitHub para publicar este site de documentação Astro Starlight automaticamente a cada commit.
---

Esta documentação foi construída com **Astro e Starlight** e está configurada para publicação contínua no **GitHub Pages** por meio do **GitHub Actions**.

---

## 1. O Workflow Automatizado (.github/workflows/deploy-docs.yml)

O repositório inclui o arquivo de automação [`.github/workflows/deploy-docs.yml`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/.github/workflows/deploy-docs.yml):

```yaml
name: Deploy Documentation to GitHub Pages

on:
  push:
    branches: [main]
    paths:
      - 'docs/**'
      - '.github/workflows/deploy-docs.yml'
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: 'pages'
  cancel-in-progress: true

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: 22
          cache: npm
          cache-dependency-path: docs/package-lock.json
      - run: npm ci || npm install
        working-directory: ./docs
      - run: npm run build
        working-directory: ./docs
      - uses: actions/upload-pages-artifact@v3
        with:
          path: ./docs/dist

  deploy:
    needs: build
    runs-on: ubuntu-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    steps:
      - id: deployment
        uses: actions/deploy-pages@v4
```

---

## 2. Ativação do GitHub Pages no Repositório

Para habilitar a publicação no GitHub:

1. Acesse o repositório no GitHub: `https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab`.
2. Acesse a aba **Settings** (Configurações) no menu superior do repositório.
3. Na barra lateral esquerda, clique em **Pages** (na seção *Code and automation*).
4. Em **Build and deployment $\to$ Source**, selecione a opção:
   - **`GitHub Actions`**.
5. Salve a configuração.

---

## 3. Endereço da Documentação Online

Após o envio de novos commits para a branch `main`, o GitHub Actions processará o build e o site estará disponível em:

👉 **`https://gabriel-freitas-s.github.io/CGPDI.StudyLab/`**

---

## 4. Execução Local

Para testar as páginas localmente no seu computador:

1. Abra o terminal na pasta `docs`:
```powershell
cd D:\source\repos\CGPDI.StudyLab\docs
```

2. Inicie o servidor de desenvolvimento:
```powershell
npm run dev
```

3. Abra o navegador no endereço: `http://localhost:4321/CGPDI.StudyLab/`.
