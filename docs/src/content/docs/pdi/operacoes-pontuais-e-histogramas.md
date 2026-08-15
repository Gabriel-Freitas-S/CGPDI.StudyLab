---
title: Operações Pontuais & Histogramas (PointAndHistograms.cs)
description: Teoria e implementação de Brilho, Contraste, LUTs, Correção Gamma, Equalização de Histograma por CDF e Normalização Min-Max.
---

As **Operações Pontuais** constituem a classe elementar e mais rápida de processamento de imagens.

Nelas, a nova cor de um pixel $g(x, y)$ depende **exclusivamente** da cor original daquele mesmo ponto $f(x, y)$, sem depender dos vizinhos:

$$
g(x, y) = T\left[ f(x, y) \right]
$$

Onde $T$ é a função de transformação de intensidade.

---

## 1. Ajuste Linear de Brilho

### A Analogia do Interruptor com Dimmer:
O brilho funciona como girar o botão dimmer de uma lâmpada: todos os pontos da imagem recebem um valor fixo a mais (ou a menos) de luz:

$$
g(x, y) = \text{clamp}\left( f(x, y) + \beta, \; 0, \; 255 \right)
$$

- $\beta > 0$: Clareia a imagem uniformemente.
- $\beta < 0$: Escurece a imagem.

---

## 2. Ajuste de Contraste com Pivô Central

### A Analogia do Elástico Esticado:
O contraste funciona como puxar um elástico fixo no meio ($128$). Os tons claros são esticados para ficarem ainda mais claros, e os escuros são esticados para ficarem ainda mais escuros:

$$
g(x, y) = \text{clamp}\left( \alpha \cdot (f(x, y) - 128) + 128, \; 0, \; 255 \right)
$$

- $\alpha > 1$: Aumenta o contraste (separa mais os tons claros e escuros).
- $0 \le \alpha < 1$: Reduz o contraste (aproxima todos os tons do cinza neutro).

---

## 3. Otimização por Look-Up Tables (LUTs)

### A Analogia da Tabuada Pronta:
Em vez de calcular a mesma fórmula matemática $262.144$ vezes para cada pixel, calculamos uma tabela de respostas pronta de 0 a 255 (apenas 256 números) antes de começar. Durante o processamento, o computador apenas consulta o valor na tabela instantaneamente ($O(1)$):

```csharp
// 1. Tabela pre-calculada de 256 posicoes:
byte[] lut = new byte[256];
for (int i = 0; i < 256; i++)
{
    double val = contrastFactor * (i - 128.0) + 128.0;
    lut[i] = (byte)Math.Clamp(val, 0, 255);
}

// 2. Consulta direta nos pixels:
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

## 4. Correção Gamma (Power-Law Transform)

A percepção de claridade dos olhos humanos segue uma curva exponencial:

$$
g(x, y) = 255 \times \left( \frac{f(x, y)}{255} \right)^\gamma
$$

- $\gamma < 1$ (ex: $\gamma = 0.5$): Clareia sombras e áreas escuras suavemente.
- $\gamma > 1$ (ex: $\gamma = 2.2$): Aprofunda sombras e acentua realces.

---

## 5. Histograma e Equalização por CDF Acumulada

O histograma é o gráfico que mostra quantos pixels de cada tom de cinza (de 0 a 255) existem na imagem.

A **Equalização de Histograma** redistribui as intensidades para que os tons fiquem bem espalhados por toda a faixa dinâmica:

1. **Calcula o Histograma $H[i]$.**
2. **Calcula a Função de Distribuição Acumulada (CDF):**
$$
\text{CDF}(i) = \sum_{j=0}^{i} H[j]
$$
3. **Mapeia cada tom de pixel original $v$ para o valor equalizado:**
$$
h_{\text{eq}}(v) = \text{round}\left( \frac{\text{CDF}(v) - \text{CDF}_{\min}}{(W \times H) - \text{CDF}_{\min}} \times 255 \right)
$$

---

👉 **Próximo Passo:** Aprenda sobre [Filtros Espaciais e Convoluções 2D](/CGPDI.StudyLab/pdi/filtros-espaciais-e-convolucoes/).
