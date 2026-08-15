# ✏️ Capítulo 3: Computação Gráfica 2D & Rasterização

O módulo [`Graphics2D/Rasterizer2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Rasterizer2D.cs) e [`Graphics2D/Matrix2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Matrix2D.cs) implementam a base matemática de conversão de primitivas vetoriais em grades discretas de pixels.

---

## 1. Álgebra Linear 2D & Coordenadas Homogêneas

Em computação gráfica, um ponto 2D $(x, y)$ é expresso em coordenadas projetivas homogêneas como um vetor coluna $[x, y, 1]^T$. Isso permite que translações, rotações, escalas e cisalhamentos sejam expressos como multiplicações de **matrizes $3\times3$**:

$$M_{\text{translação}}(\Delta x, \Delta y) = \begin{bmatrix} 1 & 0 & \Delta x \\ 0 & 1 & \Delta y \\ 0 & 0 & 1 \end{bmatrix}, \quad M_{\text{rotação}}(\theta) = \begin{bmatrix} \cos\theta & -\sin\theta & 0 \\ \sin\theta & \cos\theta & 0 \\ 0 & 0 & 1 \end{bmatrix}$$

$$M_{\text{escala}}(s_x, s_y) = \begin{bmatrix} s_x & 0 & 0 \\ 0 & s_y & 0 \\ 0 & 0 & 1 \end{bmatrix}, \quad M_{\text{shear}}(k_x, k_y) = \begin{bmatrix} 1 & k_x & 0 \\ k_y & 1 & 0 \\ 0 & 0 & 1 \end{bmatrix}$$

### Composição Matricial em Torno de um Pivô $(c_x, c_y)$:
$$M_{\text{composta}} = T(c_x, c_y) \times R(\theta) \times T(-c_x, -c_y)$$

---

## 2. Rasterização de Segmentos de Reta

### 2.1 Algoritmo de Bresenham (1965)
Utiliza **aritmética 100% inteira** através de uma variável de decisão de erro incremental, eliminando divisões e pontos flutuantes:

```csharp
public static void DrawLineBresenham(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
{
    int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
    int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
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

### 2.2 Algoritmo de Xiaolin Wu (Anti-Aliased)
Calcula a distância fracionária da linha ideal ao pixel e desenha pares de pixels verticais/horizontais com transparência alfa proporcional:
$$c_1 = \text{cor} \times (1 - \text{frac}(y)), \quad c_2 = \text{cor} \times \text{frac}(y)$$

---

## 3. Cônicas: Círculo e Elipse do Ponto Médio

### 3.1 Círculo do Ponto Médio (Simetria em 8 Octantes)
Calcula apenas o arco de $45^\circ$ ($0 \le x \le y$) e plota os 8 pontos simétricos:
$$\{( \pm x, \pm y), (\pm y, \pm x)\}$$
* Variável de decisão inicial: $d = 1 - r$.
* Se $d < 0$: $d = d + 2x + 3$.
* Se $d \ge 0$: $y = y - 1$; $d = d + 2(x - y) + 5$.

### 3.2 Elipse do Ponto Médio (Duas Regiões)
* **Região 1** ($\left|\frac{dy}{dx}\right| < 1$): Avança em $x$, decide $y$.
* **Região 2** ($\left|\frac{dy}{dx}\right| \ge 1$): Avança em $y$, decide $x$.
* Plota os 4 quadrantes simétricos $\{(\pm x, \pm y)\}$.

---

## 4. Curvas Paramétricas de Bézier

### 4.1 Curva de Bézier Cúbica
Definida por 4 pontos de controle $P_0, P_1, P_2, P_3$ através dos Polinômios de Bernstein de grau 3:

$$B(t) = (1-t)^3 P_0 + 3(1-t)^2 t P_1 + 3(1-t) t^2 P_2 + t^3 P_3, \quad t \in [0, 1]$$

---

## 5. Preenchimento de Polígonos (Scanline Fill)

1. **Construção da Tabela de Arestas (ET - Edge Table):** Arestas são ordenadas pelo $y_{\min}$.
2. **Tabela de Arestas Ativas (AET):** Conforme a linha de varredura $y$ avança:
   * Insere arestas cujo $y_{\min} = y$.
   * Remove arestas cujo $y_{\max} = y$.
   * Atualiza a interseção $x = x + 1/m$.
   * Ordena os nós por $x$ crescente e preenche os segmentos horizontais entre pares pares/ímpares de nós $[x_{2i}, x_{2i+1}]$.

---

## 6. Recorte de Linhas de Cohen-Sutherland

Classifica os vértices com códigos binários de 4 bits (*Outcodes*):
$$\text{Bit 3: Top } (y > y_{\max}), \quad \text{Bit 2: Bottom } (y < y_{\min})$$
$$\text{Bit 1: Right } (x > x_{\max}), \quad \text{Bit 0: Left } (x < x_{\min})$$

* **Aceitação Trivial:** `code0 | code1 == 0` (Ambos os pontos estão dentro da janela de visualização).
* **Rejeição Trivial:** `code0 & code1 != 0` (Ambos os pontos compartilham o mesmo lado externo).
* **Interseção:** Calcula analiticamente o ponto de corte na borda da janela e repete o teste.
