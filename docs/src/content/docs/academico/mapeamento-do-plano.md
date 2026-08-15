---
title: Mapeamento do Plano de Ensino Universitário
description: Relação completa 1:1 entre a Ementa Oficial da disciplina de Computação Gráfica / PDI e as classes do projeto.
---

O **CGPDI.StudyLab** foi integralmente construído para cobrir todas as competências, habilidades e unidades de ensino do curso de Bacharelado em Ciência/Engenharia da Computação.

Abaixo você encontra a tabela de correspondência exata entre os tópicos da ementa e os arquivos de código-fonte.

---

## 📑 Tabela de Correspondência da Ementa Oficial

| Tópico da Ementa Oficial | Onde Encontrar no Código? | Módulo na Documentação |
| :--- | :--- | :--- |
| **Apresentação do Pipeline Gráfico** | `Graphics3D/Math3D.cs` & `SoftwareRenderer3D.cs` | [Matemática 3D & Matrizes MVP](/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/) |
| **Sistemas Gráficos (Hardware e Arquitetura)** | `Core/DirectBitmap.cs` & `WpfViewport3DManager.cs` | [Fundamentos de Memória & DirectBitmap](/CGPDI.StudyLab/core/fundamentos-de-memoria/) |
| **Algoritmos Elementares para Gráficos 2D** | `Graphics2D/Rasterizer2D.cs` | [Algoritmos de Reta (Bresenham & Wu)](/CGPDI.StudyLab/cg2d/algoritmos-de-linhas/) |
| **Transformações Geométricas 2D** | `Graphics2D/Matrix2D.cs` & `ImageProcessing/GeometricTransforms.cs` | [Álgebra Linear 2D & Coordenadas Homogêneas](/CGPDI.StudyLab/cg2d/algebra-linear-e-matrizes/) |
| **Construção de Aplicações Gráficas 2D** | `MainWindow.xaml` (Aba CG 2D) | [WPF, XAML e Renderização](/CGPDI.StudyLab/arquitetura/wpf-e-xaml-explicados/) |
| **Transformações Geométricas 3D** | `Graphics3D/Math3D.cs` (Matrizes $4 \times 4$ e Quaternions) | [Matemática 3D & MVP](/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/) |
| **Visualização em 3D – Projeções** | `Graphics3D/Math3D.cs` (Perspectiva com FOV vs Ortográfica) | [Matemática 3D & MVP](/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/) |
| **Representação de Curvas e Superfícies** | `Graphics2D/Rasterizer2D.cs` (Bézier) & `Graphics3D/SoftwareRenderer3D.cs` | [Círculos, Elipses & Curvas de Bézier](/CGPDI.StudyLab/cg2d/circulos-elipses-e-curvas/) |
| **Estudo da Cor & Modelos de Cor** | `Core/ColorSpaces.cs` (RGB, HSV, YCbCr, CMYK, BT.709) | [Modelos de Cor & Percepção Humana](/CGPDI.StudyLab/core/modelos-de-cor/) |
| **Iluminação e Sombra** | `Graphics3D/Raytracer3D.cs` & `WpfViewport3DManager.cs` | [Fundamentos do Ray Tracing & Modelo Phong](/CGPDI.StudyLab/raytracing/fundamentos-e-fisica-da-luz/) |
| **Determinação de Superfícies Visíveis** | `Graphics3D/SoftwareRenderer3D.cs` (Z-Buffer & Back-face Culling) | [Renderizador em Software 3D](/CGPDI.StudyLab/cg3d/renderizador-em-software/) |
| **Modelagem de Sólidos & Malhas Triangulares** | `Graphics3D/SoftwareRenderer3D.cs` (Mesh3D: Cubo, Esfera, Pirâmide) | [Renderizador em Software 3D](/CGPDI.StudyLab/cg3d/renderizador-em-software/) |
| **Modelagem Hierárquica & Grafos de Cena** | `Graphics3D/HierarchicalModeling.cs` (Robô 4-DOF e Sistema Solar) | [Grafos de Cena & Cinemática Direta](/CGPDI.StudyLab/hierarquia/grafos-de-cena-e-teoria/) |
| **Renderização Realística (Ray Tracing)** | `Graphics3D/Raytracer3D.cs` (Reflexões, Snell, Sombras) | [Reflexões Especulares & Refração de Snell](/CGPDI.StudyLab/raytracing/reflexao-refracao-snell/) |
| **Processamento Digital de Imagens (PDI)** | Pasta `ImageProcessing/` completa | [Módulo Completo de PDI](/CGPDI.StudyLab/pdi/operacoes-pontuais-e-histogramas/) |

---

## 📚 Bibliografia Recomendada Coberta pelo Projeto

1. **HUGHES, John F. et al.** *Computer Graphics: Principles and Practice*. 3ª Edição, Addison-Wesley, 2013.
2. **GONZALEZ, Rafael C.; WOODS, Richard E.** *Digital Image Processing*. 4ª Edição, Pearson, 2018.
3. **FOLEY, James D. et al.** *Computer Graphics: Principles and Practice in C*. 2ª Edição, Addison-Wesley, 1997.
4. **HEARN, Donald; BAKER, M. Pauline.** *Computer Graphics with OpenGL*. 3ª Edição, Prentice-Hall, 2011.
5. **SHREINER, Dave et al.** *OpenGL Programming Guide: The Official Guide to Learning OpenGL*. Addison-Wesley.

---

👉 **Próximo Passo:** Veja o [Roteiro de Estudos para os Trabalhos T1, T2 e T3](/CGPDI.StudyLab/academico/roteiro-de-estudos-e-avaliacoes/).
