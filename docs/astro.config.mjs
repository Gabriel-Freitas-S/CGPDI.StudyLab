import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';

// https://astro.build/config
export default defineConfig({
  site: 'https://gabriel-freitas-s.github.io',
  base: '/CGPDI.StudyLab',
  markdown: {
    remarkPlugins: [remarkMath],
    rehypePlugins: [rehypeKatex],
  },
  integrations: [
    starlight({
      title: 'CGPDI.StudyLab',
      description: 'Documentação técnica e laboratório universitário de Computação Gráfica e Processamento Digital de Imagens (.NET 10 / C# / WPF)',
      logo: {
        alt: 'CGPDI StudyLab Logo',
        src: './src/assets/logo.svg',
      },
      social: {
        github: 'https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab',
      },
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
            
            function initMermaid() {
              const isDark = document.documentElement.dataset.theme !== 'light';
              mermaid.initialize({
                startOnLoad: false,
                theme: isDark ? 'dark' : 'default',
                securityLevel: 'loose',
                fontFamily: 'inherit',
              });

              document.querySelectorAll('pre:has(code.language-mermaid), pre.mermaid, div.mermaid, pre > code.language-mermaid').forEach(async (el) => {
                const targetPre = el.tagName.toLowerCase() === 'code' ? el.parentElement : el;
                if (targetPre.dataset.mermaidRendered) return;
                targetPre.dataset.mermaidRendered = 'true';
                
                const rawCode = el.textContent || '';
                const id = 'mermaid-svg-' + Math.random().toString(36).substring(2, 9);
                
                try {
                  const { svg } = await mermaid.render(id, rawCode.trim());
                  const wrapper = document.createElement('div');
                  wrapper.className = 'mermaid-container';
                  wrapper.style.display = 'flex';
                  wrapper.style.justifyContent = 'center';
                  wrapper.style.margin = '1.5rem 0';
                  wrapper.style.overflowX = 'auto';
                  wrapper.innerHTML = svg;
                  targetPre.replaceWith(wrapper);
                } catch (err) {
                  console.error('Erro ao renderizar Mermaid:', err);
                }
              });
            }

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
            { label: '2. Instalando o Visual Studio', link: '/iniciantes/instalacao-visual-studio/' },
            { label: '3. Executando pelo Terminal (CLI)', link: '/iniciantes/guia-linha-de-comando/' },
            { label: '4. Depuração e Truques (Debug)', link: '/iniciantes/depuracao-e-truques/' },
          ],
        },
        {
          label: 'Arquitetura do Software',
          items: [
            { label: 'Visão Geral da Arquitetura', link: '/arquitetura/visao-geral/' },
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
