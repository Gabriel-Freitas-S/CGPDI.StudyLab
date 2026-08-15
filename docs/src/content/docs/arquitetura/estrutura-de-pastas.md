---
title: Estrutura de Pastas e Arquivos do Projeto
description: Um mapa detalhado de cada diretório, arquivo de código-fonte e sua respectiva responsabilidade no sistema.
---

O repositório do **CGPDI.StudyLab** é organizado de forma modular e limpa. Abaixo você encontra a descrição completa de cada componente.

---

## 📂 Visão Geral da Árvore de Diretórios

```
📦 CGPDI.StudyLab
├── 📄 CGPDI.StudyLab.slnx          # Arquivo de Solução moderno do Visual Studio
├── 📄 Plano de Ensino.md           # Ementa oficial da disciplina universitária
├── 📄 README.md                    # Apresentação do projeto e instruções de execução
├── 📂 docs/                        # Site de documentação oficial em Astro Starlight
│   ├── 📄 astro.config.mjs         # Configurações do Astro, Starlight e KaTeX
│   ├── 📄 package.json             # Dependências e scripts da documentação
│   └── 📂 src/content/docs/        # Conteúdo das páginas em Markdown/MDX
├── 📂 .github/workflows/           # Automação de CI/CD para o GitHub Pages
│   └── 📄 deploy-docs.yml          # Workflow de build e deploy automático
└── 📂 CGPDI.StudyLab/              # Código-fonte principal da aplicação WPF
    ├── 📄 CGPDI.StudyLab.csproj    # Manifesto de compilação C# (.NET 10)
    ├── 📄 App.xaml / App.xaml.cs   # Ponto de entrada (Entry Point) da aplicação WPF
    ├── 📄 MainWindow.xaml          # Interface visual com abas, painéis e controles
    ├── 📄 MainWindow.xaml.cs       # Controlador de eventos e cronômetro
    ├── 📂 Core/                    # Núcleo de memória, cores e geradores
    ├── 📂 ImageProcessing/         # Módulos de Processamento Digital de Imagens (PDI)
    ├── 📂 Graphics2D/              # Algoritmos de Rasterização e Álgebra 2D
    └── 📂 Graphics3D/              # Pipeline 3D, Shaders, Robô e Ray Tracer
```

---

## 🔬 Detalhamento dos Módulos C#

### 1. Pasta `Core/` (Núcleo de Infraestrutura)

| Arquivo | Responsabilidade |
| :--- | :--- |
| **`DirectBitmap.cs`** | Classe de manipulação direta de memória com ponteiros `unsafe byte*`, formato `Bgra32` e alinhamento `Stride`. Suporta clonagem, leitura, escrita rápida e paralelismo `Parallel.For`. |
| **`ColorSpaces.cs`** | Funções matemáticas de conversão entre modelos cromáticos: RGB, HSV, HSL, YCbCr (JPEG), CMYK, Escala de Cinza Perceptiva (ITU-R BT.709 e BT.601), Sépia e Inversão. |
| **`ImageSampleGenerator.cs`** | Gerador procedual de imagens sintéticas de calibração ótica (tabuleiro de xadrez, gradientes de luminância, círculos concêntricos e barras de cor SMPTE). |
| **`StudyGuideData.cs`** | Base de conhecimento teórica embutida no aplicativo com fórmulas matemáticas, explicações conceituais e códigos comentados. |

---

### 2. Pasta `ImageProcessing/` (Processamento Digital de Imagens)

