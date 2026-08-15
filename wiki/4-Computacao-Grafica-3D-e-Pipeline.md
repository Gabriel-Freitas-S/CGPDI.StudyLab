# 🧊 Capítulo 4: Computação Gráfica 3D & Pipeline Gráfico

O ecossistema 3D deste projeto é composto por duas abordagens complementares:
1. **Pipeline 3D em Software (100% CPU do zero)** em [`Graphics3D/SoftwareRenderer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/SoftwareRenderer3D.cs).
2. **Pipeline Acelerado por Hardware (DirectX / WPF)** em [`Graphics3D/WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs).

---

## 1. A Jornada do Vértice 3D (Pipeline Gráfico)

```
[Vértice 3D (Espaço Local do Objeto)]
               │
               ▼  x Matriz Model (Translação, Rotação, Escala)
[Espaço de Mundo (World Space)]
               │
               ▼  x Matriz View (LookAt da Câmera)
[Espaço de Câmera / Olho (View / Eye Space)]
               │
               ▼  x Matriz Projection (Frustum Perspectivo ou Ortográfico)
[Espaço de Recorte Homogêneo (Clip Space [x, y, z, w]^T)]
               │
               ▼  Divisão Perspectiva (w-divide: [x/w, y/w, z/w])
[Coordenadas de Dispositivo Normalizadas (NDC [-1, +1])]
               │
               ▼  Mapeamento Viewport (Screen Mapping)
[Coordenadas de Tela em Pixels (Screen Space [0..W, 0..H])]
               │
               ▼  Rasterização Baricêntrica + Z-Buffering + Shading
[Framebuffer Final (DirectBitmap BGRA32)]
```

---

## 2. Matrizes Homogêneas $4\times4$

### 2.1 Matriz de Câmera LookAt
Construída a partir da posição da câmera $\vec{P}_{\text{eye}}$, alvo $\vec{P}_{\text{target}}$ e vetor para cima $\vec{V}_{\text{up}}$:
$$\vec{Z}_{\text{axis}} = \text{normalize}(\vec{P}_{\text{eye}} - \vec{P}_{\text{target}})$$
$$\vec{X}_{\text{axis}} = \text{normalize}(\vec{V}_{\text{up}} \times \vec{Z}_{\text{axis}})$$
$$\vec{Y}_{\text{axis}} = \vec{Z}_{\text{axis}} \times \vec{X}_{\text{axis}}$$

### 2.2 Projeção Perspectiva vs Ortográfica
* **Perspectiva (com Campo de Visão FOV e Ponto de Fuga):**
  $$M_{\text{proj}} = \begin{bmatrix} \frac{1}{\text{aspect} \cdot \tan(\text{fov}/2)} & 0 & 0 & 0 \\ 0 & \frac{1}{\tan(\text{fov}/2)} & 0 & 0 \\ 0 & 0 & \frac{z_{\text{far}} + z_{\text{near}}}{z_{\text{near}} - z_{\text{far}}} & \frac{2 z_{\text{far}} z_{\text{near}}}{z_{\text{near}} - z_{\text{far}}} \\ 0 & 0 & -1 & 0 \end{bmatrix}$$
* **Ortográfica (Projeção Paralela sem Fuga):**
  $$M_{\text{ortho}} = \begin{bmatrix} \frac{2}{w} & 0 & 0 & 0 \\ 0 & \frac{2}{h} & 0 & 0 \\ 0 & 0 & \frac{-2}{z_{\text{far}} - z_{\text{near}}} & \frac{-(z_{\text{far}} + z_{\text{near}})}{z_{\text{far}} - z_{\text{near}}} \\ 0 & 0 & 0 & 1 \end{bmatrix}$$

---

## 3. Descarte de Faces Ocultas (Back-face Culling) & Z-Buffer

### 3.1 Back-face Culling
Para cada triângulo no espaço de tela com vértices $v_0, v_1, v_2$:
$$\vec{N}_z = (v_1.x - v_0.x)(v_2.y - v_0.y) - (v_1.y - v_0.y)(v_2.x - v_0.x)$$
Se $\vec{N}_z \le 0$, a face está de costas para a câmera e é **descartada instantaneamente sem gastar ciclos de rasterização**.

### 3.2 Coordenadas Baricêntricas & Z-Buffering
Para cada pixel $(x, y)$ dentro da *bounding box* do triângulo, calculam-se os pesos baricêntricos $(\alpha, \beta, \gamma)$:
$$\alpha + \beta + \gamma = 1, \quad \alpha \ge 0, \; \beta \ge 0, \; \gamma \ge 0$$
A profundidade interpolada é:
$$Z(x, y) = \alpha z_0 + \beta z_1 + \gamma z_2$$
Se $Z(x, y) < \text{ZBuffer}[x, y]$, o pixel é aceito, o Z-Buffer é atualizado e a cor é gravada no buffer.

---

## 4. Modelo de Iluminação de Blinn-Phong

$$I = I_a k_a + I_d k_d (\vec{N} \cdot \vec{L}) + I_s k_s (\vec{N} \cdot \vec{H})^\alpha$$

Onde:
* $\vec{N}$: Vetor normal da superfície unitário.
* $\vec{L}$: Vetor unitário apontando para a fonte de luz.
* $\vec{V}$: Vetor unitário apontando para o observador (câmera).
* $\vec{H} = \frac{\vec{L} + \vec{V}}{|\vec{L} + \vec{V}|}$: Half-vector (vetor médio de Blinn).
* $\alpha$: Expoente de brilho (*Shininess*).

---

## 5. Viewport 3D no WPF (`MeshGeometry3D`)

O WPF encapsula o DirectX da GPU através da estrutura `MeshGeometry3D`:
* `Positions` (`Point3DCollection`): Lista de vértices $(x, y, z)$.
* `TriangleIndices` (`Int32Collection`): Triplas de índices formando as faces triangulares.
* `Normals` (`Vector3DCollection`): Vetores normais para interpolação de Gouraud/Phong.
* `TextureCoordinates` (`PointCollection`): Coordenadas UV $[0, 1] \times [0, 1]$.
