# Changelog — CGPDI StudyLab

Todas as mudanças relevantes para o usuário deste projeto serão documentadas neste arquivo.

O formato segue [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o versionamento é [SemVer](https://semver.org/lang/pt-BR/).

Cada seção publicada vira o corpo da release no GitHub Releases e é exibida
dentro do app no diálogo de atualização. **Toda mudança visível ao usuário DEVE
ser adicionada em `## [Unreleased]`** (veja a regra em `AGENTS.md`).

## [Unreleased]

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