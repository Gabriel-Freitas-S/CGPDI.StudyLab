---
title: Operações Pontuais & Histogramas (PointAndHistograms.cs)
description: Teoria e implementação de Brilho, Contraste, LUTs, Correção Gamma, Equalização de Histograma por CDF e Normalização Min-Max.
---

As **Operações Pontuais** constituem a classe mais elementar e computacionalmente rápida de processamento de imagens.

Nelas, o valor de intensidade do pixel resultante $g(x, y)$ depende **exclusivamente** da intensidade do pixel original correspondente $f(x, y)$, sem levar em consideração os pixels vizinhos:

$$
g(x, y) = T\left[ f(x, y) \right]
$$

Onde $T$ é uma função de transformação de intensidade.

---

## ☀️ 1. Ajuste Linear de Brilho

O ajuste de brilho é uma transformação **aditiva** simples:

$$
g(x, y) = \text{clamp}\left( f(x, y) + \beta, \; 0, \; 255 \right)
$$

- $\beta > 0$: Clareia a imagem uniformemente.
- $\beta < 0$: Escurece a imagem.

---

## 🌓 2. Ajuste de Contraste com Pivô Central

O contraste mede a amplitude da faixa dinâmica entre os tons mais escuros e mais claros. Para não alterar o nível médio de cinza da imagem, o contraste é calculado usando o **pivô central $128$**:

$$
g(x, y) = \text{clamp}\left( \alpha \cdot (f(x, y) - 128) + 128, \; 0, \; 255 \right)
$$

- $\alpha > 1$: Aumenta o contraste (tons claros ficam mais claros, escuros ficam mais escuros).
- $0 \le \alpha < 1$: Diminui o contraste (aproxima todos os tons do cinza médio).

---

## ⚡ 3. Otimização Crítica: Look-Up Tables (LUTs)

Como uma imagem de $512 \times 512$ pixels tem $262.144$ pixels, mas os valores de entrada possíveis para cada canal de cor são **apenas 256 inteiros** ($0$ a $255$), é computacionalmente tolo recalcular a fórmula de contraste $262.144$ vezes.

Em vez disso, usamos uma **Look-Up Table (LUT)**:

```csharp
// 1. Pré-calculamos a tabela apenas 256 vezes:
byte[] lut = new byte[256];
for (int i = 0; i < 256; i++)
{
    double val = contrastFactor * (i - 128.0) + 128.0;
    lut[i] = (byte)Math.Clamp(val, 0, 255);
}

// 2. No laço dos pixels, fazemos apenas indexação instantânea O(1):
Parallel.For(0, height, y =>
{
    byte* pDst = dst.BackBuffer + (y * dst.Stride);
    byte* pSrc = src.BackBuffer + (y * src.Stride);
    for (int x = 0; x < width; x++)
    {
        int px = x * 4;
        pDst[px + 0] = lut[pSrc[px + 0]]; // B
        pDst[px + 1] = lut[pSrc[px + 1]]; // G
        pDst[px + 2] = lut[pSrc[px + 2]]; // R
    }
});
```

---

## 🌈 4. Correção Gamma (Power-Law Transform)

A percepção humana de brilho e a resposta dos monitores físicos não são lineares, seguindo uma curva exponencial:

$$
g(x, y) = 255 \times \left( \frac{f(x, y)}{255} \right)^\gamma
$$

- $\gamma < 1$ (ex: $\gamma = 0.5$): Expande tons escuros e sombras (clareamento não-linear).
- $\gamma > 1$ (ex: $\gamma = 2.2$): Comprime sombras e destaca altas luzes.

---

## 📊 5. Histograma de Intensidade

O histograma de uma imagem digital é um vetor $H$ de $256$ posições onde cada posição $H[i]$ representa a contagem exata de pixels que possuem o nível de cinza $i \in [0, 255]$:

$$
H[i] = \sum_{y=0}^{H-1} \sum_{x=0}^{W-1} \begin{cases} 1, & \text{se } f(x, y) = i \\ 0, & \text{caso contrário} \end{cases}
$$

A **Função de Probabilidade Normalizada (PDF)** é dada por:

$$
P(i) = \frac{H[i]}{W \times H}
$$

---

## 📈 6. Equalização de Histograma por CDF Acumulada

Imagens de baixo contraste possuem histogramas concentrados em uma faixa estreita de tons. A **Equalização de Histograma** redistribui as intensidades de forma que o histograma resultante seja aproximadamente uniforme e plano.

### Algoritmo Matemático:

1. **Calcula o Histograma $H[i]$** de $0$ a $255$.
2. **Calcula a Função de Distribuição Acumulada (CDF - Cumulative Distribution Function):**
$$
\text{CDF}(i) = \sum_{j=0}^{i} H[j]
$$
3. **Identifica o primeiro valor não-nulo $\text{CDF}_{\min}$.**
4. **Mapeia cada pixel original $v$ para o novo valor equalizado $h_{\text{eq}}(v)$:**
$$
h_{\text{eq}}(v) = \text{round}\left( \frac{\text{CDF}(v) - \text{CDF}_{\min}}{(W \times H) - \text{CDF}_{\min}} \times 255 \right)
$$

```csharp
// Mapeamento em C# no PointAndHistograms.cs:
byte[] eqLut = new byte[256];
int totalPixels = src.Width * src.Height;

for (int i = 0; i < 256; i++)
{
    eqLut[i] = (byte)Math.Clamp((int)Math.Round(((double)(cdf[i] - cdfMin) / (totalPixels - cdfMin)) * 255.0), 0, 255);
}
```

---

## 📏 7. Normalização de Histograma (Min-Max Stretching)

Expande linearmente uma imagem com valores concentrados entre $[\text{Min}, \text{Max}]$ para utilizar toda a escala $[0, 255]$:

$$
g(x, y) = \text{round}\left( \frac{f(x, y) - \text{Min}}{\text{Max} - \text{Min}} \times 255 \right)
$$

---

👉 **Próximo Passo:** Aprenda sobre [Filtros Espaciais e Convoluções 2D](/CGPDI.StudyLab/pdi/filtros-espaciais-e-convolucoes/).
