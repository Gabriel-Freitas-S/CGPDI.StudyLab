# 🎓 Capítulo 7: Mapeamento Completo do Plano de Ensino

Este capítulo apresenta a matriz de rastreabilidade entre a ementa do curso e as classes do projeto.

---

## Matriz de Rastreabilidade

| Unidade | Conteúdo Programático | Implementação no Código-Fonte | Demonstração na UI |
| :--- | :--- | :--- | :--- |
| **Ementa** | Sistemas Gráficos e Arquitetura de Hardware | [`DirectBitmap.cs`](file:///D:/source/repos/teste/teste/Core/DirectBitmap.cs) | Base de renderização |
| **Ementa** | Estudo da Cor e Modelos Cromáticos | [`ColorSpaces.cs`](file:///D:/source/repos/teste/teste/Core/ColorSpaces.cs) | Aba PDI (Seletor 1) |
| **Ementa** | Algoritmos Elementares para Gráficos 2D | [`Rasterizer2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Rasterizer2D.cs) | Aba CG 2D |
| **Ementa** | Transformações Geométricas 2D | [`Matrix2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Matrix2D.cs) | Aba CG 2D (Seletor 2) |
| **Ementa** | Transformações Geométricas 3D | [`Math3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/Math3D.cs) | Aba CG 3D e Software 3D |
| **Ementa** | Projeções 3D (Perspectiva vs Ortográfica) | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) | Aba CG 3D (Seletor 1) |
| **Ementa** | Curvas e Superfícies Paramétricas | [`Rasterizer2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Rasterizer2D.cs) e [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) | Aba CG 2D e Aba CG 3D |
| **Ementa** | Iluminação e Sombra (Blinn-Phong) | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) e [`Raytracer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/Raytracer3D.cs) | Aba CG 3D e Software 3D |
| **Ementa** | Determinação de Superfícies Visíveis | [`SoftwareRenderer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/SoftwareRenderer3D.cs) | Aba Software 3D |
| **Ementa** | Modelagem de Sólidos e Malhas | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) | Aba CG 3D (Seletor 2) |
| **Unidade 1** | Pipeline Gráfico Completo | [`SoftwareRenderer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/SoftwareRenderer3D.cs) | Aba Software 3D |
| **Unidade 1** | Interações em Ambientes Gráficos | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) | Aba CG 3D (Arcball Orbit/Zoom) |
| **Unidade 1** | Renderização Realística (Ray Tracing) | [`Raytracer3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/Raytracer3D.cs) | Aba Software 3D (Seletor 2) |
| **Unidade 1** | Gráficos 2D no WPF | [`Rasterizer2D.cs`](file:///D:/source/repos/teste/teste/Graphics2D/Rasterizer2D.cs) | Aba CG 2D |
| **Unidade 2** | Coordenadas 3D e Transformações | [`Math3D.cs`](file:///D:/source/repos/teste/teste/Graphics3D/Math3D.cs) | Todas as abas 3D |
| **Unidade 2** | Construção de Malhas Triangulares | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) (`MeshGeometry3D`) | Aba CG 3D |
| **Unidade 2** | Representação de Câmeras | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) (`PerspectiveCamera`/`OrthographicCamera`) | Aba CG 3D |
| **Unidade 2** | Iluminação da Cena | [`WpfViewport3DManager.cs`](file:///D:/source/repos/teste/teste/Graphics3D/WpfViewport3DManager.cs) (`DirectionalLight`, `PointLight`, `AmbientLight`) | Aba CG 3D |
| **Unidade 3** | Grafos de Cena & Modelagem Hierárquica | [`HierarchicalModeling.cs`](file:///D:/source/repos/teste/teste/Graphics3D/HierarchicalModeling.cs) | Aba CG 3D (Seletor 3) |
| **Unidade 3** | Design Top-Down e Bottom-Up | [`HierarchicalModeling.cs`](file:///D:/source/repos/teste/teste/Graphics3D/HierarchicalModeling.cs) | Aba CG 3D |
| **Unidade 3** | Cinemática Direta & Robô Articulado | [`HierarchicalModeling.cs`](file:///D:/source/repos/teste/teste/Graphics3D/HierarchicalModeling.cs) | Aba CG 3D (Sliders + Animação) |
| **Unidade 3** | Animação Contínua e Reusabilidade | [`MainWindow.xaml.cs`](file:///D:/source/repos/teste/teste/MainWindow.xaml.cs) | Aba CG 3D |
