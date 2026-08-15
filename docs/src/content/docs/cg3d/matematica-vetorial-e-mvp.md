---
title: Matemática Vetorial 3D & Matrizes MVP (Math3D.cs)
description: Vetores 3D/4D, Quaternions vs Euler, e a jornada completa das transformações Model-View-Projection (MVP) até o espaço de tela.
---

A Computação Gráfica 3D é essencialmente a arte de mapear vértices que existem em um espaço tridimensional contínuo $(X, Y, Z)$ para pixels em uma grade bidimensional discreta $(x, y)$ da tela.

O arquivo [`Math3D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/Math3D.cs) implementa todas as operações vetoriais e matriciais $4 \times 4$.

---

## 🎯 1. Vetores 3D e Operações Fundamentais

Um vetor $\vec{v} = (x, y, z)$ representa uma direção e magnitude no espaço tridimensional.

### 1. Produto Escalar (Dot Product):
$$
\vec{a} \cdot \vec{b} = a_x b_x + a_y b_y + a_z b_z = \|\vec{a}\| \|\vec{b}\| \cos\theta
$$
- **Aplicações:** Determinar se dois vetores são perpendiculares ($\vec{a}\cdot\vec{b} = 0$), cálculo de iluminação difusa de Lambert e teste de faces visíveis (*Back-face Culling*).

### 2. Produto Vetorial (Cross Product):
$$
\vec{a} \times \vec{b} = \begin{bmatrix} a_y b_z - a_z b_y \\ a_z b_x - a_x b_z \\ a_x b_y - a_y b_x \end{bmatrix}
$$
- **Aplicações:** Gera um vetor ortogonal (perpendicular) a dois outros vetores. É a base para calcular o vetor **Normal de uma face triangular**!

---

## 🔄 2. Rotações 3D: Ângulos de Euler vs Quaternions

- **Ângulos de Euler (Yaw, Pitch, Roll):** Rotações simples em torno dos eixos $X, Y, Z$. Sofrem do problema crônico de **Gimbal Lock** (quando dois eixos de rotação se alinham e o sistema perde 1 grau de liberdade).
- **Quaternions ($q = w + xi + yj + zk$):** Números hipercomplexos de 4 dimensões que representam qualquer rotação contínua no espaço 3D com interpolação suave (SLERP) e **zero risco de Gimbal Lock**.

---

## 🗺️ 3. A Jornada do Vértice 3D: O Pipeline MVP

Para transformar um vértice modelado pelo artista 3D em um ponto desenhado no monitor, ele passa por 5 espaços matemáticos sucessivos:

```mermaid
graph LR
    Local["1️⃣ Espaço de Objeto\n(Local Space)"] -->|Model Matrix| World["2️⃣ Espaço de Mundo\n(World Space)"]
    World -->|View Matrix| Camera["3️⃣ Espaço de Câmera\n(View Space)"]
    Camera -->|Projection Matrix| Clip["4️⃣ Espaço de Corte\n(Clip Space 4D)"]
    Clip -->|Divisão Perspectiva /w| NDC["5️⃣ Coordenadas NDC\n[-1, 1]"]
    NDC -->|Viewport Transform| Screen["6️⃣ Pixels na Tela\n(1920x1080)"]
```

### 1. Matriz de Modelo (Model Matrix - $M_{\text{model}}$):
Posiciona, rotaciona e redimensiona o objeto no mundo:
$$
M_{\text{model}} = T(\vec{pos}) \times R(\vec{rot}) \times S(\vec{scale})
$$

### 2. Matriz de Visualização da Câmera (View Matrix / LookAt - $M_{\text{view}}$):
Move o mundo inteiro de forma que a câmera fique na origem $(0,0,0)$ apontando para o eixo $-Z$:
$$
M_{\text{view}} = \operatorname{LookAt}(\vec{Eye}, \vec{Target}, \vec{Up})
$$

### 3. Matriz de Projeção Perspectiva (Projection Matrix - $M_{\text{proj}}$):
Cria o volume truncado de visão piramidal (**Frustum**) e aplica o efeito onde objetos distantes parecem menores:

$$
M_{\text{proj}} = \begin{bmatrix} 
\frac{1}{\text{aspect} \cdot \tan(\text{fov}/2)} & 0 & 0 & 0 \\ 
0 & \frac{1}{\tan(\text{fov}/2)} & 0 & 0 \\ 
0 & 0 & \frac{z_{\text{far}} + z_{\text{near}}}{z_{\text{near}} - z_{\text{far}}} & \frac{2 z_{\text{far}} z_{\text{near}}}{z_{\text{near}} - z_{\text{far}}} \\ 
0 & 0 & -1 & 0 
\end{bmatrix}
$$

### 4. A Multiplicação Encadeada MVP:
$$
\vec{v}_{\text{clip}} = M_{\text{proj}} \times M_{\text{view}} \times M_{\text{model}} \times \begin{bmatrix} x \\ y \\ z \\ 1 \end{bmatrix}
$$

### 5. Divisão Perspectiva para NDC (Normalized Device Coordinates $[-1, 1]$):
$$
x_{\text{ndc}} = \frac{x_{\text{clip}}}{w_{\text{clip}}}, \quad y_{\text{ndc}} = \frac{y_{\text{clip}}}{w_{\text{clip}}}, \quad z_{\text{ndc}} = \frac{z_{\text{clip}}}{w_{\text{clip}}}
$$

### 6. Mapeamento de Viewport para Pixels de Tela:
$$
x_{\text{tela}} = \left( \frac{x_{\text{ndc}} + 1}{2} \right) \times \text{LarguraDaJanela}
$$
$$
y_{\text{tela}} = \left( \frac{1 - y_{\text{ndc}}}{2} \right) \times \text{AlturaDaJanela}
$$

---

👉 **Próximo Passo:** Veja como o [Renderizador em Software CPU](/CGPDI.StudyLab/cg3d/renderizador-em-software/) rasteriza esses triângulos com Z-Buffer!
