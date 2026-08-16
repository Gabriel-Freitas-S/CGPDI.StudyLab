---
title: Roteiro de Estudos para Avaliações (T1, T2 e T3)
description: Guia de preparação acadêmica para os trabalhos computacionais práticos das Unidades 1, 2 e 3 do curso universitário.
---

Este roteiro organiza os tópicos práticos e teóricos necessários para a preparação dos três Trabalhos Computacionais do curso.

---

## 1. Trabalho T1 — Unidade 1: Introdução, Cor & PDI

### Conteúdos de Estudo:
- Pipeline gráfico básico e manipulação de buffers de memória.
- Conversão de espaços de cor (RGB para HSV, YCbCr, Escala de Cinza ITU-R BT.709).
- Histograma de intensidade, Equalização por CDF e Normalização Min-Max.
- Convolução espacial 2D (Gaussiano, Média, Mediana e Sobel).
- Binarização de Otsu e Morfologia Matemática (Erosão e Dilatação).

### Prática no CGPDI.StudyLab:
1. Abra a aba **Processamento Digital de Imagens (PDI)**.
2. Carregue uma imagem de baixo contraste e aplique a **Equalização de Histograma** observando o histograma antes e depois.
3. Adicione ruído "Sal & Pimenta" e compare visualmente a filtragem por **Média** versus **Mediana**.
4. Execute o **Detector Canny em 5 Etapas** e varie os limiares $T_{\text{baixo}}$ e $T_{\text{alto}}$ para analisar a conexão de bordas por histerese.

---

## 2. Trabalho T2 — Unidade 2: Geometria 3D, Malhas & Câmeras

### Conteúdos de Estudo:
- Álgebra Linear 3D: Produto Escalar, Produto Vetorial e Matrizes de Rotação e Translação.
- Construção de malhas triangulares (`Mesh3D` / `MeshGeometry3D`) e cálculo de normais por vértice.
- O Pipeline MVP: Objeto $\to$ Mundo $\to$ Câmera (LookAt) $\to$ Projeção (Perspectiva com FOV) $\to$ NDC $\to$ Viewport.
- O algoritmo de profundidade **Z-Buffer** e descarte de faces ocultas (*Back-face Culling*).
- Representação de iluminação (Luz Ambiente, Difusa e Especular de Phong).

### Prática no CGPDI.StudyLab:
1. Abra a aba **Computação Gráfica 3D (DirectX)** e teste a **Câmera Orbital Arcball** girando os modelos 3D com o mouse.
2. Alterne entre **Projeção Perspectiva** e **Projeção Ortográfica**.
3. Abra a aba **Software 3D & Ray Tracing** e execute o renderizador CPU puro. Alterne entre **Wireframe**, **Flat Shading** e **Gouraud Shading**.

---

## 3. Trabalho T3 — Unidade 3: Modelagem Hierárquica & Cinemática

### Conteúdos de Estudo:
- Motivação para modelagem hierárquica e Grafos de Cena (*Scene Graph*).
- Design Top-Down e Construção Bottom-Up de primitivas geométricas.
- Propagação de matrizes pai-filho: $M_{\text{filho}} = M_{\text{pai}} \times M_{\text{local}}$.
- Cinemática Direta (*Forward Kinematics*) aplicada a robôs articulados.
- Animação em tempo real e reusabilidade de componentes gráficos.

### Prática no CGPDI.StudyLab:
1. Abra a aba **Computação Gráfica 3D** $\to$ **Modelagem Hierárquica**.
2. Carregue o **Braço Robótico Articulado** e mova individualmente os sliders de **Base**, **Ombro**, **Cotovelo** e **Pulso**.
3. Observe como a rotação da base move todos os membros seguintes juntos.
4. Teste a simulação do **Sistema Solar** para ver a translação simultânea do Sol, Terra e Lua.

---

**Próximo Passo:** Aprenda a publicar esta documentação no [GitHub Pages & Configurar o CI/CD](/deploy/github-pages-e-ci-cd/).
