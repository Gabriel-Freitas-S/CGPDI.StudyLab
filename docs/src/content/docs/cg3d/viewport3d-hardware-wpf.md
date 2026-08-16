---
title: Viewport3D por Hardware & Câmera Arcball (WpfViewport3DManager.cs)
description: Renderização DirectX acelerada por GPU no WPF, câmera orbital esférica com controle por mouse, fontes de luz e materiais Phong.
---

O arquivo [`WpfViewport3DManager.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/WpfViewport3DManager.cs) gerencia a visualização de modelos 3D com aceleração de hardware via **DirectX**.

---

## 1. Câmera Orbital Arcball com Coordenadas Esféricas

A câmera orbita suavemente ao redor da cena utilizando coordenadas esféricas:
- **Distância ($r$):** Controlada pelo *Scroll* do mouse.
- **Ângulo Azimutal ($\theta$):** Rotação horizontal de $0^\circ$ a $360^\circ$.
- **Ângulo Polar ($\phi$):** Elevação vertical de $-89^\circ$ a $+89^\circ$.

$$
X = r \cdot \cos(\phi) \cdot \sin(\theta), \quad Y = r \cdot \sin(\phi), \quad Z = r \cdot \cos(\phi) \cdot \cos(\theta)
$$

---

## 2. Iluminação e Materiais de Phong

- **`DiffuseMaterial` (Reflexão Difusa):** A luz espalha igualmente em todas as direções ($I_d = k_d (\vec{N} \cdot \vec{L})$).
- **`SpecularMaterial` (Brilho Especular):** Simula o brilho refletivo de superfícies polidas ($I_s = k_s (\vec{R} \cdot \vec{V})^\alpha$).

---

<div class="ms-ref-card">
  <h4>📚 Referências Oficiais Microsoft Learn</h4>
  <p>Documentação da API de Gráficos 3D do WPF:</p>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.viewport3d" target="_blank" rel="noopener">Classe Viewport3D (System.Windows.Controls)</a> — Container visual para renderização de cenas 3D no espaço de interface 2D.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.perspectivecamera" target="_blank" rel="noopener">Classe PerspectiveCamera</a> — Configuração de posição, vetor <code>LookDirection</code>, <code>UpDirection</code> e campo de visão (<code>FieldOfView</code>).</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.material" target="_blank" rel="noopener">Classe Material (System.Windows.Media.Media3D)</a> — Composição de materiais com <code>DiffuseMaterial</code>, <code>SpecularMaterial</code> e <code>EmissiveMaterial</code>.</li>
  </ul>
</div>

---

👉 **Próximo Passo:** Explore a [Modelagem Hierárquica e Grafos de Cena (Unidade 3)](/hierarquia/grafos-de-cena-e-teoria/).
