---
title: Círculos, Elipses & Curvas de Bézier (Rasterizer2D.cs)
description: Algoritmo do Ponto Médio para Círculos (simetria em 8 octantes), Elipses e Curvas Paramétricas de Bézier com De Casteljau.
---

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) reúne os métodos analíticos e incrementais para rasterização de cônicas e curvas livres.

---

## 1. Algoritmo do Círculo do Ponto Médio (Midpoint Circle)

O círculo possui simetria perfeita em 8 setores ($45^\circ$ cada). Por essa razão, **o algoritmo calcula os pontos apenas para 1/8 do círculo** e plota instantaneamente os **8 pontos simétricos**:

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

A variável de decisão de erro começa com $d = 1 - R$ e é atualizada apenas com adições de números inteiros a cada passo.

---

## 2. Curvas Paramétricas de Bézier Cúbicas

Controladas por 4 pontos: ponto inicial $P_0$, pontos de controle $P_1$ e $P_2$, e ponto final $P_3$.

### Polinômio de Bernstein Cúbico ($t \in [0, 1]$):
$$
B(t) = (1 - t)^3 P_0 + 3(1 - t)^2 t P_1 + 3(1 - t) t^2 P_2 + t^3 P_3
$$

---

👉 **Próximo Passo:** Aprenda sobre [Preenchimento Scanline & Recorte Cohen-Sutherland](/cg2d/preenchimento-e-recorte/).
