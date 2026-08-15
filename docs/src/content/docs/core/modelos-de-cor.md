---
title: Modelos de Cor & Percepção Humana (ColorSpaces.cs)
description: A física da luz, fisiologia dos cones da retina humana e as fórmulas matemáticas dos espaços RGB, HSV, HSL, YCbCr e CMYK.
---

A cor não é uma propriedade física absoluta dos objetos, mas sim uma sensação biológica produzida no cérebro humano em resposta a ondas do espectro eletromagnético visível (de $380\text{ nm}$ a $740\text{ nm}$).

O arquivo [`ColorSpaces.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Core/ColorSpaces.cs) implementa as principais transformações de cor usadas na indústria de computação visual.

---

## 👁️ 1. A Fisiologia dos Cones da Retina

A retina humana contém dois tipos principais de células fotorreceptoras:
- **Bastonetes:** Sensíveis à intensidade luminosa em ambientes escuros (visão escotópica), sem percepção de cor.
- **Cones:** Responsáveis pela visão colorida (visão fotópica). Existem 3 tipos:
  - **Cones L (Long):** Pico de sensibilidade no espectro Vermelho (~$560\text{ nm}$).
  - **Cones M (Medium):** Pico de sensibilidade no espectro Verde (~$530\text{ nm}$).
  - **Cones S (Short):** Pico de sensibilidade no espectro Azul (~$420\text{ nm}$).

:::important[Curiosidade Fisiológica Fundamental]
Aproximadamente **64%** dos nossos cones são do tipo L (Vermelho) e **32%** são do tipo M (Verde). Apenas **2% a 4%** são cones S (Azul). Por isso, **o olho humano é muito mais sensível a variações de brilho no verde e no vermelho do que no azul!**
:::

---

## 📺 2. Escala de Cinza Perceptiva (Grayscale)

Por causa da sensibilidade dos cones explicada acima, se calcularmos a escala de cinza por uma média aritmética simples:

$$
Y_{\text{média}} = \frac{R + G + B}{3}
$$

A imagem resultante parecerá artificial, com azuis escuros claros demais e verdes vibrantes apagados.

### As Normas Internacionais Corretas:

#### 1. ITU-R BT.709 (Padrão para Monitores HDTV e sRGB moderno):
$$
Y = 0.2126 \cdot R + 0.7152 \cdot G + 0.0722 \cdot B
$$

#### 2. ITU-R BT.601 (Padrão histórico de TV analógica NTSC/PAL):
$$
Y = 0.299 \cdot R + 0.587 \cdot G + 0.114 \cdot B
$$

```csharp
// Implementação ultra-rápida em C# usando apenas inteiros:
public static byte RgbToGrayscaleBT709(byte r, byte g, byte b)
{
    return (byte)((r * 2126 + g * 7152 + b * 722) / 10000);
}
```

---

## 🎨 3. O Espaço Cilíndrico HSV (Hue, Saturation, Value)

Enquanto o **RGB** é ótimo para o hardware dos monitores, ele é péssimo para humanos escolherem cores ou para segmentação por visão computacional.

O espaço **HSV** organiza as cores em um cone/cilindro:
- **Hue (Matiz - $H$):** Ângulo no círculo cromático de $0^\circ$ a $360^\circ$ ($0^\circ = \text{Vermelho}, 120^\circ = \text{Verde}, 240^\circ = \text{Azul}$).
- **Saturation (Saturação - $S$):** Pureza da cor de $0.0$ (cinza) a $1.0$ (cor pura).
- **Value (Brilho - $V$):** Luminosidade de $0.0$ (preto total) a $1.0$ (brilho máximo).

### Fórmulas de Conversão RGB $\to$ HSV:
Seja $R, G, B \in [0, 1]$:

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

## 📹 4. O Espaço YCbCr (Compressão JPEG e Vídeo MPEG)

No modelo **YCbCr**, separamos:
- **$Y$ (Luminância):** O mapa em escala de cinza da imagem (brilho perceptivo).
- **$Cb$ (Chroma Blue):** A diferença entre o canal azul e o brilho ($B - Y$).
- **$Cr$ (Chroma Red):** A diferença entre o canal vermelho e o brilho ($R - Y$).

### Por que ele é a base do JPEG?
Como a visão humana percebe muito pouco detalhes finos em variações de cor se comparado ao brilho, os compressores JPEG descartam metade da resolução dos canais $Cb$ e $Cr$ (*Chroma Subsampling 4:2:0*) sem que o olho humano note qualquer perda de qualidade visual!

### Fórmulas Matemáticas (ITU-R BT.601):
$$
Y = 0.299 R + 0.587 G + 0.114 B
$$
$$
Cb = 128 - 0.168736 R - 0.331264 G + 0.5 B
$$
$$
Cr = 128 + 0.5 R - 0.418688 G - 0.081312 B
$$

---

## 🖨️ 5. O Modelo CMYK (Impressão Gráfica)

O modelo **CMYK** (*Cyan, Magenta, Yellow, Key/Black*) é um modelo **subtrativo** utilizado em tintas e impressoras físicas. Ao invés de somar luz, as tintas absorvem comprimentos de onda específicos:

$$
K = 1 - \max(R, G, B)
$$
$$
C = \frac{1 - R - K}{1 - K}, \quad M = \frac{1 - G - K}{1 - K}, \quad Y = \frac{1 - B - K}{1 - K}
$$

---

## 🎞️ 6. Efeito Fotográfico Sépia Clássico

O efeito sépia simula fotografias antigas do século XIX através de uma multiplicação matricial ponderada nos canais RGB:

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

👉 **Próximo Passo:** Veja como o [Gerador de Amostras Sintéticas](/CGPDI.StudyLab/core/gerador-de-amostras/) cria imagens de teste.
