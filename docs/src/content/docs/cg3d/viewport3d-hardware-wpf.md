---
title: Viewport3D por Hardware, Malhas & Câmeras (WpfViewport3DManager.cs)
description: Renderização DirectX acelerada por GPU no WPF, MeshGeometry3D, câmeras de perspectiva, modelo difuso de Lambert e repetição de texturas.
---

O arquivo [`WpfViewport3DManager.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/WpfViewport3DManager.cs) gerencia a visualização de modelos 3D com aceleração de hardware via **DirectX** e pipeline de malhas triangulares do WPF.

---

## 1. Malhas Triangulares com MeshGeometry3D

No WPF, todo modelo tridimensional é constituído por triângulos descritos na classe `MeshGeometry3D`:

- **`Positions`:** Vetores tridimensionais $(X, Y, Z)$ definindo a localização de cada vértice no espaço local.
- **`TriangleIndices`:** Trios de índices inteiros que conectam os vértices para formar as faces triangulares.
- **Ordenação Anti-Horária (CCW):** A ordem de indexação $(V_0 \to V_1 \to V_2)$ define a face frontal através da regra da mão direita. Triângulos visualizados por trás são descartados por *Back-Face Culling*.

```xml
<!-- Malha Triangular de um Plano com Coordenadas UV de Textura -->
<MeshGeometry3D Positions="-1,0,-1  1,0,-1  1,0,1  -1,0,1"
                TriangleIndices="0,1,2  0,2,3"
                TextureCoordinates="0,0  4,0  4,4  0,4"/>
```

---

## 2. Câmeras: PerspectiveCamera e Projeção

A classe `PerspectiveCamera` simula o olho humano e lentes ópticas:
- **`Position`:** Localização do observador no espaço $(X, Y, Z)$.
- **`LookDirection`:** Vetor direcional que aponta para onde a câmera está olhando.
- **`UpDirection`:** Vetor vertical de orientação (usualmente $(0, 1, 0)$).
- **`FieldOfView`:** Ângulo do cone de visão em graus (ex: $60^\circ$).

### Câmera Orbital Arcball com Coordenadas Esféricas
A câmera orbita ao redor da cena utilizando parametrização esférica:
- **Distância ($r$):** Controlada pelo *Scroll* do mouse.
- **Ângulo Azimutal ($\theta$):** Rotação horizontal de $0^\circ$ a $360^\circ$.
- **Ângulo Polar ($\phi$):** Elevação vertical de $-89^\circ$ a $+89^\circ$.

$$
X = r \cdot \cos(\phi) \cdot \sin(\theta), \quad Y = r \cdot \sin(\phi), \quad Z = r \cdot \cos(\phi) \cdot \cos(\theta)
$$

---

## 3. Iluminação Difusa e a Lei de Lambert

Para iluminar superfícies foscas (como pedra, madeira ou argamassa), o motor 3D aplica a **Lei de Reflexão de Lambert**:

$$
I_{\text{difusa}} = I_{\text{luz}} \cdot k_d \cdot \max(0, \; \vec{N} \cdot \vec{L})
$$

Onde $\vec{N}$ é a normal unitária da superfície e $\vec{L}$ é a direção da luz incidente. O produto escalar $\vec{N} \cdot \vec{L}$ calcula o cosseno do ângulo de incidência da luz.

### Sistema Solar com Duas Luzes Direcionais
Em cenas arquitetônicas abertas, combinam-se duas instâncias de `DirectionalLight` com inclinação de $30^\circ$ e rotação oposta para manter ambas as fachadas iluminadas sem sombras pretas absolutas.

---

## 4. Repetição de Texturas com TileMode="Tile"

Para aplicar texturas de piso e parede sem esticar a imagem, utiliza-se `ImageBrush` com repetição em mosaico:

```xml
<DiffuseMaterial>
    <DiffuseMaterial.Brush>
        <ImageBrush ImageSource="Assets/granite.png"
                    TileMode="Tile"
                    Viewport="0,0,0.25,0.25"
                    ViewportUnits="Relative"/>
    </DiffuseMaterial.Brush>
</DiffuseMaterial>
```

---

<div class="ms-ref-card">
  <h4>Referências Oficiais Microsoft Learn</h4>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.controls.viewport3d" target="_blank" rel="noopener">Classe Viewport3D (System.Windows.Controls)</a> — Container visual para renderização de cenas 3D no espaço de interface 2D.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.perspectivecamera" target="_blank" rel="noopener">Classe PerspectiveCamera</a> — Configuração de posição, vetor LookDirection, UpDirection e FieldOfView.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.tilebrush.tilemode" target="_blank" rel="noopener">Propriedade TileBrush.TileMode</a> — Mapeamento e repetição de texturas no WPF.</li>
  </ul>
</div>

---

**Próximo Passo:** Explore a [Modelagem Hierárquica e Grafos de Cena (Unidade 3)](/hierarquia/grafos-de-cena-e-teoria/).
