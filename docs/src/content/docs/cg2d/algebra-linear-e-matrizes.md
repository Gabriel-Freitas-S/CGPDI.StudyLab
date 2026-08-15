---
title: Álgebra Linear 2D & Coordenadas Homogêneas (Matrix2D.cs)
description: Por que usamos matrizes 3x3 no plano 2D, coordenadas homogêneas, translação, rotação, escala, cisalhamento e composição afim.
---

A base matemática de toda a Computação Gráfica é a **Álgebra Linear Matricial**.

O arquivo [`Matrix2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Matrix2D.cs) encapsula as matrizes de transformação afim no plano bidimensional.

---

## 1. Por que Usamos Matrizes 3x3 no Espaço 2D?

No plano bidimensional, rotação e escala podem ser calculadas por matrizes $2 \times 2$:

$$
\begin{bmatrix} x' \\ y' \end{bmatrix} = \begin{bmatrix} \cos\theta & -\sin\theta \\ \sin\theta & \cos\theta \end{bmatrix} \begin{bmatrix} x \\ y \end{bmatrix}
$$

Entretanto, mover um ponto de lugar (**Translação**) exige uma soma $(x + t_x, \; y + t_y)$, e somas não podem ser multiplicadas diretamente com matrizes $2 \times 2$.

### A Solução: Coordenadas Homogêneas ($x, y, 1$)
Ao adicionar uma dimensão auxiliar $w = 1$, conseguimos unificar todas as operações (translação, rotação, escala) em **matrizes $3 \times 3$**:

$$
\begin{bmatrix} x' \\ y' \\ 1 \end{bmatrix} = 
\begin{bmatrix} 
m_{00} & m_{01} & m_{02} \\ 
m_{10} & m_{11} & m_{12} \\ 
0 & 0 & 1 
\end{bmatrix} 
\begin{bmatrix} x \\ y \\ 1 \end{bmatrix}
$$

---

## 2. Matrizes Elementares 2D

- **Translação:** Move o objeto no plano por $(t_x, t_y)$.
- **Rotação:** Gira o objeto por um ângulo $\theta$.
- **Escala:** Aumenta ou diminui o tamanho do objeto por $(s_x, s_y)$.
- **Cisalhamento (Shear):** Inclina e deforma a geometria lateralmente.

---

## 3. Rotação ao Redor de um Ponto Arbitrário

Para girar um desenho ao redor do seu próprio centro $(P_x, P_y)$:

```mermaid
graph LR
    A[1. Translacao para a Origem: T de -Px, -Py] --> B[2. Rotacao Angular: R de theta]
    B --> C[3. Translacao de Volta: T de +Px, +Py]
```

$$
M_{\text{final}} = T(P_x, P_y) \times R(\theta) \times T(-P_x, -P_y)
$$

:::caution[Ordem de Multiplicação]
A multiplicação de matrizes **não é comutativa** ($A \times B \neq B \times A$). A ordem das operações afeta diretamente o resultado visual.
:::

---

👉 **Próximo Passo:** Aprenda sobre os [Algoritmos de Traçado de Retas (Bresenham e Wu)](/CGPDI.StudyLab/cg2d/algoritmos-de-linhas/).
