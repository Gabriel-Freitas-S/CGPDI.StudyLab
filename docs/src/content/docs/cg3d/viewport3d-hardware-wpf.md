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

👉 **Próximo Passo:** Explore a [Modelagem Hierárquica e Grafos de Cena (Unidade 3)](/CGPDI.StudyLab/hierarquia/grafos-de-cena-e-teoria/).
