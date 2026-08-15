---
title: Matemática Vetorial 3D & Matrizes MVP (Math3D.cs)
description: Vetores 3D/4D, Quaternions vs Euler, e a jornada completa das transformações Model-View-Projection (MVP) até o espaço de tela.
---

A Computação Gráfica 3D é o conjunto de transformações matemáticas que mapeiam pontos no espaço tridimensional $(X, Y, Z)$ para pixels na tela bidimensional $(x, y)$.

O arquivo [`Math3D.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/Math3D.cs) implementa todas as operações vetoriais e matriciais $4 \times 4$.

---

## 1. Operações Vetoriais Fundamentais

- **Produto Escalar (Dot Product):** $\vec{a} \cdot \vec{b} = a_x b_x + a_y b_y + a_z b_z$. Usado para iluminação difusa e teste de faces visíveis.
- **Produto Vetorial (Cross Product):** $\vec{a} \times \vec{b}$. Gera um vetor perpendicular aos dois vetores de entrada (utilizado para calcular o vetor **Normal** de triângulos).

---

## 2. A Jornada do Vértice: O Pipeline MVP

Para transformar um ponto 3D em um pixel na tela:

```mermaid
graph LR
    Local["1. Espaco do Objeto"] -->|Model Matrix| World["2. Espaco de Mundo"]
    World -->|View Matrix| Camera["3. Espaco da Camera"]
    Camera -->|Projection Matrix| Clip["4. Espaco de Corte 4D"]
    Clip -->|Divisao Perspectiva por w| NDC["5. Coordenadas NDC"]
    NDC -->|Mapeamento de Viewport| Screen["6. Pixels na Tela"]
```

1. **Model Matrix ($M_{\text{model}}$):** Posiciona e rotaciona o objeto no mundo.
2. **View Matrix ($M_{\text{view}}$):** Move a cena para o ponto de vista da câmera (LookAt).
3. **Projection Matrix ($M_{\text{proj}}$):** Aplica a perspectiva (objetos distantes parecem menores).
4. **Divisão Perspectiva:** Converte para coordenadas normalizadas (NDC $[-1, 1]$).
5. **Mapeamento de Viewport:** Converte para a resolução em pixels da janela do aplicativo.

---

<div class="ms-ref-card">
  <h4>📚 Referências Oficiais Microsoft Learn</h4>
  <p>Conceitos matemáticos e classes do .NET para transformações geométricas e gráficos 3D:</p>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.numerics" target="_blank" rel="noopener">System.Numerics Namespace</a> — Tipos acelerados por hardware via SIMD: <code>Vector3</code>, <code>Vector4</code>, <code>Matrix4x4</code> e <code>Quaternion</code>.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview" target="_blank" rel="noopener">Visão Geral de Transformações no WPF</a> — Operações matriciais de translação, rotação e escala.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.matrix3d" target="_blank" rel="noopener">Estrutura Matrix3D (System.Windows.Media.Media3D)</a> — Representação de matriz de transformação afim 4x4 em ponto flutuante.</li>
  </ul>
</div>

---

👉 **Próximo Passo:** Veja como o [Renderizador em Software CPU](/CGPDI.StudyLab/cg3d/renderizador-em-software/) rasteriza esses triângulos com Z-Buffer.
