---
title: Filtros Espaciais & Convoluções 2D (SpatialFilters.cs)
description: A teoria matemática da convolução discreta 2D, matrizes de kernel, filtro Gaussiano com desvio padrão, Unsharp Masking e filtro da Mediana.
---

Diferente das operações pontuais, a **Filtragem Espacial** calcula o novo valor de um pixel $g(x, y)$ considerando a vizinhança de pontos ao seu redor (geralmente uma janela de $3 \times 3$ ou $5 \times 5$ pixels).

O arquivo [`SpatialFilters.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/SpatialFilters.cs) reúne os operadores lineares e de ordenação.

---

## 1. O que é uma Convolução 2D?

### A Analogia da Lupa de Mistura:
Imagine colocar uma pequena lupa de $3 \times 3$ quadradinhos sobre a imagem. A lupa lê a cor dos 9 pixels vizinhos, multiplica cada um pelo peso indicado na matriz (**Kernel**) e soma tudo para descobrir a cor do quadradinho central:

$$
g(x, y) = \sum_{u=-k}^{k} \sum_{v=-k}^{k} f(x - u, \; y - v) \cdot K(u, v)
$$

```
Janela da Imagem:          Kernel K (3x3):
[ p00  p01  p02 ]         [ w00  w01  w02 ]
[ p10  p11  p12 ]    *    [ w10  w11  w12 ]
[ p20  p21  p22 ]         [ w20  w21  w22 ]

Novo Pixel Central = (p00*w00 + p01*w01 + ... + p22*w22) / Divisor + Bias
```

---

## 2. Filtro da Média (Box Blur)

Substitui cada ponto pela média aritmética simples dos seus $3 \times 3 = 9$ vizinhos:

$$
K_{\text{box}} = \frac{1}{9} \begin{bmatrix} 1 & 1 & 1 \\ 1 & 1 & 1 \\ 1 & 1 & 1 \end{bmatrix}
$$

---

## 3. Filtro Gaussiano 2D (Gaussian Blur)

O filtro Gaussiano dá mais importância ao pixel central e diminui o peso suavemente conforme nos afastamos, seguindo a **Curva de Sino Normal 2D**:

$$
G(x, y) = \frac{1}{2\pi \sigma^2} e^{-\frac{x^2 + y^2}{2\sigma^2}}
$$

### Matriz Gaussiana Discreta $3 \times 3$:
$$
K_{\text{gauss}} = \frac{1}{16} \begin{bmatrix} 1 & 2 & 1 \\ 2 & 4 & 2 \\ 1 & 2 & 1 \end{bmatrix}
$$

---

## 4. Máscara de Desfoque & Nitidez (Unsharp Masking)

### Como Deixar uma Imagem Mais Nítida?
1. Desfocamos uma cópia da imagem original com o Filtro Gaussiano.
2. Subtraímos a cópia borrada da imagem original para isolar apenas os contornos finos:
$$
\text{Máscara}(x, y) = f(x, y) - f_{\text{blur}}(x, y)
$$
3. Somamos esses contornos de volta com um fator de ganho $k$:
$$
g(x, y) = f(x, y) + k \cdot \left[ f(x, y) - f_{\text{blur}}(x, y) \right]
$$

---

## 5. Filtro da Mediana (Eliminação de Ruído Sal & Pimenta)

### A Analogia da Votação Democrática:
Quando a imagem tem pontinhos brancos ou pretos aleatórios (**ruído Sal & Pimenta**), a média borra o defeito. O **Filtro da Mediana** faz algo mais inteligente:
1. Pega os 9 números da vizinhança;
2. Organiza em ordem crescente (`Array.Sort`);
3. Escolhe o número que ficou exatamente no meio (o 5º número).

```
Vizinhanca com Ruido Branco (255): [12, 14, 15, 13, 255, 14, 15, 12, 13]
Valores Ordenados:                  [12, 12, 13, 13,  14, 14, 15, 15, 255]
                                                       ^
                                                  Mediana = 14 (Ruido eliminado!)
```

---

**Próximo Passo:** Explore os [Operadores de Gradiente e o Algoritmo Canny de 5 Etapas](/pdi/deteccao-de-bordas-e-canny/).
