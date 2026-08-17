# Changelog — CGPDI StudyLab

Todas as mudanças relevantes para o usuário deste projeto serão documentadas neste arquivo.

O formato segue [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o versionamento é [SemVer](https://semver.org/lang/pt-BR/).

Cada seção publicada vira o corpo da release no GitHub Releases e é exibida
dentro do app no diálogo de atualização. **Toda mudança visível ao usuário DEVE
ser adicionada em `## [Unreleased]`** (veja a regra em `AGENTS.md`).

## [Unreleased]

### Adicionado
- **Screenshots e GIF Animado Automatizados na Documentação**:
  - Novo script PowerShell `scripts/capture-screenshots.ps1` que compila o app, o inicia, navega por todas as 7 abas via UIAutomation e captura cada uma com `PrintWindow` (GDI32 P/Invoke), salvando em `docs/public/screenshots/`.
  - GIF animado `docs/public/gifs/demo.gif` gerado automaticamente via FFmpeg a partir dos frames de cada aba (slideshow 2 s/frame, paleta otimizada, escala 1280px).
  - Novo workflow GitHub Actions `capture-screenshots.yml` que dispara a cada push em `main` (alterações em `CGPDI.StudyLab/**`), instala FFmpeg via Chocolatey e commita os assets atualizados com `[skip ci]`.
  - Galeria visual interativa na homepage da documentação Astro (`index.mdx`) com abas (`Tabs`) mostrando screenshot de cada módulo.
  - Seção "📸 Interface" no `README.md` com o GIF animado e tabela de screenshots em `<details>`, servidos pelo GitHub Pages em `cgpdi.gabrielfs.dev`.
- **Versionamento automático no desenvolvimento com MinVer**: builds locais agora derivam a versão do histórico git (tags `v*` + altura de commits), eliminando a parametrização manual de versão para pacotes de desenvolvimento. O GitHub Actions continua usando versão explícita (`MinVerSkip=true`), sem impacto nos instaladores publicados.
- **Personalização Visual dos Instaladores**:
  - Tela Splash temática Dark Slate (`#0D0E18`) com barra de progresso em Ciano Elétrico (`#38BDF8`) e logotipo 3D cristalino para o instalador Velopack (`Setup.exe`).
  - Banner superior personalizado (`493x58 px`) e tela de diálogo com logotipo (`493x312 px`) no padrão WiX para o instalador corporativo MSI (`.msi`).
  - Textos de boas-vindas (`installer_welcome.txt`), introdução (`installer_readme.md`) e conclusão (`installer_conclusion.txt`) customizados com informações sobre o .NET 10 e a plataforma.
- **Página de Documentação — Fluxo de Desenvolvimento & Ferramentas**:
  - Novo capítulo detalhando todas as 11 tecnologias e ferramentas do projeto (.NET 10, WPF, Roslyn Scripting, Velopack, WiX, SonarQube, Snyk, Graphify, MinVer, xUnit, Astro Starlight), explicando seu papel no ecossistema com diagrama interativo Mermaid.
- **Identidade Visual e Favicon na Documentação**:
  - Configuração do favicon vetorial oficial transparente (`/favicon.svg`) e padronização do portal Astro sem uso de emojis nos títulos e diagramas, priorizando ícones nativos e tipografia limpa.

### Segurança
- **Blindagem contra Ataques na Cadeia de Suprimentos (Supply Chain Hardening)**:
  - **Ecossistema .NET / NuGet**:
    - Criação de `nuget.config` com `<clear />` e `Package Source Mapping` para prevenção contra *Dependency Confusion* e *Typosquatting*.
    - Ativação de `RestorePackagesWithLockFile` e geração de lockfiles `packages.lock.json` com hashes criptográficos SHA-512 de todas as dependências diretas e transitivas.
    - Ativação de `NuGetAudit` com auditoria em tempo de restore para todas as dependências (`NuGetAuditMode=all`).
  - **Ecossistema Node.js / npm**:
    - Criação de arquivos `.npmrc` (raiz e `docs/`) com `ignore-scripts=true` (bloqueio de execução arbitrária de código em `preinstall`/`postinstall`), `save-exact=true`, `package-lock=true` e `audit=true`.
    - Fixação determinística de versões exatas no `docs/package.json`.
  - **Testes Automatizados de Conformidade**:
    - Criação de `SupplyChainSecurityTests.cs` (4 testes automatizados) e integração à suíte de testes xUnit (118 testes totais).

### Alterado
- Modernização de expressões regulares para geradores de código em tempo de compilação com `[GeneratedRegex]`, eliminando riscos de ReDoS e melhorando a performance na renderização de sintaxe C#/XAML e fórmulas matemáticas.
- Refatoração de comparadores de ponto flutuante em conversões de espaços de cores para tolerâncias seguras com epsilon (`1e-9`).
- Otimização do gerenciamento de recursos gráficos WPF com congelamento (`Freeze()`) de pincéis estáticos, reduzindo alocação e prevenindo retenção desnecessária de memória.

### Corrigido
- **Captura automática no GitHub Actions mais estável**: o pipeline de screenshots/GIF agora força resolução virtual Full HD, adiciona aquecimento da janela/abas e retentativas de captura, reduzindo truncamentos visuais e frames incompletos em runners Windows Server.
- **Robustez do compilador ao vivo (Roslyn) em ambientes automatizados**: o `LiveCodeCompiler` passou a inicializar referências de compilação com fallback seguro (incluindo TPA) e tratamento resiliente de falhas, evitando quebra global por exceções de inicialização.
- **Compatibilidade do Script de Screenshots no PowerShell 7 e CI**: Substituída a dependência de `System.Drawing.Bitmap` no script `scripts/capture-screenshots.ps1` por P/Invoke direto para Win32 GDI/GDI+ (`gdiplus.dll`/`gdi32.dll`), eliminando o erro de compilação dinâmica `CS1069` (tipo `Bitmap` não resolvido no runtime .NET Core do `pwsh` no GitHub Actions).
- **Falha de empacotamento MSI no GitHub Actions** (`There is a release in channel win which is equal or greater to the current version`): o diretório `Releases-msi/` (com `releases.win.json` obsoleto) foi removido do controle de versão — o Velopack agora empacota sempre em canal limpo no runner, permitindo republicar a mesma versão sem conflito de canal.
- **Compatibilidade e Transparência dos Ícones Multi-Resolução**: Refatorada a estrutura interna do arquivo `app_icon.ico` para a especificação DIB 32bpp nativa do Windows com cabeçalho `BITMAPINFOHEADER` e máscara AND de transparência nas resoluções 16x16 a 128x128 e stream PNG em 256x256, eliminando definitivamente o fundo preto nos atalhos da Área de Trabalho, Menu Iniciar, barra de tarefas e instaladores Velopack/MSI.
- **Resolução de Conflito de Versão BAML/Assembly na Inicialização**: Removido o MinVer do build de publicação e unificada a parametrização de versão via MSBuild (`Version`, `AssemblyVersion`, `FileVersion`, `InformationalVersion`), garantindo que o compilador BAML do WPF e a compilação C# usem exatamente o mesmo `AssemblyVersion`, eliminando a falha fatal (`FileNotFoundException` no `App.InitializeComponent`) na abertura do aplicativo após a instalação via EXE ou MSI.
- Ícones da aplicação com fundo transparente no gerador vetorial `AppIconHelper` e nos assets SVG/PNG/ICO.
- Resiliência na inicialização pós-instalação (EXE e MSI): implementado tratamento global de exceções (`DispatcherUnhandledException`, `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`) com gravação de diagnóstico em `%LocalAppData%\CGPDI.StudyLab\logs\crash.log` e diálogo informativo em caso de falhas de hardware ou permissões.
- Inclusão dos arquivos de `Assets` diretamente na distribuição do executável (`CopyToOutputDirectory="PreserveNewest"`), evitando tentativas desnecessárias de escrita em diretórios protegidos como `Program Files`.
- Tratamento explícito de cancelamento e descarte seguro de `CancellationTokenSource` no diálogo de atualizações.
- Correção de operações binárias em métodos de teste dinâmico do compilador em tempo de execução.
- **Responsividade e Contenção de Layout no Astro Docs**: adicionado sistema global de contenção de grid (`min-width: 0`, `overflow-wrap: anywhere`), scroll horizontal seguro para tabelas e quebra responsiva de cards e código inline, evitando colisões com o sumário lateral.
- **Renderização de Componentes Starlight**: migração de arquivos com imports JSX para extensão `.mdx` e ajuste da configuração do Astro para execução adequada de `<CardGrid>`, `<Steps>` e `<Tabs>`.
- **Formato de Imagens para o Instalador WiX MSI**: geração dos assets `msi_banner.bmp` (493x58) e `msi_dialog_logo.bmp` (493x312) no formato nativo Windows Bitmap (`.bmp`) exigido pelo WiX Toolset / Velopack, corrigindo a falha de empacotamento no GitHub Actions.

## [v1.0.5] - 2026-08-16

### Adicionado
- Migração do auto-update para o **Velopack**: instalador moderno em `%LocalAppData%` (sem UAC), **atualizações delta** (baixa apenas as alterações, frequentemente < 5 MB) e notas da release embutidas no pacote.
- Fallback inteligente de atualização para laboratórios universitários: caso a máquina esteja em modo *machine-wide* (`Program Files`) e o aluno/professor não possua permissão de administrador nem tarefa SYSTEM disponível, o app faz fallback automático e aplica a versão mais recente no diretório local do usuário (`%LocalAppData%`) ou pacote portátil, garantindo atualização instantânea sem depender de chamados de TI.
- Badge informativo de ambiente de instalação (`Instalação da TI (Zero-Admin)`, `Instalação por Usuário (Zero-Admin)` ou `Modo Portátil`) no diálogo de atualizações.
- Guia completo e análise de segurança de TI para laboratórios universitários em `docs/src/content/docs/iniciantes/cenario-universitario-sem-admin.md` abordando conformidade com CIS Benchmarks, NIST SP 800-53, AppLocker e prevenção de escalação de privilégios.
- Diálogo de atualização reformulado: changelog formatado com seções coloridas (Adicionado/Corrigido/Alterado), destaque de código e negrito; modo delta mostra o tamanho do download otimizado.
- Botão "Ignorar esta versão" no diálogo de atualização — a versão ignorada não é mais oferecida em verificações automáticas.
- "Lembrar mais tarde" agora é persistente: a notificação fica suspensa por 7 dias por versão.
- Tamanho dos pacotes (instalador/portátil) exibido no diálogo de atualização.
- `CHANGELOG.md` como fonte única de notas de release: o pipeline CI extrai a seção da versão e a publica no GitHub Releases e dentro dos pacotes Velopack.
- Instalação com escolha entre **"apenas este usuário"** e **"todos os usuários (requer administrador)"** no instalador.
- Instalador **MSI** (`CGPDI-StudyLab-MachineWide.msi`) para implantação machine-wide via GPO/Intune pela equipe de TI.
- Atualização automática em instalações para todos os usuários **sem depender da TI**: ao clicar em "Atualizar Agora", o app aplica tudo em segundo plano via tarefa agendada SYSTEM (criada na instalação) ou reiniciando-se com permissão de administrador — sem janelas extras e sem downloads manuais.

### Corrigido
- Reutilização de `HttpClient` no `UpdateManager` (evita esgotamento de conexões a cada verificação).
- Notificação de atualização não incomoda mais a cada inicialização quando o usuário já adiou/ignorou a versão.
- Atualização manual de pacote `.zip` em diretórios somente leitura (como `Program Files` executado por usuário não-administrador) agora redireciona a extração com segurança para o diretório de usuário sem falhas de permissão.

### Segurança
- Atualizações verificadas via HTTPS (TLS 1.3) com hashes de integridade do GitHub Releases oficial e proteção contra injeção de parâmetros arbitrários na tarefa agendada SYSTEM.

### Removido
- Instalador Inno Setup (`installer/`) — substituído pelo instalador Velopack (`Setup.exe`).

## [v1.0.4] - 2026-08-16

### Adicionado
- Sistema de verificação e instalação automática de atualizações via GitHub Releases (`UpdateManager`), com diálogo de progresso de download.
- Estrutura inicial WPF do projeto consolidada.
- Documentação abrangente dos módulos de estudo e pipeline de deploy.

## [v1.0.3] - 2026-08-16

### Adicionado
- Estúdio de Projetos: interface completa com renderização matemática e compilação de código ao vivo.
- `WpfViewport3DManager`: câmera arcball e geometria 3D paramétrica, com testes unitários.
- Configuração de release no CI/CD.

## [v1.0.0] - 2026-08-15

### Adicionado
- Lançamento inicial do CGPDI StudyLab — Estúdio de Computação Gráfica e Processamento Digital de Imagens:
  - PDI: filtros, histogramas, morfologia, detecção de bordas e operações pontuais.
  - Rasterização 2D: Bresenham, círculos, elipses, preenchimento e recorte.
  - Computação Gráfica 3D: pipeline MVP, iluminação Blinn-Phong e renderização em software.
  - Ray Tracing: interseções, reflexões e refrações.
  - Central de Estudos com quizzes de fixação.
  - Laboratório interativo de C#/WPF.
- Documentação completa com Astro Starlight, suporte a Mermaid e deploy no GitHub Pages.
- Sistema de atualização do aplicativo e publicação automática de releases.
- Workflows de GitHub Actions para build, empacotamento e release de binários Windows.
- Análise de segurança CodeQL.