---
title: Algoritmos de Rasterização de Linhas (Rasterizer2D.cs)
description: Comparativo aprofundado entre DDA, o Algoritmo de Linha de Bresenham (aritmética 100% inteira) e Anti-Aliasing de Xiaolin Wu.
---

Como uma tela digital formada por uma grade discreta de quadradinhos (pixels) consegue desenhar uma linha contínua perfeita que passa em diagonal?

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) implementa os três algoritmos históricos mais importantes da Computação Gráfica.

---

## 📏 1. Algoritmo DDA (Digital Differential Analyzer)

O **DDA** calcula a variação incremental de $x$ e $y$ ao longo do eixo de maior deslocamento utilizando números reais de ponto flutuante (*floating-point*):

```csharp
public static void DrawLineDDA(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
{
    int dx = x1 - x0;
    int dy = y1 - y0;
    int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

    double xInc = (double)dx / steps;
    double yInc = (double)dy / steps;

    double x = x0;
    double y = y0;

    for (int i = 0; i <= steps; i++)
    {
        bmp.SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
        x += xInc;
        y += yInc;
    }
}
```

### Limitações do DDA:
1. Requer divisões e somas contínuas em `double` / `float`.
2. A função `Math.Round` consome ciclos de clock caros da CPU/GPU.
3. Acúmulo de erro de arredondamento em linhas muito longas.

---

## ⚡ 2. O Algoritmo de Reta de Bresenham (1965)

Criado por Jack Bresenham na IBM em 1965, este algoritmo revolucionou a computação gráfica mundial ao provar que é possível desenhar qualquer reta usando **exclusivamente somas e subtrações com números inteiros**!

### Dedução Matemática da Variável de Decisão de Erro:
A inclinação da reta é $m = \frac{\Delta y}{\Delta x}$. 
A cada incremento horizontal em $x$, o valor ideal contínuo de $y$ cresce por $m$. O algoritmo mantém uma variável acumuladora de erro $e$:

$$
e = 2 \Delta y - \Delta x
$$

- Se $e \ge 0$: O ponto real cruzou a metade do pixel superior! Incrementamos $y$ em $+1$ e subtraímos $2(\Delta y - \Delta x)$ de $e$.
- Se $e < 0$: O ponto real ainda está mais próximo da linha atual. Mantemos $y$ inalterado e somamos $2\Delta y$ a $e$.

```csharp
// Implementação Completa em C# (Bresenham Generalizado para Todos os Octantes):
public static void DrawLineBresenham(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
{
    int dx = Math.Abs(x1 - x0);
    int dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1;
    int sy = y0 < y1 ? 1 : -1;
    int err = (dx > dy ? dx : -dy) / 2;

    while (true)
    {
        bmp.SetPixel(x0, y0, color);
        if (x0 == x1 && y0 == y1) break;

        int e2 = err;
        if (e2 > -dx) { err -= dy; x0 += sx; }
        if (e2 < dy)  { err += dx; y0 += sy; }
    }
}
```

---

## 🖌️ 3. Algoritmo de Linhas Suavizadas de Xiaolin Wu (Anti-Aliasing)

Tanto o DDA quanto o Bresenham desenham linhas com o efeito "escada" (*jagged / aliased*).

Criado por Xiaolin Wu em 1991, este algoritmo calcula a **fração subpixel exata** que a reta corta em dois pixels vizinhos e divide a opacidade (Alpha) proporcionalmente:

```
Linha Real passa em y = 10.3:
------------------------------------------
Pixel (x, 10): Recebe (1.0 - 0.3) = 70% de Opacidade
Pixel (x, 11): Recebe (0.3)       = 30% de Opacidade
------------------------------------------
```

O resultado visual é uma reta perfeitamente suave e contínua aos olhos humanos, eliminando 100% dos serrilhados!

---

👉 **Próximo Passo:** Aprenda sobre [Círculos, Elipses e Curvas de Bézier](/CGPDI.StudyLab/cg2d/circulos-elipses-e-curvas/).
