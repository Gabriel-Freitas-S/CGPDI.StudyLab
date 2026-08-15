---
title: Morfologia Matemática & Limiarização de Otsu (Morphology.cs)
description: A teoria dos conjuntos aplicada a imagens digitais, elementos estruturantes, erosão, dilatação, abertura, fechamento e o algoritmo de Otsu.
---

A **Morfologia Matemática** é uma ferramenta teórica não-linear baseada na **Teoria dos Conjuntos** e na topologia. Ela é amplamente utilizada para análise de formas geométricas, eliminação de ruídos morfológicos, preenchimento de buracos e separação de objetos conectados.

O arquivo [`Morphology.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/Morphology.cs) contém todas as operações morfológicas fundamentais.

---

## 🧩 1. O Elemento Estruturante (Structuring Element)

O elemento estruturante $B$ é uma pequena máscara binária (como uma matriz $3 \times 3$ ou $5 \times 5$) com uma origem/pivô central.

### Tipos Comuns Suportados no Sistema:

1. **Quadrado $3 \times 3$ (8-conectividade):**
$$
B_{\text{quad}} = \begin{bmatrix} 1 & 1 & 1 \\ 1 & 1 & 1 \\ 1 & 1 & 1 \end{bmatrix}
$$

2. **Cruz / Diamante $3 \times 3$ (4-conectividade):**
$$
B_{\text{cruz}} = \begin{bmatrix} 0 & 1 & 0 \\ 1 & 1 & 1 \\ 0 & 1 & 0 \end{bmatrix}
$$

3. **Disco $5 \times 5$ (Aproximação Euclidiana):**
$$
B_{\text{disco}} = \begin{bmatrix} 
0 & 0 & 1 & 0 & 0 \\ 
0 & 1 & 1 & 1 & 0 \\ 
1 & 1 & 1 & 1 & 1 \\ 
0 & 1 & 1 & 1 & 0 \\ 
0 & 0 & 1 & 0 & 0 
\end{bmatrix}
$$

---

## 📉 2. Operadores Primários: Erosão e Dilatação

### 1. Erosão ($A \ominus B$)
Encolhe os objetos brancos e alarga as regiões pretas de fundo.
- **Em Escala de Cinza:** Substitui o pixel central pelo **menor valor ($\min$)** encontrado sob o elemento estruturante:
$$
(f \ominus B)(x, y) = \min_{(i, j) \in B} f(x + i, \; y + j)
$$

### 2. Dilatação ($A \oplus B$)
Expande os objetos brancos e preenche pequenas frestas pretas.
- **Em Escala de Cinza:** Substitui o pixel central pelo **maior valor ($\max$)** sob o elemento estruturante:
$$
(f \oplus B)(x, y) = \max_{(i, j) \in B} f(x - i, \; y - j)
$$

---

## 🔄 3. Operadores Secundários: Abertura e Fechamento

### 1. Abertura Morfológica ($A \circ B$)
Definida como uma **Erosão seguida por uma Dilatação**:
$$
A \circ B = (A \ominus B) \oplus B
$$
- **Finalidade:** Remove pequenas saliências pontuais claras e ruídos brancos sem alterar a dimensão geral dos objetos principais.

### 2. Fechamento Morfológico ($A \bullet B$)
Definido como uma **Dilatação seguida por uma Erosão**:
$$
A \bullet B = (A \oplus B) \ominus B
$$
- **Finalidade:** Fecha pequenos buracos escuros internos e conecta contornos próximos que estavam quebrados.

---

## 🎭 4. Operadores Morfológicos Avançados

| Operador | Fórmula Matemática | Finalidade Prática |
| :--- | :--- | :--- |
| **Gradiente Morfológico** | $(A \oplus B) - (A \ominus B)$ | Destaca os contornos exatos dos objetos |
| **Top-Hat (White Hat)** | $A - (A \circ B)$ | Destaca elementos e picos de brilho menores que o elemento estruturante |
| **Black-Hat (Bottom Hat)** | $(A \bullet B) - A$ | Destaca vales e detalhes escuros sobre fundo claro |

---

## 🎯 5. Limiarização Ótima de Otsu (Binarização Automática)

Criado por Nobuyuki Otsu em 1979, o algoritmo de **Otsu** calcula de forma 100% automática o limiar $T^*$ ideal de $0$ a $255$ para converter uma imagem em escala de cinza em preto e branco ($0$ e $255$).

### O Critério Matemático:
Otsu busca o limiar $t$ que **maximiza a variância entre as duas classes** (objeto $\omega_0$ e fundo $\omega_1$):

$$
\sigma_B^2(t) = \omega_0(t) \cdot \omega_1(t) \cdot \left[ \mu_0(t) - \mu_1(t) \right]^2
$$

Onde:
- $\omega_0(t) = \sum_{i=0}^{t} P(i)$ e $\omega_1(t) = \sum_{i=t+1}^{255} P(i)$ são as probabilidades de cada classe;
- $\mu_0(t)$ e $\mu_1(t)$ são as médias de intensidade de cada classe.

```csharp
// Algoritmo de Otsu em C# no Morphology.cs:
double maxVariance = 0;
int optimalThreshold = 0;

for (int t = 0; t < 256; t++)
{
    w0 += histogram[t];
    if (w0 == 0) continue;
    
    int w1 = totalPixels - w0;
    if (w1 == 0) break;

    sum0 += t * histogram[t];
    double m0 = (double)sum0 / w0;
    double m1 = (double)(sumTotal - sum0) / w1;

    double varianceBetween = (double)w0 * w1 * (m0 - m1) * (m0 - m1);
    if (varianceBetween > maxVariance)
    {
        maxVariance = varianceBetween;
        optimalThreshold = t;
    }
}
```

---

👉 **Próximo Passo:** Explore as [Transformações Geométricas & Mapeamento Inverso](/CGPDI.StudyLab/pdi/transformacoes-geometricas/).
