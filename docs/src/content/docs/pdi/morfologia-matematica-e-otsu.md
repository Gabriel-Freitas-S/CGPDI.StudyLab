---
title: Morfologia Matemática & Limiarização de Otsu (Morphology.cs)
description: A teoria dos conjuntos aplicada a imagens digitais, elementos estruturantes, erosão, dilatação, abertura, fechamento e o algoritmo de Otsu.
---

A **Morfologia Matemática** é uma ferramenta para analisar e transformar formas geométricas em imagens, baseada na **Teoria dos Conjuntos**.

O arquivo [`Morphology.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/Morphology.cs) contém todas as operações morfológicas.

---

## 1. O Elemento Estruturante (Structuring Element)

O elemento estruturante $B$ é uma pequena forma de teste (como um quadrado $3 \times 3$, uma cruz ou um disco) que desliza sobre a imagem.

---

## 2. Operações Básicas: Erosão e Dilatação

### 1. Erosão ($A \ominus B$)
Encolhe as formas brancas e aumenta as regiões pretas.
- **Em Escala de Cinza:** Escolhe o **menor valor ($\min$)** sob a máscara:
$$
(f \ominus B)(x, y) = \min_{(i, j) \in B} f(x + i, \; y + j)
$$

### 2. Dilatação ($A \oplus B$)
Engrossa as formas brancas e preenche pequenas falhas pretas.
- **Em Escala de Cinza:** Escolhe o **maior valor ($\max$)** sob a máscara:
$$
(f \oplus B)(x, y) = \max_{(i, j) \in B} f(x - i, \; y - j)
$$

---

## 3. Abertura e Fechamento

### 1. Abertura ($A \circ B = (A \ominus B) \oplus B$)
Uma **Erosão seguida por uma Dilatação**. Remove pontinhos brancos indesejados sem mudar o tamanho geral dos objetos grandes.

### 2. Fechamento ($A \bullet B = (A \oplus B) \ominus B$)
Uma **Dilatação seguida por uma Erosão**. Fecha furinhos pretos dentro dos objetos e conecta partes quebradas.

---

## 4. Limiarização Automática de Otsu (Binarização Preto e Branco)

O algoritmo de **Otsu** descobre sozinho o número de corte $T^*$ ideal para transformar uma imagem em escala de cinza em preto e branco perfeito.

Ele calcula o ponto que **maximiza a separação estatística entre o fundo e os objetos** (variância interclasses $\sigma_B^2$):

$$
\sigma_B^2(t) = \omega_0(t) \cdot \omega_1(t) \cdot \left[ \mu_0(t) - \mu_1(t) \right]^2
$$

---

**Próximo Passo:** Explore as [Transformações Geométricas & Mapeamento Inverso](/pdi/transformacoes-geometricas/).
