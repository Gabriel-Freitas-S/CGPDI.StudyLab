---
title: Filtros Espaciais & Convoluções 2D (SpatialFilters.cs)
description: A teoria matemática da convolução discreta 2D, matrizes de kernel, filtro Gaussiano com desvio padrão, Unsharp Masking e filtro da Mediana.
---

Diferente das operações pontuais, a **Filtragem Espacial** calcula o novo valor de um pixel $g(x, y)$ levando em consideração uma vizinhança de pixels ao seu redor (geralmente uma janela de tamanho $3 \times 3$, $5 \times 5$ ou $7 \times 7$).

O arquivo [`SpatialFilters.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/SpatialFilters.cs) reúne os principais operadores lineares e não-lineares.

---

## 🧮 1. A Equação da Convolução Discreta 2D

A convolução espacial discreta consiste em deslizar uma pequena matriz de pesos chamada **Kernel (ou Máscara)** $K$ sobre cada pixel $(x, y)$ da imagem $f$:

$$
g(x, y) = \sum_{u=-k}^{k} \sum_{v=-k}^{k} f(x - u, \; y - v) \cdot K(u, v)
$$

Onde $k$ é o raio do kernel (para um kernel $3 \times 3$, $k = 1$; para $5 \times 5$, $k = 2$).

```
Janela da Imagem:          Kernel K (3x3):
[ p00  p01  p02 ]         [ w00  w01  w02 ]
[ p10  p11  p12 ]    *    [ w10  w11  w12 ]
[ p20  p21  p22 ]         [ w20  w21  w22 ]

Novo Pixel Central = (p00*w00 + p01*w01 + ... + p22*w22) / Divisor + Bias
```

---

## 📦 2. Filtro da Média (Box Blur)

O filtro da média substitui cada pixel pela média aritmética simples de seus $3 \times 3 = 9$ vizinhos:

$$
K_{\text{box}} = \frac{1}{9} \begin{bmatrix} 1 & 1 & 1 \\ 1 & 1 & 1 \\ 1 & 1 & 1 \end{bmatrix}
$$

- **Efeito:** Suaviza ruído de alta frequência, mas borra arestas e detalhes nítidos.

---

## 🔔 3. Filtro Gaussiano 2D (Gaussian Blur)

O filtro Gaussiano pondera os vizinhos usando a **Distribuição Normal / Curva de Sino 2D**, dando peso máximo ao pixel central e diminuindo suavemente com a distância euclidiana:

$$
G(x, y) = \frac{1}{2\pi \sigma^2} e^{-\frac{x^2 + y^2}{2\sigma^2}}
$$

Onde $\sigma$ (sigma) é o desvio padrão da curva que controla a intensidade do desfoque.

### Matriz Gaussiana Discreta $3 \times 3$ ($\sigma \approx 1.0$):

$$
K_{\text{gauss}} = \frac{1}{16} \begin{bmatrix} 1 & 2 & 1 \\ 2 & 4 & 2 \\ 1 & 2 & 1 \end{bmatrix}
$$

### Matriz Gaussiana $5 \times 5$:
$$
K_{\text{gauss5}} = \frac{1}{273} \begin{bmatrix} 
1 & 4 & 7 & 4 & 1 \\ 
4 & 16 & 26 & 16 & 4 \\ 
7 & 26 & 41 & 26 & 7 \\ 
4 & 16 & 26 & 16 & 4 \\ 
1 & 4 & 7 & 4 & 1 
\end{bmatrix}
$$

---

## 🗡️ 4. Máscara de Desfoque & Nitidez (Unsharp Masking)

O **Unsharp Masking** é o algoritmo clássico usado pelo Adobe Photoshop e câmeras digitais para aumentar a nitidez aparente de contornos.

### As 3 Etapas do Algoritmo:
1. Cria uma versão desfocada da imagem original $f_{\text{blur}}$ via Filtro Gaussiano.
2. Extrai a "máscara de detalhes de alta frequência":
$$
\text{Máscara}(x, y) = f(x, y) - f_{\text{blur}}(x, y)
$$
3. Soma a máscara de volta à imagem original multiplicada por um fator de ganho (*Amount* $k$):
$$
g(x, y) = f(x, y) + k \cdot \left[ f(x, y) - f_{\text{blur}}(x, y) \right]
$$

---

## 🧂 5. Filtro da Mediana (Filtro Não-Linear para Ruído Sal & Pimenta)

Filtros lineares de convolução (como a Média e o Gaussiano) falham miseravelmente ao tentar remover o ruído de impulsos extremos (**Sal & Pimenta** - pixels aleatórios totalmente pretos ou brancos), pois eles apenas espalham o ponto preto/branco na vizinhança.

O **Filtro da Mediana** é uma operação estatística de ordenação:
1. Coleta os 9 valores da vizinhança $3 \times 3$ em um vetor.
2. **Ordena o vetor** do menor para o maior (`Array.Sort`).
3. Substitui o pixel central pelo valor que ficou **exatamente no meio** (índice 4 do vetor ordenado).

```
Vizinhança com Ruído Sal (255): [12, 14, 15, 13, 255, 14, 15, 12, 13]
Valores Ordenados:               [12, 12, 13, 13,  14, 14, 15, 15, 255]
                                                    ^
                                               Mediana = 14 (Ruído 255 eliminado!)
```

:::note[Vantagem Exclusiva]
O filtro da mediana elimina $100\%$ do ruído Sal & Pimenta preservando as bordas e contornos das imagens sem criar borramento difuso!
:::

---

👉 **Próximo Passo:** Explore os [Operadores de Gradiente e o Algoritmo Canny de 5 Etapas](/CGPDI.StudyLab/pdi/deteccao-de-bordas-e-canny/).
