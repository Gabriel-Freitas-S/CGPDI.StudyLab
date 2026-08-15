---
title: Renderizador em Software 3D (CPU Pipeline) (SoftwareRenderer3D.cs)
description: Como construir uma placa de vídeo virtual em C# na CPU com Back-face Culling, Coordenadas Baricêntricas, Z-Buffering e Gouraud Shading.
---

O arquivo [`SoftwareRenderer3D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/SoftwareRenderer3D.cs) é uma das joias pedagógicas do projeto: ele implementa **todo o pipeline de uma GPU moderna do zero**, sem utilizar OpenGL ou DirectX, calculando cada triângulo e pixel na CPU.

---

## 🚫 1. Descarte de Faces Ocultas (Back-face Culling)

Em um objeto 3D fechado (como um cubo, esfera ou nave espacial), as faces de trás nunca são vistas pela câmera. 

Para economizar 50% do tempo de desenho, calculamos o produto escalar entre o vetor **Normal da Face $\vec{N}$** e o vetor que aponta para a **Câmera $\vec{V}$**:

$$
\text{Visível} = (\vec{N} \cdot \vec{V} < 0)
$$

- Se o produto escalar for positivo ($\ge 0$), a face está de costas para a câmera e é **descartada imediatamente** antes mesmo de ser rasterizada!

---

## 📐 2. Rasterização por Bounding Box & Coordenadas Baricêntricas

Para cada triângulo 2D projetado na tela com vértices $P_0 = (x_0, y_0)$, $P_1 = (x_1, y_1)$ e $P_2 = (x_2, y_2)$:

1. **Calcula a Caixa Delimitadora (Bounding Box):**
$$
X_{\min} = \min(x_0, x_1, x_2), \quad X_{\max} = \max(x_0, x_1, x_2)
$$
$$
Y_{\min} = \min(y_0, y_1, y_2), \quad Y_{\max} = \max(y_0, y_1, y_2)
$$

2. **Para cada pixel $(x, y)$ dentro da caixa, calcula as Coordenadas Baricêntricas $(w_0, w_1, w_2)$:**

$$
w_0 = \frac{(y_1 - y_2)(x - x_2) + (x_2 - x_1)(y - y_2)}{\text{ÁreaTotal}}
$$
$$
w_1 = \frac{(y_2 - y_0)(x - x_2) + (x_0 - x_2)(y - y_2)}{\text{ÁreaTotal}}
$$
$$
w_2 = 1.0 - w_0 - w_1
$$

- Se $w_0 \ge 0$, $w_1 \ge 0$ e $w_2 \ge 0$, o pixel está **dentro do triângulo**!

---

## 🛡️ 3. O Algoritmo de Profundidade: Z-Buffer (Depth Buffer)

Quando dois ou mais triângulos se sobrepõem no mesmo pixel da tela, como o computador sabe qual está na frente?

Mantemos uma matriz de ponto flutuante `float[] zBuffer` do tamanho da imagem, inicializada com $+\infty$.

```csharp
// Interpola a profundidade Z baricêntrica do pixel:
float zInterpolado = (float)(w0 * z0 + w1 * z1 + w2 * z2);

int idx = y * width + x;

// Teste de Profundidade do Z-Buffer:
if (zInterpolado < zBuffer[idx])
{
    zBuffer[idx] = zInterpolado; // Atualiza com a nova profundidade mais próxima
    dstRow[x] = corDoPixel;      // Pinta o pixel na tela
}
```

---

## 💡 4. Modelos de Tonalização: Flat vs Gouraud Shading

| Modelo de Shading | Como calcula a iluminação? | Aspecto Visual |
| :--- | :--- | :--- |
| **Flat Shading** | Calcula 1 única cor de luz por face triangular usando a normal da face. | Facetado, estilo *Low-Poly*. |
| **Gouraud Shading** | Calcula a iluminação nos 3 vértices e **interpola as cores linearmente** no interior do triângulo via coordenadas baricêntricas. | Superfície perfeitamente curva e suave. |

---

👉 **Próximo Passo:** Veja como o WPF acelera isso por hardware em [Viewport3D & Câmera Arcball](/CGPDI.StudyLab/cg3d/viewport3d-hardware-wpf/).
