---
title: Transformações Geométricas & Interpolação (GeometricTransforms.cs)
description: Mapeamento Direto vs Inverso, interpolações Vizinho Mais Próximo, Bilinear e Bicúbica, rotações e deformações não-lineares.
---

As **Transformações Geométricas** alteram as posições espaciais dos pixels, permitindo girar, esticar e deformar imagens.

O arquivo [`GeometricTransforms.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/ImageProcessing/GeometricTransforms.cs) implementa transformações lineares e não-lineares.

---

## 1. Mapeamento Direto versus Mapeamento Inverso

### Por que o Mapeamento Inverso é Necessário?
Se pegarmos os pixels da imagem original e calcularmos suas novas posições para frente, alguns pontos da tela destino ficarão sem receber nenhum pixel, criando **"buracos pretos"** indesejados.

No **Mapeamento Inverso**, fazemos a pergunta ao contrário:
> *"Para cada pixel da imagem final $(x', y')$, qual é a posição correspondente de onde ele veio na imagem original $(x, y) = T^{-1}(x', y')$?"*

Isso garante que **100% dos pixels da imagem destino serão preenchidos sem nenhuma falha**.

---

## 2. Métodos de Interpolação Espacial

Como a fórmula inversa quase sempre resulta em números fracionários (ex: $x = 104.3, y = 52.8$), precisamos estimar a cor naquele ponto contínuo:

### 1. Vizinho Mais Próximo (Nearest Neighbor)
Arredonda as coordenadas para o número inteiro mais próximo:
- Muito rápido ($O(1)$), mas gera bordas serrilhadas (efeito pixelado).

### 2. Interpolação Bilinear (4 Vizinhos)
Faz uma média ponderada suave entre os 4 pixels vizinhos mais próximos:
$$
\begin{aligned}
f(x, y) = & (1 - dx)(1 - dy) \cdot f(x_0, y_0) + \\
          & dx(1 - dy) \cdot f(x_1, y_0) + \\
          & (1 - dx)dy \cdot f(x_0, y_1) + \\
          & dx \cdot dy \cdot f(x_1, y_1)
\end{aligned}
$$

### 3. Interpolação Bicúbica (16 Vizinhos)
Utiliza uma vizinhança de 16 pixels com curvas spline cúbicas para manter os contornos nítidos e naturais.

---

## 3. Deformações Espaciais Não-Lineares

- **Redemoinho (Swirl):** Gira a imagem em espiral ao redor do centro.
- **Ondulação (Ripple):** Aplica ondas senoidais como água em movimento.
- **Olho de Peixe (Fisheye):** Simula a distorção esférica de lentes grande-angulares.

---

**Próximo Passo:** Explore a [Transformada de Fourier 2D & Geração Procedural](/pdi/dominio-da-frequencia-e-ruidos/).
