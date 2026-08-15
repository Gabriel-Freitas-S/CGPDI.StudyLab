---
title: Modelos de Cor & Percepção Humana (ColorSpaces.cs)
description: A física da luz, fisiologia dos fotorreceptores da retina humana e as formulações matemáticas dos espaços RGB, HSV, HSL, YCbCr e CMYK.
---

A cor é a percepção produzida no sistema visual humano em resposta a radiações eletromagnéticas na faixa visível (de $380\text{ nm}$ a $740\text{ nm}$).

O arquivo [`ColorSpaces.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/ColorSpaces.cs) reúne as transformações cromáticas fundamentais.

---

## 1. A Fisiologia dos Cones da Retina

A retina humana contém células fotorreceptoras divididas em:
- **Bastonetes:** Atuam em baixa luminosidade (visão noturna), sem discriminação de cores.
- **Cones:** Responsáveis pela visão em cores (visão diurna). São classificados em três tipos:
  - **Cones L (Long):** Sensíveis ao espectro Vermelho (~$560\text{ nm}$).
  - **Cones M (Medium):** Sensíveis ao espectro Verde (~$530\text{ nm}$).
  - **Cones S (Short):** Sensíveis ao espectro Azul (~$420\text{ nm}$).

:::important[Característica Fisiológica]
Cerca de **64%** dos cones humanos são do tipo L (Vermelho) e **32%** são do tipo M (Verde), enquanto apenas **2% a 4%** são do tipo S (Azul). Por essa razão, os olhos humanos são muito mais sensíveis a variações de brilho no verde e no vermelho do que no azul.
:::

---

## 2. Escala de Cinza Perceptiva (Grayscale)

Se convertermos uma imagem colorida para cinza usando uma média aritmética simples:

$$
Y_{\text{média}} = \frac{R + G + B}{3}
$$

A imagem resultante apresentará distorção perceptiva, pois o azul parecerá excessivamente claro e o verde parecerá escurecido.

### Padrões Internacionais:

#### 1. ITU-R BT.709 (Padrão sRGB e Telas HD):
$$
Y = 0.2126 \cdot R + 0.7152 \cdot G + 0.0722 \cdot B
$$

#### 2. ITU-R BT.601 (Padrão de Televisão Analógica NTSC/PAL):
$$
Y = 0.299 \cdot R + 0.587 \cdot G + 0.114 \cdot B
$$

---

## 3. O Espaço Cilíndrico HSV (Hue, Saturation, Value)

Enquanto o **RGB** reflete a forma como os monitores acendem pequenos LEDs, o **HSV** modela as cores de forma intuitiva para o ser humano:
- **Hue (Matiz - $H$):** Ângulo no círculo de cores de $0^\circ$ a $360^\circ$ ($0^\circ = \text{Vermelho}, 120^\circ = \text{Verde}, 240^\circ = \text{Azul}$).
- **Saturation (Saturação - $S$):** Pureza da cor de $0.0$ (cinza) a $1.0$ (cor viva).
- **Value (Brilho - $V$):** Luminosidade de $0.0$ (preto) a $1.0$ (brilho máximo).

### Fórmulas de Conversão RGB para HSV ($R, G, B \in [0, 1]$):
$$
V = \max(R, G, B), \quad \Delta = V - \min(R, G, B)
$$

$$
S = \begin{cases} 0, & \text{se } V = 0 \\ \frac{\Delta}{V}, & \text{caso contrário} \end{cases}
$$

$$
H = \begin{cases} 
0^\circ, & \text{se } \Delta = 0 \\
60^\circ \times \left( \frac{G - B}{\Delta} \bmod 6 \right), & \text{se } V = R \\
60^\circ \times \left( \frac{B - R}{\Delta} + 2 \right), & \text{se } V = G \\
60^\circ \times \left( \frac{R - G}{\Delta} + 4 \right), & \text{se } V = B 
\end{cases}
$$

---

## 4. O Espaço YCbCr (Compressão JPEG e Vídeo)

No padrão **YCbCr**:
- **$Y$ (Luminância):** O mapa de brilho (preto e branco).
- **$Cb$ (Chroma Blue):** Diferença de cor azul ($B - Y$).
- **$Cr$ (Chroma Red):** Diferença de cor vermelha ($R - Y$).

Como o olho humano percebe menos detalhes em variações de cor do que em variações de brilho, os compressores JPEG descartam parte dos dados dos canais $Cb$ e $Cr$ sem perda perceptível de qualidade (*Subamostragem de Crominância*).

---

## 5. Efeito Sépia Fotográfico

A transformação sépia clássica é realizada por uma multiplicação matricial ponderada nos canais RGB:

$$
\begin{bmatrix} R' \\ G' \\ B' \end{bmatrix} = 
\begin{bmatrix} 
0.393 & 0.769 & 0.189 \\
0.349 & 0.686 & 0.168 \\
0.272 & 0.534 & 0.131 
\end{bmatrix}
\begin{bmatrix} R \\ G \\ B \end{bmatrix}
$$

---

👉 **Próximo Passo:** Veja como o [Gerador de Amostras Sintéticas](/CGPDI.StudyLab/core/gerador-de-amostras/) cria imagens de calibração.
