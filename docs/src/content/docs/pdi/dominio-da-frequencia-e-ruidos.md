---
title: Domínio da Frequência (DFT 2D) & Texturas Procedurais (FrequencyAndProcedural.cs)
description: Transformada Discreta de Fourier 2D, FFTShift, filtragem em frequência, Ruído de Perlin, terrenos fractais fBm, Voronoi e conjuntos de Mandelbrot e Julia.
---

O arquivo [`FrequencyAndProcedural.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/FrequencyAndProcedural.cs) implementa a matemática avançada de análise espectral no domínio da frequência e geradores matemáticos de mundos procedurais.

---

## 🌊 1. A Transformada Discreta de Fourier 2D (DFT)

A Transformada de Fourier decompõe qualquer imagem digital $f(x, y)$ em uma soma infinita de ondas senoidais e cossenoidais puras com diferentes frequências, amplitudes e orientações espaciais.

### Equação de Análise 2D (Direta):
$$
F(u, v) = \sum_{x=0}^{M-1} \sum_{y=0}^{N-1} f(x, y) \cdot e^{-j 2\pi \left( \frac{ux}{M} + \frac{vy}{N} \right)}
$$

Pela **Fórmula de Euler** ($e^{-j\theta} = \cos\theta - j\sin\theta$):
$$
F(u, v) = R(u, v) + j \cdot I(u, v)
$$

---

## 🌌 2. Espectro de Magnitude e Centralização `FFTShift`

O olho humano não enxerga números complexos. Para visualizar o espectro de frequências:

### Espectro de Magnitude com Compressão Logarítmica:
$$
|F(u, v)| = \sqrt{R(u, v)^2 + I(u, v)^2}
$$
$$
S(u, v) = c \cdot \ln\left(1 + |F(u, v)|\right)
$$

### Centralização com `FFTShift`:
Por padrão, a frequência contínua DC ($u=0, v=0$ - brilho médio da imagem) fica nos 4 cantos da matriz. 
O algoritmo **`FFTShift`** troca os quadrantes diagonais $1 \leftrightarrow 4$ e $2 \leftrightarrow 3$, movendo a frequência zero para o **centro exato da imagem**.

```
[ Quadrante 1 | Quadrante 2 ]       [ Quadrante 4 | Quadrante 3 ]
----------------------------- --->  -----------------------------
[ Quadrante 3 | Quadrante 4 ]       [ Quadrante 2 | Quadrante 1 ]
   (Antes do FFTShift)                   (Centro Espectral)
```

---

## 🌫️ 3. Geração Procedural: Ruído de Perlin & fBm

Criado por Ken Perlin para o filme *Tron* (1982) (o que lhe rendeu um Oscar Técnico), o **Perlin Noise** é um ruído gradiente suave e natural.

### Movimento Browniano Fracionário (fBm - Fractal Brownian Motion):
Soma múltiplas "oitavas" de ruído de Perlin dobrando a frequência e diminuindo a amplitude pela metade:

$$
\text{fBm}(x, y) = \sum_{i=0}^{\text{octaves}-1} \text{amplitude}^i \cdot \text{Perlin}\left(x \cdot \text{frequency}^i, \; y \cdot \text{frequency}^i\right)
$$

- **Aplicações:** Geração de nuvens realistas, mapas de relevo geográfico e texturas de madeira e mármore.

---

## 💎 4. Diagramas de Voronoi (Células Orgânicas)

Distribui $N$ pontos sementes $P_i = (x_i, y_i)$ aleatoriamente no espaço. Para cada pixel $(x, y)$, encontra a semente mais próxima:

$$
d(x, y) = \min_{i=1 \dots N} \sqrt{(x - x_i)^2 + (y - y_i)^2}
$$

- **Aplicações:** Simulação de pele de répteis, tecidos celulares biológicos, vitrais e rachaduras em pedra.

---

## 🌀 5. Fractais de Mandelbrot & Julia

Baseados na iteração de números complexos no plano de Argand-Gauss:

$$
z_{n+1} = z_n^2 + c
$$

- **Conjunto de Mandelbrot:** $z_0 = 0$, variando $c = x + jy$.
- **Conjunto de Julia:** $c$ é fixo (ex: $c = -0.7 + 0.27015j$), variando $z_0 = x + jy$.

Se a magnitude $|z_n| = \sqrt{x_n^2 + y_n^2} > 2$, a órbita escapa para o infinito. O número de iterações até o escape define a cor psicodélica do fractal!

---

👉 **Próximo Passo:** Entre no módulo de [Computação Gráfica 2D & Álgebra Linear](/CGPDI.StudyLab/cg2d/algebra-linear-e-matrizes/).
