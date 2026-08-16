---
title: Renderizador em Software 3D (CPU Pipeline) (SoftwareRenderer3D.cs)
description: Como construir uma placa de vídeo virtual em C# na CPU com Back-face Culling, Coordenadas Baricêntricas, Z-Buffering e Gouraud Shading.
---

O arquivo [`SoftwareRenderer3D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/SoftwareRenderer3D.cs) implementa o **pipeline completo de renderização 3D na CPU**, sem utilizar OpenGL ou DirectX.

---

## 1. Descarte de Faces Ocultas (Back-face Culling)

Em um objeto 3D fechado, as faces de trás nunca são visíveis pela câmera. Se o produto escalar entre a normal da face $\vec{N}$ e o vetor da câmera $\vec{V}$ for positivo ($\vec{N} \cdot \vec{V} \ge 0$), a face está virada para trás e é **descartada imediatamente**, economizando metade do tempo de desenho.

---

## 2. Rasterização por Coordenadas Baricêntricas

Para cada triângulo 2D desenhado na tela, calculamos as **Coordenadas Baricêntricas $(w_0, w_1, w_2)$** de cada pixel dentro da caixa delimitadora (*Bounding Box*). Se $w_0 \ge 0$, $w_1 \ge 0$ e $w_2 \ge 0$, o pixel está dentro do triângulo.

---

## 3. O Algoritmo de Profundidade: Z-Buffer

### A Analogia da Mão na Frente do Rosto:
Se você colocar a mão na frente dos olhos, você não vê a parede atrás porque a sua mão está mais perto.

O **Z-Buffer** é uma matriz de profundidade na memória RAM:
```csharp
float zInterpolado = (float)(w0 * z0 + w1 * z1 + w2 * z2);
int idx = y * width + x;

// Teste de Profundidade do Z-Buffer:
if (zInterpolado < zBuffer[idx])
{
    zBuffer[idx] = zInterpolado; // Atualiza a profundidade mais proxima
    dstRow[x] = corDoPixel;      // Pinta o pixel na tela
}
```

---

## 4. Tonalização Flat versus Gouraud Shading

- **Flat Shading:** Pinta cada face triangular com uma única cor uniforme (efeito facetado *Low-Poly*).
- **Gouraud Shading:** Calcula a luz em cada vértice e interpola as cores suavemente pelo meio do triângulo usando as coordenadas baricêntricas.

---

👉 **Próximo Passo:** Veja como o WPF acelera isso por hardware em [Viewport3D & Câmera Arcball](/cg3d/viewport3d-hardware-wpf/).
