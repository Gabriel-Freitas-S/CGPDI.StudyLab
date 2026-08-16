---
title: Domínio da Frequência (DFT 2D) & Texturas Procedurais (FrequencyAndProcedural.cs)
description: Transformada Discreta de Fourier 2D, FFTShift, filtragem em frequência, Ruído de Perlin, terrenos fractais fBm, Voronoi e conjuntos de Mandelbrot e Julia.
---

O arquivo [`FrequencyAndProcedural.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/FrequencyAndProcedural.cs) implementa a análise espectral de Fourier e algoritmos matemáticos de geração procedural.

---

## 1. A Transformada Discreta de Fourier 2D (DFT)

### A Analogia da Partitura Musical:
Imagine uma música de orquestra: ela é uma mistura de vários instrumentos tocando ao mesmo tempo (flautas com notas agudas, violoncelos com notas graves). A Transformada de Fourier é como um maestro que escuta a música inteira e **escreve a partitura exata de cada frequência separadamente**.

Em uma imagem digital, as variações de intensidade são decompostas em ondas senoidais e cossenoidais puras:

$$
F(u, v) = \sum_{x=0}^{M-1} \sum_{y=0}^{N-1} f(x, y) \cdot e^{-j 2\pi \left( \frac{ux}{M} + \frac{vy}{N} \right)}
$$

---

## 2. Espectro de Magnitude e `FFTShift`

O espectro de frequências é visualizado calculando a magnitude com compressão logarítmica:

$$
|F(u, v)| = \sqrt{R(u, v)^2 + I(u, v)^2}
$$
$$
S(u, v) = c \cdot \ln\left(1 + |F(u, v)|\right)
$$

O algoritmo **`FFTShift`** troca os quadrantes diagonais para posicionar a frequência contínua zero (o brilho médio) no **centro exato da imagem**.

---

## 3. Ruído de Perlin & Terrenos Fractais (fBm)

Criado por Ken Perlin para efeitos cinematográficos, o **Ruído de Perlin** gera variações suaves e orgânicas.

Ao somar várias camadas (*oitavas*) de ruído com o método de **Movimento Browniano Fracionário (fBm)**, o sistema produz mapas de montanhas, relevo geográfico e nuvens realistas:

$$
\text{fBm}(x, y) = \sum_{i=0}^{\text{octaves}-1} \text{amplitude}^i \cdot \text{Perlin}\left(x \cdot \text{frequency}^i, \; y \cdot \text{frequency}^i\right)
$$

---

## 4. Diagramas de Voronoi e Fractais

- **Diagramas de Voronoi:** Divide o espaço em células baseadas na distância para a semente mais próxima (usado para peles de animais e pedras).
- **Fractais de Mandelbrot e Julia:** Gerados pela iteração matemática de números complexos $z_{n+1} = z_n^2 + c$.

---

👉 **Próximo Passo:** Entre no módulo de [Computação Gráfica 2D & Álgebra Linear](/cg2d/algebra-linear-e-matrizes/).
