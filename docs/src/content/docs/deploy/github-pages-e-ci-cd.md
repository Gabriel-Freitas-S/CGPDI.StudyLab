---
title: Publicação no GitHub Pages & Automação CI/CD
description: Como configurar o repositório no GitHub para publicar este site de documentação Astro Starlight automaticamente a cada commit.
---

Esta documentação foi construída com **Astro e Starlight** e está pronta para ser publicada no **GitHub Pages** de forma 100% gratuita e automatizada através do **GitHub Actions**.

---

## 🚀 1. O Workflow Automatizado (`.github/workflows/deploy-docs.yml`)

O repositório já inclui o arquivo de automação [`.github/workflows/deploy-docs.yml`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/.github/workflows/deploy-docs.yml):

```yaml
name: Deploy Documentation to GitHub Pages

on:
  push:
    branches: [master, main]
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

## ⚙️ 2. Ativando o GitHub Pages no seu Repositório (Passo a Passo)

Para habilitar a publicação no GitHub pela primeira vez:

1. Acesse seu repositório no GitHub: `https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab`.
2. Clique na aba **Settings** (Configurações) no menu superior do repositório.
3. Na barra lateral esquerda, clique em **Pages** (dentro da seção *Code and automation*).
4. Em **Build and deployment $\to$ Source**, mude de *"Deploy from a branch"* para:
   👉 **`GitHub Actions`**.
5. Salve a alteração!

```
[ Build and deployment ]
Source: [ GitHub Actions ▼ ]
```

---

## 🌐 3. Endereço Oficial da Documentação Online

Assim que você fizer um `git push` para o branch `main`, o GitHub Actions compilará os arquivos e seu site estará disponível mundialmente em:

👉 **`https://gabriel-freitas-s.github.io/CGPDI.StudyLab/`**

---

## 💻 4. Testando a Documentação Localmente

Se você quiser testar ou editar os textos da documentação no seu computador antes de enviar para o GitHub:

1. Abra o terminal na pasta `docs`:
```powershell
cd D:\source\repos\CGPDI.StudyLab\docs
```

2. Inicie o servidor local de desenvolvimento do Astro:
```powershell
npm run dev
```

3. Abra seu navegador no endereço: `http://localhost:4321/CGPDI.StudyLab/`. 
Todas as alterações em arquivos `.md` e fórmulas KaTeX serão atualizadas instantaneamente com *Live Reload*!

4. Para testar o build final de produção:
```powershell
npm run build
```

---

🎉 **Parabéns!** Você tem agora uma documentação de nível internacional, interativa, didática e pronta para a nuvem!
