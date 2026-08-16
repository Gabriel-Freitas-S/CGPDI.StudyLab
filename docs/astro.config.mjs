import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';

// https://astro.build/config
export default defineConfig({
  site: 'https://cgpdi.gabrielfs.dev',
  base: '/',
  markdown: {
    remarkPlugins: [remarkMath],
    rehypePlugins: [rehypeKatex],
  },
  integrations: [
    starlight({
      title: 'CGPDI.StudyLab',
      description: 'Documentação técnica e laboratório universitário de Computação Gráfica e Processamento Digital de Imagens (.NET 10 / C# / WPF)',
      favicon: '/favicon.svg',
      logo: {
        alt: 'CGPDI StudyLab Logo',
        src: './src/assets/logo.svg',
      },
      social: [
        {
          icon: 'github',
          label: 'GitHub',
          href: 'https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab',
        },
      ],
      defaultLocale: 'root',
      locales: {
        root: {
          label: 'Português (Brasil)',
          lang: 'pt-BR',
        },
      },
      customCss: [
        'katex/dist/katex.min.css',
        './src/styles/custom.css',
      ],
      tableOfContents: {
        minHeadingLevel: 2,
        maxHeadingLevel: 4,
      },
      head: [
        {
          tag: 'script',
          attrs: {
            type: 'module',
          },
          content: `
            import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@11/dist/mermaid.esm.min.mjs';

            let currentTheme = '';

            function getMermaidTheme() {
              const isDark = document.documentElement.dataset.theme !== 'light';
              return isDark ? 'dark' : 'default';
            }

            function extractMermaidCode(pre, expressiveContainer) {
              const ecLines = pre.querySelectorAll('.ec-line');
              if (ecLines && ecLines.length > 0) {
                const lines = [];
                ecLines.forEach(line => {
                  lines.push(line.textContent || '');
                });
                return lines.join('\\n').trim();
              }

              if (pre.innerText && pre.innerText.includes('\\n')) {
                return pre.innerText.trim();
              }

              const clone = pre.cloneNode(true);
              clone.querySelectorAll('br').forEach(br => br.replaceWith('\\n'));
              clone.querySelectorAll('div').forEach(div => div.after('\\n'));
              return (clone.textContent || '').trim();
            }

            async function renderDiagram(container, rawCode) {
              const theme = getMermaidTheme();
              mermaid.initialize({
                startOnLoad: false,
                theme: theme,
                securityLevel: 'loose',
                fontFamily: 'inherit',
                themeVariables: {
                  darkMode: theme === 'dark',
                  background: theme === 'dark' ? '#14141E' : '#FFFFFF',
                  primaryColor: theme === 'dark' ? '#3B82F6' : '#2563EB',
                  primaryTextColor: theme === 'dark' ? '#F8FAFC' : '#0F172A',
                  primaryBorderColor: theme === 'dark' ? '#60A5FA' : '#3B82F6',
                  lineColor: theme === 'dark' ? '#94A3B8' : '#64748B',
                  secondaryColor: theme === 'dark' ? '#1E293B' : '#F1F5F9',
                  tertiaryColor: theme === 'dark' ? '#0F172A' : '#E2E8F0',
                }
              });

              const id = 'mermaid-svg-' + Math.random().toString(36).substring(2, 9);
              try {
                const { svg } = await mermaid.render(id, rawCode.trim());
                container.innerHTML = svg;
                container.dataset.renderedTheme = theme;
              } catch (err) {
                console.error('Erro ao renderizar Mermaid:', err, 'Código:', rawCode);
                container.innerHTML = '<div style="color: #ef4444; padding: 1rem; border: 1px dashed #ef4444; border-radius: 8px;">⚠️ Erro ao renderizar diagrama Mermaid.</div>';
              }
            }

            async function initMermaid() {
              const theme = getMermaidTheme();
              currentTheme = theme;

              // 1. Converte novos blocos de código em containers Mermaid
              const codeBlocks = document.querySelectorAll('pre[data-language="mermaid"], pre:has(code.language-mermaid), pre.mermaid, div.mermaid');
              
              for (const pre of codeBlocks) {
                const expressiveContainer = pre.closest('.expressive-code') || pre.closest('figure.frame') || pre;
                if (expressiveContainer.dataset.mermaidProcessed) continue;
                expressiveContainer.dataset.mermaidProcessed = 'true';

                let rawCode = extractMermaidCode(pre, expressiveContainer);
                if (!rawCode) continue;

                const wrapper = document.createElement('div');
                wrapper.className = 'mermaid-container';
                wrapper.dataset.mermaidCode = rawCode;
                
                expressiveContainer.replaceWith(wrapper);
                await renderDiagram(wrapper, rawCode);
              }

              // 2. Re-renderiza containers existentes se o tema mudou
              const existingWrappers = document.querySelectorAll('.mermaid-container[data-mermaid-code]');
              for (const wrapper of existingWrappers) {
                if (wrapper.dataset.renderedTheme !== theme) {
                  await renderDiagram(wrapper, wrapper.dataset.mermaidCode);
                }
              }
            }

            // Observador de mudança de tema Claro/Escuro no Starlight
            const themeObserver = new MutationObserver(() => {
              const theme = getMermaidTheme();
              if (theme !== currentTheme) {
                initMermaid();
              }
            });

            themeObserver.observe(document.documentElement, {
              attributes: true,
              attributeFilter: ['data-theme']
            });

            if (document.readyState === 'loading') {
              document.addEventListener('DOMContentLoaded', initMermaid);
            } else {
              initMermaid();
            }

            document.addEventListener('astro:page-load', initMermaid);
          `,
        },
      ],
      sidebar: [
        {
          label: 'Começando do Zero (Guia do Iniciante)',
          items: [
            { label: 'Visão Geral e Boas-Vindas', link: '/' },
            { label: '1. O que é C#, .NET e WPF?', link: '/iniciantes/o-que-e-dotnet-csharp/' },
            { label: '2. Modo Interativo & Playground Guiado', link: '/iniciantes/modo-interativo-e-playground/' },
            { label: '3. Cenário Universitário & Zero-Admin', link: '/iniciantes/cenario-universitario-sem-admin/' },
            { label: '4. Instalando o Visual Studio', link: '/iniciantes/instalacao-visual-studio/' },
            { label: '5. Executando pelo Terminal (CLI)', link: '/iniciantes/guia-linha-de-comando/' },
            { label: '6. Depuração e Truques (Debug)', link: '/iniciantes/depuracao-e-truques/' },
          ],
        },
        {
          label: 'Arquitetura do Software',
          items: [
            { label: 'Visão Geral da Arquitetura', link: '/arquitetura/visao-geral/' },
            { label: 'Fluxo de Desenvolvimento e Ferramentas', link: '/arquitetura/fluxo-de-desenvolvimento-e-ferramentas/' },
            { label: 'Estrutura de Pastas e Arquivos', link: '/arquitetura/estrutura-de-pastas/' },
            { label: 'WPF, XAML e Renderização em Tempo Real', link: '/arquitetura/wpf-e-xaml-explicados/' },
          ],
        },
        {
          label: 'Núcleo de Memória e Hardware',
          items: [
            { label: 'Fundamentos de Memória e Ponteiros', link: '/core/fundamentos-de-memoria/' },
            { label: 'DirectBitmap e Buffer Bgra32', link: '/core/directbitmap/' },
            { label: 'Modelos de Cor e Percepção Humana', link: '/core/modelos-de-cor/' },
            { label: 'Gerador de Padrões Óticos de Teste', link: '/core/gerador-de-amostras/' },
          ],
        },
        {
          label: 'Processamento Digital de Imagens (PDI)',
          items: [
            { label: '1. Operações Pontuais e Histogramas', link: '/pdi/operacoes-pontuais-e-histogramas/' },
            { label: '2. Filtros Espaciais e Convoluções', link: '/pdi/filtros-espaciais-e-convolucoes/' },
            { label: '3. Detecção de Bordas e Algoritmo Canny', link: '/pdi/deteccao-de-bordas-e-canny/' },
            { label: '4. Morfologia Matemática e Otsu', link: '/pdi/morfologia-matematica-e-otsu/' },
            { label: '5. Transformações Geométricas e Warping', link: '/pdi/transformacoes-geometricas/' },
            { label: '6. Domínio da Frequência (DFT) e Ruídos', link: '/pdi/dominio-da-frequencia-e-ruidos/' },
          ],
        },
        {
          label: 'Computação Gráfica 2D',
          items: [
            { label: '1. Álgebra Linear 2D e Coordenadas Homogêneas', link: '/cg2d/algebra-linear-e-matrizes/' },
            { label: '2. Algoritmos de Reta (DDA, Bresenham, Wu)', link: '/cg2d/algoritmos-de-linhas/' },
            { label: '3. Círculos, Elipses e Curvas de Bézier', link: '/cg2d/circulos-elipses-e-curvas/' },
            { label: '4. Preenchimento Scanline e Recorte Cohen-Sutherland', link: '/cg2d/preenchimento-e-recorte/' },
          ],
        },
        {
          label: 'Computação Gráfica 3D',
          items: [
            { label: '1. Matemática 3D e Matrizes MVP', link: '/cg3d/matematica-vetorial-e-mvp/' },
            { label: '2. Renderizador em Software (CPU 3D Pipeline)', link: '/cg3d/renderizador-em-software/' },
            { label: '3. Viewport3D por Hardware (DirectX e Câmera Arcball)', link: '/cg3d/viewport3d-hardware-wpf/' },
          ],
        },
        {
          label: 'Modelagem Hierárquica',
          items: [
            { label: '1. Grafos de Cena e Cinemática Direta (Teoria)', link: '/hierarquia/grafos-de-cena-e-teoria/' },
            { label: '2. Robô Articulado e Sistema Solar (Prática)', link: '/hierarquia/braco-robotico-e-animacoes/' },
          ],
        },
        {
          label: 'Renderização Realística e Ray Tracing',
          items: [
            { label: '1. Fundamentos do Ray Tracing e Modelo Phong', link: '/raytracing/fundamentos-e-fisica-da-luz/' },
            { label: '2. Interseção Analítica de Raios (Esfera e Plano)', link: '/raytracing/intersecao-e-geometria/' },
            { label: '3. Reflexões Especulares e Refração de Snell', link: '/raytracing/reflexao-refracao-snell/' },
          ],
        },
        {
          label: 'Guia Acadêmico e Avaliações',
          items: [
            { label: 'Mapeamento do Plano de Ensino', link: '/academico/mapeamento-do-plano/' },
            { label: 'Roteiro de Estudos para os Trabalhos T1, T2 e T3', link: '/academico/roteiro-de-estudos-e-avaliacoes/' },
          ],
        },
        {
          label: 'Publicação e GitHub Pages',
          items: [
            { label: 'Configuração do GitHub Pages e CI/CD', link: '/deploy/github-pages-e-ci-cd/' },
          ],
        },
      ],
    }),
  ],
});
