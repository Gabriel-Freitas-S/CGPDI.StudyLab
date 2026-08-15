---
title: Transformações Geométricas & Interpolação (GeometricTransforms.cs)
description: Mapeamento Direto vs Inverso, interpolações Vizinho Mais Próximo, Bilinear e Bicúbica, rotações e deformações não-lineares (Swirl, Ripple, Fisheye).
---

As **Transformações Geométricas** alteram a relação espacial entre os pixels, permitindo rotacionar, redimensionar, esticar e distorcer imagens.

O arquivo [`GeometricTransforms.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/GeometricTransforms.cs) implementa algoritmos de deformação linear e não-linear.

---

## 🕳️ 1. Mapeamento Direto vs Mapeamento Inverso

Por que não podemos simplesmente aplicar a fórmula de rotação para frente em cada pixel original $(x, y) \to (x', y')$?

```
❌ Mapeamento Direto (Forward Mapping):
Pixel Origem (x, y) ----> Posição Destino (x', y') [Gera "buracos" pretos e sobreposições!]

✅ Mapeamento Inverso (Inverse Mapping):
Para cada pixel da tela destino (x', y') ----> Pergunta: De onde você veio em (x, y)?
```

Com o **Mapeamento Inverso**, garantimos que **100% dos pixels da imagem destino serão preenchidos**, calculando a transformação inversa $T^{-1}(x', y')$.

---

## 🧮 2. Métodos de Interpolação Espacial

Como as coordenadas calculadas pela transformação inversa $T^{-1}(x', y')$ geralmente caem em números fracionários (ex: $x = 142.37, y = 89.64$), precisamos estimar a cor naquele ponto contínuo.

### 1. Vizinho Mais Próximo (Nearest Neighbor)
Arredonda as coordenadas para o inteiro mais próximo:
$$
x_{\text{int}} = \text{round}(x), \quad y_{\text{int}} = \text{round}(y)
$$
- **Prós:** Extremamente rápido ($O(1)$).
- **Contras:** Gera efeito serrilhado (*pixelado*).

---

### 2. Interpolação Bilinear (4 Vizinhos)
Interpola linearmente entre os 4 pixels mais próximos: $(x_0, y_0)$, $(x_1, y_0)$, $(x_0, y_1)$ e $(x_1, y_1)$.

Seja $dx = x - x_0$ e $dy = y - y_0$ (frações entre $0.0$ e $1.0$):

$$
\begin{aligned}
f(x, y) = & (1 - dx)(1 - dy) \cdot f(x_0, y_0) + \\
          & dx(1 - dy) \cdot f(x_1, y_0) + \\
          & (1 - dx)dy \cdot f(x_0, y_1) + \\
          & dx \cdot dy \cdot f(x_1, y_1)
\end{aligned}
$$

- **Resultado:** Transições suaves sem serrilhamento grosseiro.

---

### 3. Interpolação Bicúbica (16 Vizinhos com Spline Cúbica)
Utiliza uma vizinhança de $4 \times 4 = 16$ pixels e a função polinomial cúbica de Mitchell-Netravali / Catmull-Rom para preservar contornos nítidos e curvas suaves:

$$
W(t) = \begin{cases}
(a+2)|t|^3 - (a+3)|t|^2 + 1, & \text{para } |t| \le 1 \\
a|t|^3 - 5a|t|^2 + 8a|t| - 4a, & \text{para } 1 < |t| < 2 \\
0, & \text{caso contrário}
\end{cases}
$$
Com $a = -0.5$.

---

## 🌀 3. Deformações Espaciais Não-Lineares (Warps)

### 1. Efeito Redemoinho / Turbilhão (Swirl Effect)
Rotaciona a imagem com um ângulo que decresce quadraticamente com a distância do centro $(x_c, y_c)$:

$$
r = \sqrt{(x - x_c)^2 + (y - y_c)^2}
$$
$$
\theta_{\text{novo}} = \theta_{\text{atual}} + \text{Strength} \times \left(1 - \frac{r}{R_{\max}}\right)^2
$$

---

### 2. Efeito de Onda / Ondulação (Ripple Effect)
Simula a superfície da água jogando uma pedra:

$$
x_{\text{orig}} = x + A_x \cdot \sin\left(\frac{2\pi y}{\lambda_y}\right)
$$
$$
y_{\text{orig}} = y + A_y \cdot \cos\left(\frac{2\pi x}{\lambda_x}\right)
$$

---

### 3. Olho de Peixe (Fisheye Lens Distortion)
Simula lentes esféricas convexas de grande-angular:

$$
r_{\text{polar}} = \frac{r}{R_{\max}}, \quad r_{\text{distorcido}} = r_{\text{polar}}^k
$$

---

👉 **Próximo Passo:** Explore a [Transformada de Fourier 2D & Geração Procedural](/CGPDI.StudyLab/pdi/dominio-da-frequencia-e-ruidos/).
