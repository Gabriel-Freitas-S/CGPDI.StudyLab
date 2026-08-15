---
title: Viewport3D por Hardware & Câmera Arcball (WpfViewport3DManager.cs)
description: Renderização DirectX acelerada por GPU no WPF, câmera orbital esférica com controle por mouse, fontes de luz e materiais Phong.
---

O arquivo [`WpfViewport3DManager.cs`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/blob/main/CGPDI.StudyLab/Graphics3D/WpfViewport3DManager.cs) gerencia a visualização de cenas tridimensionais em tempo real usando a aceleração de hardware nativa do WPF via **DirectX**.

---

## 🌐 1. A Câmera Orbital Arcball (Controle por Mouse)

Uma **Câmera Arcball** permite ao usuário "orbitar" suavemente ao redor do objeto 3D clicando e arrastando com o mouse, mantendo o foco sempre no centro da cena.

### A Matemática das Coordenadas Esféricas:
A posição $(X, Y, Z)$ da câmera é calculada a partir de três parâmetros esféricos:
- **Raio / Distância ($r$):** Controlado pelo *Scroll* do mouse (Zoom In / Zoom Out).
- **Ângulo Azimutal ($\theta$ - Theta):** Rotação horizontal de $0^\circ$ a $360^\circ$.
- **Ângulo Polar ($\phi$ - Phi):** Elevação vertical de $-89^\circ$ a $+89^\circ$.

$$
X = r \cdot \cos(\phi) \cdot \sin(\theta)
$$
$$
Y = r \cdot \sin(\phi)
$$
$$
Z = r \cdot \cos(\phi) \cdot \cos(\theta)
$$

```csharp
// Atualização em tempo real da Câmera do WPF:
public void UpdateCamera()
{
    double radTheta = _theta * Math.PI / 180.0;
    double radPhi   = _phi   * Math.PI / 180.0;

    double x = _distance * Math.Cos(radPhi) * Math.Sin(radTheta);
    double y = _distance * Math.Sin(radPhi);
    double z = _distance * Math.Cos(radPhi) * Math.Cos(radTheta);

    _camera.Position = new Point3D(x, y, z);
    _camera.LookDirection = new Vector3D(-x, -y, -z);
    _camera.UpDirection = new Vector3D(0, 1, 0);
}
```

---

## 💡 2. Tipos de Luzes do WPF

O gerenciador permite alternar e combinar três fontes luminosas:

1. **`AmbientLight` (Luz Ambiente):** Luz difusa e onipresente que ilumina uniformemente todas as superfícies, simulando a luz rebatida do ambiente sem direção específica.
2. **`DirectionalLight` (Luz Direcional):** Raios de luz paralelos que vêm de uma direção fixa no infinito (como a luz do Sol).
3. **`PointLight` (Luz Pontual):** Luz que irradia em todas as direções a partir de um ponto específico no espaço $(X, Y, Z)$, com atenuação de intensidade pela distância.

---

## 🎨 3. Materiais e Iluminação de Phong

As malhas 3D recebem materiais compostos por dois componentes do modelo de reflexão de **Bui Tuong Phong**:

1. **`DiffuseMaterial` (Componente Difusa - Lei do Cosseno de Lambert):**
   - A luz é refletida igualmente em todas as direções.
   - Intensidade: $I_d = k_d (\vec{N} \cdot \vec{L})$.
2. **`SpecularMaterial` (Componente Especular - Brilho Refletivo):**
   - Simula o "ponto de brilho metálico ou plástico" (*Highlight*).
   - Intensidade: $I_s = k_s (\vec{R} \cdot \vec{V})^\alpha$, onde $\alpha$ é o expoente de brilho (*Shininess*).

---

👉 **Próximo Passo:** Explore a [Modelagem Hierárquica e Grafos de Cena (Unidade 3)](/CGPDI.StudyLab/hierarquia/grafos-de-cena-e-teoria/).
