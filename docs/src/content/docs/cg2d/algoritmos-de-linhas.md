---
title: Algoritmos de Rasterização de Linhas (Rasterizer2D.cs)
description: Comparativo aprofundado entre DDA, o Algoritmo de Linha de Bresenham (aritmética 100% inteira) e Anti-Aliasing de Xiaolin Wu.
---

Como uma tela digital formada por uma grade discreta de quadradinhos consegue desenhar uma linha diagonal suave e contínua?

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) reúne os principais métodos de traçado de retas.

---

## 1. Algoritmo DDA (Digital Differential Analyzer)

O **DDA** calcula passos fracionários ao longo do eixo de maior deslocamento utilizando números reais de ponto flutuante (*float* ou *double*):

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

---

## 2. O Algoritmo de Reta de Bresenham (1965)

Criado por Jack Bresenham na IBM, este método calcula retas perfeitas utilizando **exclusivamente somas e subtrações com números inteiros**, sem nenhuma divisão ou ponto flutuante:

$$
e = 2 \Delta y - \Delta x
$$

- Se $e \ge 0$: Incrementa $y$ e atualiza $e = e + 2(\Delta y - \Delta x)$.
- Se $e < 0$: Mantém $y$ e atualiza $e = e + 2\Delta y$.

```csharp
// Implementacao em C# de Bresenham:
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

## 3. Algoritmo de Linhas Suavizadas de Xiaolin Wu (Anti-Aliasing)

O algoritmo de Xiaolin Wu elimina o aspecto serrilhado (*pixelado*) calculando a fração exata de cobertura subpixel da reta entre dois pixels vizinhos e ajustando a transparência (Alpha) proporcionalmente. O resultado visual é uma linha suave e nítida.

---

👉 **Próximo Passo:** Aprenda sobre [Círculos, Elipses e Curvas de Bézier](/cg2d/circulos-elipses-e-curvas/).
