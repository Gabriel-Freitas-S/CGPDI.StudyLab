---
title: Círculos, Elipses & Curvas de Bézier (Rasterizer2D.cs)
description: Algoritmo do Ponto Médio para Círculos (simetria em 8 octantes), Elipses e Curvas Paramétricas de Bézier com De Casteljau.
---

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) reúne as soluções analíticas e incrementais para rasterização de cônicas e curvas livres.

---

## ⭕ 1. Algoritmo do Círculo do Ponto Médio (Midpoint Circle)

A equação implícita de um círculo centrado na origem é:

$$
f_{\text{círculo}}(x, y) = x^2 + y^2 - R^2
$$

### A Simetria Mágica dos 8 Octantes:
Um círculo possui simetria rotacional e reflexiva completa de 8 vias ($45^\circ$ cada). Por isso, **o algoritmo calcula apenas os pontos do primeiro octante** ($x = 0$ até $x = y$) e plota instantaneamente os **8 pontos simétricos**:

```csharp
void Plot8Points(int xc, int yc, int x, int y, Color c)
{
    bmp.SetPixel(xc + x, yc + y, c);
    bmp.SetPixel(xc - x, yc + y, c);
    bmp.SetPixel(xc + x, yc - y, c);
    bmp.SetPixel(xc - x, yc - y, c);
    bmp.SetPixel(xc + y, yc + x, c);
    bmp.SetPixel(xc - y, yc + x, c);
    bmp.SetPixel(xc + y, yc - x, c);
    bmp.SetPixel(xc - y, yc - x, c);
}
```

### Variável de Decisão $d$:
Avaliamos o ponto médio entre os dois pixels candidatos $(x + 1, \; y)$ e $(x + 1, \; y - 1)$:

$$
d_{\text{inicial}} = 1 - R
$$

- Se $d < 0$: O ponto médio está dentro do círculo. Escolhemos o pixel $(x + 1, y)$ e atualizamos:
$$
d = d + 2x + 3
$$
- Se $d \ge 0$: O ponto médio está fora. Escolhemos o pixel $(x + 1, y - 1)$ e atualizamos:
$$
d = d + 2(x - y) + 5
$$

---

## 🥚 2. Algoritmo da Elipse do Ponto Médio

A elipse tem equação implícita:

$$
f_{\text{elipse}}(x, y) = r_y^2 x^2 + r_x^2 y^2 - r_x^2 r_y^2
$$

Diferente do círculo, a elipse possui simetria em apenas **4 quadrantes**. O algoritmo é dividido em duas regiões baseadas na inclinação da reta tangente:
- **Região 1 ($|\text{declive}| < 1$):** A curva cresce mais em $x$.
- **Região 2 ($|\text{declive}| \ge 1$):** A curva desce mais rápido em $y$.

---

## 〰️ 3. Curvas Paramétricas de Bézier Cúbicas

Desenvolvidas pelo engenheiro francês Pierre Bézier para projetar a carroceria dos carros da Renault, as curvas de Bézier definem curvas suaves e elegantes controladas por 4 pontos:
- $P_0$: Ponto inicial.
- $P_1, P_2$: Pontos de controle que "puxam" a curvatura no espaço.
- $P_3$: Ponto final.

### Polinômio de Bernstein Cúbico ($t \in [0, 1]$):
$$
B(t) = (1 - t)^3 P_0 + 3(1 - t)^2 t P_1 + 3(1 - t) t^2 P_2 + t^3 P_3
$$

```csharp
// Implementação em C#:
public static Point EvaluateBezierCubic(Point p0, Point p1, Point p2, Point p3, double t)
{
    double u = 1.0 - t;
    double tt = t * t;
    double uu = u * u;
    double uuu = uu * u;
    double ttt = tt * t;

    double x = uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X;
    double y = uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y;

    return new Point(x, y);
}
```

---

👉 **Próximo Passo:** Aprenda sobre [Preenchimento Scanline & Recorte Cohen-Sutherland](/CGPDI.StudyLab/cg2d/preenchimento-e-recorte/).
