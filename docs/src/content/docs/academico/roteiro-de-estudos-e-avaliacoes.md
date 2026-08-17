---
title: Roteiro Pedagógico de Estudos e Atividades Práticas Aplicadas
description: Guia integrado de estudos teóricos e atividades práticas aplicadas para as Unidades Curriculares de Computação Gráfica e PDI.
---

Este roteiro organiza os conteúdos teóricos e as atividades práticas aplicadas disponíveis na **Central de Estudos (Aba 5)**, no **Laboratório Interativo (Aba 6)** e no **Estúdio de Projetos (Aba 7)**.

---

## 1. Unidade 1: Fundamentos de Memória, Cor & Processamento de Imagens

### Conteúdo de Estudo & Teoria
- **Estrutura de Memória & DirectBitmap**: Formato BGRA32, ponteiros não gerenciados (`unsafe`), cálculo de `Stride` e ciclo de vida do `WriteableBitmap`.
- **Espaços de Cores & Percepção**: Conversões RGB para HSV, YCbCr e Escala de Cinza Perceptual (ITU-R BT.709).
- **Operações Pontuais e Histogramas**: Equalização de histograma por CDF acumulada e normalização Min-Max.
- **Filtros Espaciais e Convoluções**: Filtro da Média, Gaussiano, Mediana e operadores gradientes (Sobel e Prewitt).
- **Segmentação e Morfologia**: Limiarização de Otsu e operações morfológicas de Erosão e Dilatação.

### Atividades Práticas Aplicadas
1. **Calibração de Memória no Laboratório:** Acesse a **Lição 01** e a **Lição 03** para manipular o array BGRA e observar o deslocamento linear na memória RAM em tempo real.
2. **Filtragem e Limiarização:** Acesse a aba **Processamento Digital de Imagens (PDI)**, aplique o **Detector Canny em 5 Etapas** e ajuste os limiares de histerese para analisar a conectividade de bordas.
3. **Equalização de Contraste:** Carregue uma imagem com distribuição concentrada de tons e execute a **Equalização de Histograma** comparando os gráficos antes e depois do processamento.

---

## 2. Unidade 2: Computação Gráfica 2D, Malhas Triangulares 3D & Iluminação

### Conteúdo de Estudo & Teoria
- **Pipeline Gráfico 2D & Coordenadas Homogêneas:** Matrizes afins $3\times3$, translação, rotação em torno de ponto pivô arbitrário $P(x_0,y_0)$ e instanciação com `ControlTemplate`.
- **Rasterização dos Primeiros Princípios:** Reta de Bresenham com inteiros puros, suavização de Xiaolin Wu e Círculo do Ponto Médio.
- **Malhas Triangulares 3D & Câmeras:** Estrutura de vértices e triângulos no `MeshGeometry3D`, ordenação anti-horária (CCW) e projeção perspectiva com `PerspectiveCamera`.
- **Modelos de Iluminação & Texturas:** Lei de Reflexão Difusa de Lambert ($I = k_d \cdot \max(0, N \cdot L)$), sombreamento de Gouraud e repetição de texturas com `TileMode="Tile"`.

### Atividades Práticas Aplicadas
1. **Sistema Mecânico Articulado 2D:**
   - Acesse a **Lição 13** do Laboratório e o template **Veículo Articulado 2D** no Estúdio de Projetos.
   - Projete um veículo composto por chassi e 4 rodas formadas por templates de ponteiros angulares pivotados, com translação sincronizada e inversão contínua via `AutoReverse="True"`.
2. **Cena Arquitetônica 3D com Sistema Solar Duplo:**
   - Acesse a **Lição 14** e o template correspondente no Estúdio de Projetos.
   - Construa uma estrutura tridimensional multifacetada sobre piso texturizado com repetição, posicionando duas fontes de luz direcional a 30° de inclinação em rotação oposta e uma câmera orbital em arco de 180°.

---

## 3. Unidade 3: Modelagem Hierárquica, Grafos de Cena & Ray Tracing

### Conteúdo de Estudo & Teoria
- **Grafos de Cena & Design Hierárquico:** Metodologia Top-Down e Bottom-Up, separação entre componentes primitivos e agrupadores (`Model3DGroup`).
- **Transformações de Instância vs Transformações de Junta:** Posicionamento estático no sistema de coordenadas pai versus rotação dinâmica parametrizada no tempo.
- **Cinemática Harmônica de Marcha:** Defasagem angular periódica ($\theta(t) = A \cdot \sin(\omega t + \phi)$) para locomoção fluida de múltiplos membros articulados.
- **Fundamentos de Ray Tracing:** Equação paramétrica do raio $r(t) = O + t D$, interseção analítica quadrática com esferas, reflexão especular e refração pela Lei de Snell.

### Atividades Práticas Aplicadas
1. **Quadrúpede Articulado 3D e Caravana em Marcha:**
   - Acesse a **Lição 15** do Laboratório e o template **Modelo Hierárquico de Quadrúpede** no Estúdio de Projetos.
   - Instancie um animal tridimensional completo com 14 componentes primitivos coloridos e 9 juntas animadas, aplicando funções senoidais defasadas em $\pi$ e $\pi/2$ para simular a marcha coordenada.
2. **Exportação Autônoma para o Visual Studio 2022:**
   - No Estúdio de Projetos, clique em **Exportar Solução VS2022 (.ZIP)** para gerar o projeto completo (`.sln`, `.csproj`, XAML e C#) pronto para ser compilado e executado fora do aplicativo.

---

<div class="ms-ref-card">
  <h4>Documentação e Recursos Oficiais</h4>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview" target="_blank" rel="noopener">Visão Geral de Transformações no WPF</a> — Classes RotateTransform, TranslateTransform e MatrixTransform.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview" target="_blank" rel="noopener">Visão Geral de Gráficos 3D no WPF</a> — MeshGeometry3D, PerspectiveCamera e Model3DGroup.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.tilebrush.tilemode" target="_blank" rel="noopener">Classe TileBrush e Propriedade TileMode</a> — Mapeamento e repetição de texturas.</li>
  </ul>
</div>

---

**Próximo Passo:** Consulte o [Mapeamento do Plano de Ensino](/academico/mapeamento-do-plano/) para detalhes curriculares por unidade.
