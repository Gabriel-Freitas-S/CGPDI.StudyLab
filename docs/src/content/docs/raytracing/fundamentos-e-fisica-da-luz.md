---
title: Fundamentos do Ray Tracing & Modelo Phong (Raytracer3D.cs)
description: A física da luz, por que traçamos raios ao contrário (da câmera para as luzes), raios de sombra e o modelo de iluminação local de Phong.
---

O **Ray Tracing (Traçado de Raios)** é a técnica de renderização que simula com maior fidelidade o comportamento físico da luz no mundo real. É o algoritmo por trás dos efeitos visuais dos filmes de Hollywood e dos jogos de última geração.

O arquivo [`Raytracer3D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/Raytracer3D.cs) implementa um **Whitted Ray Tracer** recursivo de alta performance.

---

## 🔦 1. Por que Traçamos Raios "de Trás para Frente"?

Na natureza física:
1. O Sol emite bilhões de fótons de luz por segundo em todas as direções;
2. Quase todos esses fótons se perdem no espaço ou são absorvidos;
3. Apenas uma fração infinitesimal de fótons atinge a pupila do olho humano.

Se um computador tentasse simular os raios saindo da lâmpada, **99.9999% do poder da CPU seria desperdiçado** calculando raios de luz que nunca seriam vistos pela câmera!

### O Truque do Ray Tracing (Turner Whitted, 1980):
Lançamos raios **exclusivamente a partir da câmera**, passando por cada pixel da tela em direção à cena 3D:

```mermaid
graph LR
    Camera["📷 Olho / Câmera"] -->|1. Raio Primário| Pixel["🔲 Pixel da Tela"]
    Pixel --> HitPoint["💥 Ponto de Impacto na Esfera"]
    HitPoint -->|2. Raio de Sombra| Light["💡 Lâmpada / Luz"]
    HitPoint -->|3. Raio Refletido| Mirror["🪞 Outro Objeto"]
```

---

## 📏 2. A Equação Paramétrica do Raio 3D

Um raio é uma semirreta matemática definida por uma **Origem $\vec{O}$** e um vetor unitário de **Direção normalizada $\vec{D}$**:

$$
\vec{r}(t) = \vec{O} + t \cdot \vec{D}, \quad \text{onde } t > 0
$$

- $t$ é a distância percorrida pelo raio ao longo do vetor $\vec{D}$.
- Se $t \le 0$, o objeto está atrás da câmera e é ignorado.

---

## 🌓 3. Raios de Sombra (Shadow Rays)

Como o Ray Tracer sabe se um ponto na superfície de uma esfera está iluminado ou na sombra?

1. No ponto de impacto $\vec{P} = \vec{r}(t)$, calculamos a direção para a lâmpada:
$$
\vec{L}_{\text{dir}} = \frac{\vec{Pos}_{\text{luz}} - \vec{P}}{\|\vec{Pos}_{\text{luz}} - \vec{P}\|}
$$
2. Lançamos um novo raio (**Raio de Sombra**) com origem em $\vec{P} + \epsilon \cdot \vec{N}$ apontando para $\vec{L}_{\text{dir}}$.
3. Se o raio atingir qualquer objeto opaco antes de chegar na lâmpada ($0 < t_{\text{sombra}} < \text{DistânciaDaLuz}$), o ponto está **em sombra total**!
4. Se o caminho estiver desobstruído, calculamos a iluminação direta de Phong.

:::note[Por que usamos $\epsilon \cdot \vec{N}$ (Shadow Bias)?]
Para evitar que o raio de sombra colida com o próprio ponto de onde ele acabou de sair devido a pequenas imprecisões de ponto flutuante (*Shadow Acne*).
:::

---

## 💡 4. O Modelo de Iluminação Completo de Phong

A intensidade de cor resultante em cada ponto de impacto é a soma de 3 componentes físicos:

$$
I_{\text{total}} = \underbrace{I_a k_a}_{\text{Luz Ambiente}} + \underbrace{I_d k_d (\vec{N} \cdot \vec{L})}_{\text{Reflexão Difusa (Lambert)}} + \underbrace{I_s k_s (\vec{R} \cdot \vec{V})^\alpha}_{\text{Brilho Especular (Phong)}}
$$

Onde:
- $\vec{N}$ é a normal da superfície;
- $\vec{L}$ é a direção da luz;
- $\vec{V}$ é a direção para a câmera;
- $\vec{R} = 2(\vec{N} \cdot \vec{L})\vec{N} - \vec{L}$ é o reflexo da luz;
- $\alpha$ é a rugosidade/brilho do material (*Shininess*).

---

👉 **Próximo Passo:** Veja como calcular analiticamente a [Interseção Raio-Esfera e Raio-Plano](/CGPDI.StudyLab/raytracing/intersecao-e-geometria/).
