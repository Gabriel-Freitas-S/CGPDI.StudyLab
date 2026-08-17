---
title: Mapeamento Curricular dos Módulos
description: Relação estruturada entre os tópicos pedagógicos de Computação Gráfica / PDI e as classes do projeto.
---

O **CGPDI.StudyLab** foi desenvolvido para cobrir de forma modular e prática todos os tópicos curriculares essenciais de Computação Gráfica e Processamento Digital de Imagens.

---

## Tabela de Correspondência dos Módulos

| Tópico Pedagógico | Localização no Código | Módulo na Documentação |
| :--- | :--- | :--- |
| **Apresentação do Pipeline Gráfico** | `Graphics3D/Math3D.cs` e `SoftwareRenderer3D.cs` | [Matemática 3D & Matrizes MVP](/cg3d/matematica-vetorial-e-mvp/) |
| **Sistemas Gráficos (Hardware e Arquitetura)** | `Core/DirectBitmap.cs` e `WpfViewport3DManager.cs` | [Fundamentos de Memória & DirectBitmap](/core/fundamentos-de-memoria/) |
| **Algoritmos Elementares para Gráficos 2D** | `Graphics2D/Rasterizer2D.cs` | [Algoritmos de Reta (Bresenham & Wu)](/cg2d/algoritmos-de-linhas/) |
| **Transformações Geométricas 2D** | `Graphics2D/Matrix2D.cs` e `ImageProcessing/GeometricTransforms.cs` | [Álgebra Linear 2D & Coordenadas Homogêneas](/cg2d/algebra-linear-e-matrizes/) |
| **Construção de Aplicações Gráficas 2D** | `MainWindow.xaml` (Aba CG 2D) | [WPF, XAML e Renderização](/arquitetura/wpf-e-xaml-explicados/) |
| **Transformações Geométricas 3D** | `Graphics3D/Math3D.cs` (Matrizes $4 \times 4$ e Quaternions) | [Matemática 3D & MVP](/cg3d/matematica-vetorial-e-mvp/) |
| **Visualização em 3D – Projeções** | `Graphics3D/Math3D.cs` (Perspectiva com FOV vs Ortográfica) | [Matemática 3D & MVP](/cg3d/matematica-vetorial-e-mvp/) |
| **Representação de Curvas e Superfícies** | `Graphics2D/Rasterizer2D.cs` (Bézier) e `Graphics3D/SoftwareRenderer3D.cs` | [Círculos, Elipses & Curvas de Bézier](/cg2d/circulos-elipses-e-curvas/) |
| **Estudo da Cor & Modelos de Cor** | `Core/ColorSpaces.cs` (RGB, HSV, YCbCr, CMYK, BT.709) | [Modelos de Cor & Percepção Humana](/core/modelos-de-cor/) |
| **Iluminação e Sombra** | `Graphics3D/Raytracer3D.cs` e `WpfViewport3DManager.cs` | [Fundamentos do Ray Tracing & Modelo Phong](/raytracing/fundamentos-e-fisica-da-luz/) |
| **Determinação de Superfícies Visíveis** | `Graphics3D/SoftwareRenderer3D.cs` (Z-Buffer & Back-face Culling) | [Renderizador em Software 3D](/cg3d/renderizador-em-software/) |
| **Modelagem de Sólidos & Malhas Triangulares** | `Graphics3D/SoftwareRenderer3D.cs` (Mesh3D: Cubo, Esfera, Pirâmide) | [Renderizador em Software 3D](/cg3d/renderizador-em-software/) |
| **Modelagem Hierárquica & Grafos de Cena** | `Graphics3D/HierarchicalModeling.cs` (Robô 4-DOF e Sistema Solar) | [Grafos de Cena & Cinemática Direta](/hierarquia/grafos-de-cena-e-teoria/) |
| **Renderização Realística (Ray Tracing)** | `Graphics3D/Raytracer3D.cs` (Reflexões, Snell, Sombras) | [Reflexões Especulares & Refração de Snell](/raytracing/reflexao-refracao-snell/) |
| **Processamento Digital de Imagens (PDI)** | Diretório `ImageProcessing/` completo | [Módulo Completo de PDI](/pdi/operacoes-pontuais-e-histogramas/) |

---

<div class="ms-ref-card">
  <h4>Referências Oficiais Microsoft Learn</h4>
  <p>Portais e materiais de referência recomendados para o aprendizado:</p>
  <ul>
    <li><a href="https://learn.microsoft.com/pt-br/training/dotnet/" target="_blank" rel="noopener">Roteiros de Aprendizagem do .NET no Microsoft Learn</a> — Cursos e trilhas gratuitas do iniciante ao avançado.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/csharp/" target="_blank" rel="noopener">Documentação da Linguagem C#</a> — Guia completo de referência do compilador Roslyn e especificações da linguagem.</li>
    <li><a href="https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/" target="_blank" rel="noopener">Documentação do Windows Presentation Foundation (WPF)</a> — Referência completa para criação de interfaces gráficas para Windows.</li>
  </ul>
</div>

---

**Próximo Passo:** Veja o [Roteiro Integrado de Estudos e Práticas](/academico/roteiro-de-estudos-e-avaliacoes/).
