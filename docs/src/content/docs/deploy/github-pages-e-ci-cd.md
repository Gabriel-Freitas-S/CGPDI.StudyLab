---
title: Publicação no GitHub Pages & CI/CD de Releases do Aplicativo
description: Automação completa no GitHub Actions para deploy contínuo da documentação e geração de instaladores (.exe) e pacotes portáteis (.zip) com auto-updater.
---

O projeto [`CGPDI.StudyLab`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab) conta com uma esteira completa de **Integração e Entrega Contínuas (CI/CD)** dividida em dois workflows:

1. **Deploy Contínuo da Documentação**: Compila o site Astro Starlight e publica no GitHub Pages.
2. **Build & Release do Aplicativo Windows**: Compila o .NET 10, empacota o executável portátil (`.zip`) e o instalador oficial (`.exe` com Inno Setup) e publica no GitHub Releases.

---

## 🚀 1. Pipeline de Release do Aplicativo (`release-app.yml`)

O workflow [`.github/workflows/release-app.yml`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/.github/workflows/release-app.yml) é disparado automaticamente ao criar uma tag (ex: `v1.0.1`) ou manualmente via `workflow_dispatch`:

```mermaid
graph TD
    A["Tag Git vX.Y.Z ou Disparo Manual"] --> B["GitHub Actions Windows Runner"]
    B --> C["Compilação .NET 10 Release"]
    C --> D["dotnet publish win-x64 Self-Contained"]
    D --> E["Compactação ZIP Portátil"]
    D --> F["Compilação do Instalador com Inno Setup"]
    E --> G["GitHub Releases Oficial"]
    F --> G
    G --> H["Auto-Updater no Aplicativo Notifica os Usuários"]
```

### Artefatos Gerados em Cada Release:
* **`CGPDI-StudyLab-Setup.exe`**: Instalador clássico para Windows com atalho na Área de Trabalho, Menu Iniciar e desinstalador completo.
* **`CGPDI-StudyLab-Portable-win-x64.zip`**: Versão portátil autônoma pronta para rodar sem necessidade de instalação ou direitos de administrador.

---

## 🔄 2. Sistema de Auto-Update Integrado

O aplicativo possui um sistema inteligente de verificação e atualização automática ([`UpdateManager.cs`](file:///d:/source/repos/CGPDI.StudyLab/CGPDI.StudyLab/Core/UpdateManager.cs)):

1. **Verificação em Segundo Plano:** Ao iniciar, o app consulta a API do GitHub Releases (`/releases/latest`) de forma não bloqueante.
2. **Diálogo de Atualização:** Se uma versão mais recente for encontrada (ex: `v1.0.1 > v1.0.0`), uma janela moderna exibe o changelog e pergunta ao usuário se deseja atualizar.
3. **Download e Aplicação Automática:**
   - **Instalador:** Faz o download e executa o assistente de instalação silencioso.
   - **Portátil:** Baixa o `.zip`, descompacta em segundo plano e reinicia a aplicação atualizada.
4. **Verificação Manual:** O botão **`🔔 Atualizações`** na barra superior permite checar novidades a qualquer momento.

---

## 🌐 3. Deploy da Documentação no GitHub Pages (`deploy-docs.yml`)

O repositório inclui o workflow [`.github/workflows/deploy-docs.yml`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/.github/workflows/deploy-docs.yml) que sincroniza a documentação a cada commit na branch `main`. O workflow gera o arquivo `CNAME` automaticamente, habilitando o domínio customizado.

A documentação está publicada em:
👉 **`https://cgpdi.gabrielfs.dev`**

Para que o domínio funcione, o DNS do provedor deve apontar para o GitHub Pages:

| Tipo | Nome | Valor |
| :--- | :--- | :--- |
| `CNAME` | `cgpdi.gabrielfs.dev` | `gabriel-freitas-s.github.io` |

Além do registro DNS, o domínio `cgpdi.gabrielfs.dev` deve estar cadastrado em **Settings → Pages → Custom domain** do repositório. O `CNAME` embutido no artefato de deploy garante que ele seja recriado a cada publicação.

---

## 💻 4. Geração Local de Release

Para compilar o pacote de release localmente sem enviar para o GitHub, basta executar o script PowerShell:

```powershell
.\build_release.ps1 -Version "1.0.0"
```

Os arquivos finais serão gerados na pasta `dist/`.
