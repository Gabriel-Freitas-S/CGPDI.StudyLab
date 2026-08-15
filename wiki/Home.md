# 🏛️ Bem-vindo à Wiki do CGPDI.StudyLab (Laboratório Universitário de CG & PDI)

Esta Wiki serve como o **manual de referência técnica, matemática e pedagógica** do projeto. O objetivo deste repositório é fornecer uma implementação completa, legível e de altíssimo desempenho de todos os conceitos clássicos e modernos de **Processamento Digital de Imagens (PDI)** e **Computação Gráfica 2D e 3D**.

---

## 🗺️ Mapa de Navegação da Wiki

* [**Capítulo 1: Fundamentos & Manipulação de Memória em Baixo Nível**](./1-Fundamentos-e-Manipulacao-de-Memoria.md)
  * Acesso a memória via ponteiros `unsafe byte*`.
  * Formato `Bgra32`, `Stride` e alinhamento de cache.
  * Paralelismo multinúcleo com `Parallel.For`.
  * Integração de buffer direto com o DirectX do WPF (`WriteableBitmap`).

* [**Capítulo 2: Processamento Digital de Imagens (PDI)**](./2-Processamento-Digital-de-Imagens.md)
  * Modelos de Cor: RGB, HSV, HSL, YCbCr, CMYK, Luminância Perceptiva (BT.709 vs BT.601) e Sépia.
  * Operações Pontuais: Brilho, Contraste Linear, Correção de Gamma (Lei de Potência), Posterização e Solarização.
  * Histogramas: Histograma em tempo real, Equalização por CDF e Normalização Min-Max.
  * Filtros Espaciais: Box Blur, Gaussiano, Sharpen, Unsharp Mask, Mediana e Emboss 3D.
  * Detecção de Bordas: Sobel ($G_x, G_y, G, \theta$), Scharr, Laplaciano ($\nabla^2 f$), Laplaciano do Gaussiano (LoG) e o algoritmo **Canny completo em 5 etapas**.
  * Morfologia Matemática: Limiarização Ótima de Otsu, Limiarização Adaptativa Local, Erosão, Dilatação, Abertura, Fechamento, Gradiente Morfológico e Top-Hat.
  * Transformações Geométricas: Mapeamento Inverso (*Backward Mapping*), Interpolação Vizinho Mais Próximo, Bilinear e Bicúbica (Spline de Catmull-Rom), Rotação, Escala, Cisalhamento, Espelhamento e Deformações Não-Lineares (*Swirl*, *Ripple*, *Fisheye*).
  * Domínio da Frequência & Geração Procedural: Transformada Discreta de Fourier 2D (DFT) com *FFTShift*, Ruído de Perlin 2D, Terrenos Fractais *fBm*, Diagramas de Voronoi e Fractais de Mandelbrot & Julia.

* [**Capítulo 3: Computação Gráfica 2D & Algoritmos de Rasterização**](./3-Computacao-Grafica-2D-e-Rasterizacao.md)
  * Álgebra Linear 2D: Coordenadas Homogêneas e Matrizes $3\times3$ afins.
  * Rasterização de Retas: Bresenham (aritmética 100% inteira), DDA (*Digital Differential Analyzer*) e Xiaolin Wu (*Anti-Aliasing* sub-pixel).
  * Cônicas: Círculo do Ponto Médio (simetria de 8 octantes) e Elipse do Ponto Médio (regiões de derivada e 4 quadrantes).
  * Curvas Paramétricas: Curvas de Bézier Quadráticas e Cúbicas (Polinômios de Bernstein e Algoritmo de De Casteljau).
  * Preenchimento de Polígonos: Algoritmo de Varredura (*Scanline Fill*) com Tabela de Arestas Ativas (*Active Edge Table - AET*).
  * Recorte de Linhas: Algoritmo de Cohen-Sutherland com *Outcodes* de 4 bits.
  * Preenchimento por Inundação: *Flood Fill* baseado em fila (BFS - *Breadth-First Search*).

* [**Capítulo 4: Computação Gráfica 3D & Pipeline Gráfico**](./4-Computacao-Grafica-3D-e-Pipeline.md)
  * Vetores 3D e Coordenadas Homogêneas 4D ($[x, y, z, w]^T$).
  * Matrizes de Transformação $4\times4$: *Model*, *View* (LookAt) e *Projection* (Perspectiva vs Ortográfica).
  * Pipeline Gráfico em Software (100% CPU): *Vertex Shading* $\to$ *Frustum Clipping* $\to$ *Perspective Divide* $\to$ *Back-face Culling* $\to$ *Barycentric Triangle Rasterization* $\to$ *Z-Buffering*.
  * Modelo de Iluminação de Blinn-Phong: Componentes Ambiente, Difusa Lambertiana e Especular com Expoente de Brilho (*Shininess*).
  * Renderizador Hardware do WPF: `Viewport3D`, `ModelVisual3D`, `MeshGeometry3D`, Câmera Orbital Arcball e geração paramétrica de Toro, Esfera UV, Cubo, Cilindro, Cone e Faixa de Möbius.

* [**Capítulo 5: Modelagem Hierárquica & Cinemática Direta**](./5-Modelagem-Hierarquica-e-Cinematica-Direta.md)
  * Grafos de Cena (*Scene Graph*) e árvores de nós.
  * Acúmulo e propagação de matrizes pai-filho: $M_{global, filho} = M_{global, pai} \times M_{local, filho}$.
  * Design Top-Down vs Construção Bottom-Up.
  * Robô Articulado de 4 Níveis: Base Giratória $\to$ Ombro $\to$ Cotovelo $\to$ Garra Pinça.
  * Cinemática Direta (*Forward Kinematics - FK*) e animação contínua em tempo real.

* [**Capítulo 6: Ray Tracing & Renderização Realística**](./6-Ray-Tracing-e-Renderizacao-Realistica.md)
  * O modelo óptico de Whitted.
  * Resolução analítica de interseção raio-esfera e raio-plano.
  * Raios de Sombra (*Shadow Rays*) e luz pontual com atenuação.
  * Reflexão Especular Recursiva com controle de profundidade (*Bounces*).
  * Refração Dielétrica com Lei de Snell e coeficiente de reflexão de Fresnel.

* [**Capítulo 7: Mapeamento Completo do Plano de Ensino**](./7-Mapeamento-do-Plano-de-Ensino.md)
  * Matriz cruzada de correspondência com todas as unidades curriculares da disciplina universitária.