| Arquivo | Responsabilidade |
| :--- | :--- |
| **`PointAndHistograms.cs`** | Operações pontuais $g(x, y) = T[f(x, y)]$, ajuste de brilho linear, contraste com pivô central, Look-Up Tables (LUTs), correção Gamma, equalização de histograma por CDF e normalização Min-Max. |
| **`SpatialFilters.cs`** | Filtragem linear e não-linear no domínio espacial: Convolução 2D genérica, Box Blur, Filtro Gaussiano com desvio padrão $\sigma$, Unsharp Masking, Filtro da Mediana e Detecção de Bordas (Sobel, Prewitt, Scharr, Laplaciano e Algoritmo Canny de 5 etapas). |
| **`Morphology.cs`** | Morfologia Matemática e Binarização: Algoritmo de Otsu (variância interclasses), Erosão, Dilatação, Abertura, Fechamento, Gradiente Morfológico, Top-Hat e Black-Hat com elementos estruturantes configuráveis. |
| **`GeometricTransforms.cs`** | Mapeamento inverso com interpolação vizinho mais próximo, bilinear e bicúbica. Rotação afim, cisalhamento e deformações espaciais (Swirl, Onda/Ripple e Olho de Peixe). |
| **`FrequencyAndProcedural.cs`** | Transformada Discreta de Fourier 2D (DFT), centralização de frequência `FFTShift`, espectros de magnitude/fase, filtros passa-baixa/passa-alta em frequência, Ruído de Perlin, Terrenos Fractais fBm, Voronoi e Fractais de Mandelbrot e Julia. |

---

### 3. Pasta `Graphics2D/` (Rasterização 2D dos Primeiros Princípios)

| Arquivo | Responsabilidade |
| :--- | :--- |
| **`Matrix2D.cs`** | Matrizes $3 \times 3$ em coordenadas homogêneas para transformações afins 2D (Translação, Rotação, Escala, Cisalhamento e composição encadeada). |
| **`Rasterizer2D.cs`** | Algoritmos fundamentais de rasterização: Traçado de Linhas DDA, Linha de Bresenham (aritmética 100% inteira), Linhas Suavizadas com Anti-Aliasing de Xiaolin Wu, Círculo do Ponto Médio (8 octantes), Elipse do Ponto Médio, Curvas de Bézier Cúbicas (Bernstein / De Casteljau), Scanline Polygon Fill com AET e Recorte Cohen-Sutherland. |

---

### 4. Pasta `Graphics3D/` (Computação Gráfica 3D, Pipeline e Ray Tracing)

| Arquivo | Responsabilidade |
| :--- | :--- |
| **`Math3D.cs`** | Estruturas de dados vetoriais e matriciais: `Vec3`, `Vec4`, `Mat4x4`, `Quaternion`, `Ray3D` e geradores de matrizes MVP (Model, View LookAt, Projection Perspectiva e Ortográfica). |
| **`SoftwareRenderer3D.cs`** | Pipeline gráfico completo 3D implementado em CPU: Leitura de malhas `Mesh3D`, Back-face Culling, Divisão Perspectiva para NDC, Rasterização de triângulos por Coordenadas Baricêntricas, Algoritmo de Profundidade Z-Buffer e Iluminação Flat/Gouraud. |
| **`WpfViewport3DManager.cs`** | Renderização 3D acelerada por hardware via DirectX no WPF: Controle de malhas `MeshGeometry3D`, Câmera Orbital Arcball com coordenadas esféricas $(r, \theta, \phi)$ e controle por mouse, Fontes de Luz e Materiais Phong. |
| **`HierarchicalModeling.cs`** | Grafos de Cena (Scene Graphs) e Robô Articulado com Cinemática Direta (Unidade 3 do curso), com propagação de matrizes pai-filho e animações em tempo real. |
| **`Raytracer3D.cs`** | Ray Tracer físico de Whitted: Resolução analítica de equação quadrática para interseção raio-esfera, interseção com plano xadrez, raios de sombra, reflexões especulares e refração transparente pela Lei de Snell e coeficientes de Fresnel. |

---

👉 **Próximo Passo:** Entenda como funcionam o [WPF, XAML e a Renderização em Tempo Real](/CGPDI.StudyLab/arquitetura/wpf-e-xaml-explicados/).
