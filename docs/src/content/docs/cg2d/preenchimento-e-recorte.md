---
title: Preenchimento de Polígonos & Recorte (Rasterizer2D.cs)
description: Algoritmo de Preenchimento por Varredura (Scanline com AET), Flood Fill por fila e Recorte de Linhas de Cohen-Sutherland com Outcodes.
---

O arquivo [`Rasterizer2D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics2D/Rasterizer2D.cs) implementa o preenchimento de polígonos côncavos e complexos e o recorte analítico contra a janela de visualização.

---

## 🎨 1. Preenchimento por Varredura (Scanline Polygon Fill)

Como preencher o interior de um polígono arbitrário de $N$ vértices (incluindo formas côncavas e polígonos em estrela) linha por linha?

```
Linha de Varredura y:
  Fora       Dentro             Fora
---------[ x_intersecao1 ======= x_intersecao2 ]---------
```

### Estrutura de Dados do Algoritmo:
1. **Tabela de Arestas Globais (ET - Edge Table):** Guarda todas as arestas do polígono ordenadas pelo $y_{\min}$.
2. **Tabela de Arestas Ativas (AET - Active Edge Table):** Mantém apenas as arestas que cruzam a linha de varredura atual $y$.
3. **Regra de Paridade (Par-Ímpar):** Ordena as interseções em $x$ crescentemente:
   - Do ponto $x_0$ ao $x_1$: Pinta os pixels (dentro).
   - Do ponto $x_1$ ao $x_2$: Pula os pixels (fora).
   - Do ponto $x_2$ ao $x_3$: Pinta os pixels (dentro).

---

## 🌊 2. Algoritmo de Inundação (Flood Fill baseado em Fila)

O **Flood Fill** (a famosa ferramenta de "balde de tinta" do Paint) substitui uma cor inicial por uma nova cor em todas as direções conectadas.

:::danger[Por que NÃO usar recursão simples?]
Uma chamada recursiva ingênua para imagens de $512 \times 512$ atinge mais de $200.000$ níveis de profundidade, causando o infame erro de **StackOverflowException** (estouro de memória da pilha de chamadas).
:::

### A Solução Profissional: Fila Explícita (`Queue<Point>`)
No nosso projeto, usamos uma **fila FIFO alocada no Heap**, garantindo estabilidade e memória infinita para preencher qualquer forma complexa:

```csharp
public static void FloodFill(DirectBitmap bmp, int startX, int startY, Color targetColor, Color fillColor)
{
    Queue<Point> queue = new Queue<Point>();
    queue.Enqueue(new Point(startX, startY));

    while (queue.Count > 0)
    {
        Point pt = queue.Dequeue();
        int x = (int)pt.X, y = (int)pt.Y;

        if (bmp.GetPixel(x, y) == targetColor)
        {
            bmp.SetPixel(x, y, fillColor);
            // Enfileira os 4 vizinhos (Cima, Baixo, Esquerda, Direita)
            queue.Enqueue(new Point(x + 1, y));
            queue.Enqueue(new Point(x - 1, y));
            queue.Enqueue(new Point(x, y + 1));
            queue.Enqueue(new Point(x, y - 1));
        }
    }
}
```

---

## ✂️ 3. Recorte de Linhas de Cohen-Sutherland (Outcodes)

O algoritmo de **Cohen-Sutherland** determina rapidamente se um segmento de reta está totalmente dentro, totalmente fora ou cruza os limites da janela retangular de recorte $[X_{\min}, X_{\max}, Y_{\min}, Y_{\max}]$.

### Códigos de Região (Outcodes de 4 bits):
O espaço é dividido em 9 regiões usando 4 bits: `[Cima, Baixo, Direita, Esquerda]`

```
 1001 (Top-Left)   | 1000 (Top)    | 1010 (Top-Right)
 ------------------+---------------+------------------
 0001 (Left)       | 0000 (DENTRO) | 0010 (Right)
 ------------------+---------------+------------------
 0101 (Bottom-Left)| 0100 (Bottom) | 0110 (Bottom-Right)
```

### Regras de Decisão Instantâneas via Operadores Binários Bitwise:
- **Totalmente Visível (Aceitação Trivial):**
$$
\text{code}_0 \mid \text{code}_1 == 0
$$
- **Totalmente Invisível (Rejeição Trivial):**
$$
\text{code}_0 \ \& \ \text{code}_1 \neq 0
$$
- **Caso Contrário:** A reta cruza uma das bordas. O ponto de corte é calculado analiticamente por semelhança de triângulos e o teste é repetido.

---

👉 **Próximo Passo:** Entre no módulo de [Computação Gráfica 3D & Matrizes MVP](/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/).
