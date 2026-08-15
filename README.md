# 🎓 CGPDI.StudyLab — Laboratório Universitário de Computação Gráfica & PDI (.NET 10 WPF)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?logo=c-sharp)](https://docs.microsoft.com/dotnet/csharp/)
[![WPF DirectX](https://img.shields.io/badge/UI-WPF%20%2F%20DirectX-0078D6?logo=windows)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![Astro Starlight](https://img.shields.io/badge/Docs-Astro%20Starlight-FF5D01?logo=astro)](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/)
[![GitHub Pages](https://img.shields.io/badge/Deploy-GitHub%20Pages-22c55e?logo=github)](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Um ecossistema educacional e prático de alta performance para o estudo de **Processamento Digital de Imagens (PDI)**, **Computação Gráfica 2D (Rasterização dos Primeiros Princípios)**, **Computação Gráfica 3D (Pipeline em Software e Aceleração por Hardware)**, **Modelagem Hierárquica / Cinemática Direta** e **Renderização Realística (Ray Tracing)**.

Desenvolvido integralmente em conformidade com o **Plano de Ensino Universitário** para cursos de Bacharelado em Ciência/Engenharia da Computação.

---

## 🌐 📖 Site Oficial de Documentação & Wiki

Acesse a documentação completa, interativa e detalhada no GitHub Pages:

👉 **[https://gabriel-freitas-s.github.io/CGPDI.StudyLab/](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/)**

### 📑 Sumário da Documentação no Astro Starlight (`docs/`):

1. 🚀 **[Começando do Zero (Guia para Iniciantes)](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/iniciantes/o-que-e-dotnet-csharp/)**: O que é C#, .NET 10, WPF, como instalar o Visual Studio passo a passo, executar pelo terminal e depurar com Breakpoints.
2. 🏗️ **[Arquitetura do Software](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/arquitetura/visao-geral/)**: Camadas, fluxo de dados, estrutura de pastas e integração DirectX.
3. 🧠 **[Núcleo de Memória & Hardware](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/core/fundamentos-de-memoria/)**: `DirectBitmap`, ponteiros `unsafe byte*`, formato `Bgra32`, `Stride` e modelos de cores (`ColorSpaces.cs`).
4. 🖼️ **[Processamento Digital de Imagens (PDI)](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/pdi/operacoes-pontuais-e-histogramas/)**: Operações pontuais, histogramas e CDF, convoluções espaciais 2D, Canny em 5 etapas, morfologia matemática (Otsu), transformações geométricas e Transformada de Fourier 2D (DFT).
5. ✏️ **[Computação Gráfica 2D](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/cg2d/algebra-linear-e-matrizes/)**: Álgebra linear homogênea 3x3, retas de Bresenham e Xiaolin Wu, círculo/elipse do ponto médio, curvas de Bézier e preenchimento Scanline.
6. 🧊 **[Computação Gráfica 3D](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/cg3d/matematica-vetorial-e-mvp/)**: Pipeline MVP, renderizador em software na CPU com Z-Buffer baricêntrico e Viewport3D DirectX com câmera Arcball.
7. 🤖 **[Modelagem Hierárquica](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/hierarquia/grafos-de-cena-e-teoria/)**: Grafos de cena (Scene Graphs), propagação de matrizes pai-filho e braço robótico com cinemática direta.
8. ⚡ **[Ray Tracing Realístico](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/raytracing/fundamentos-e-fisica-da-luz/)**: Whitted Ray Tracer, interseção analítica raio-esfera, sombras nítidas, reflexões e refração com a Lei de Snell e Fresnel.
9. 🎓 **[Guia Acadêmico](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/academico/mapeamento-do-plano/)**: Mapeamento completo da Ementa e roteiro de estudos para os trabalhos T1, T2 e T3.
10. 🌐 **[Publicação & GitHub Pages](https://gabriel-freitas-s.github.io/CGPDI.StudyLab/deploy/github-pages-e-ci-cd/)**: Como ativar o GitHub Pages com GitHub Actions.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior instalado.
* Sistema Operacional Windows 10/11 (necessário para o subsistema WPF / DirectX).

### Linha de Comando (PowerShell / Terminal)
```powershell
# 1. Clone ou navegue até o repositório
cd D:\source\repos\CGPDI.StudyLab\CGPDI.StudyLab

# 2. Restaure as dependências e compile
dotnet build

# 3. Execute a aplicação
dotnet run
```

### Executar a Documentação Localmente (Astro Starlight)
```powershell
cd docs
npm install
npm run dev
# Acesse http://localhost:4321/CGPDI.StudyLab/
```

---

## 🌟 Funcionalidades e Módulos do Sistema

```
📦 Solução CGPDI.StudyLab
├── 📂 docs/                     -> Site oficial de documentação em Astro Starlight
├── 📂 .github/workflows/        -> Automação de CI/CD para o GitHub Pages
├── 📂 CGPDI.StudyLab/
│   ├── 📂 Core/
│   │   ├── DirectBitmap.cs          -> Manipulação de memória direta via ponteiros (unsafe byte*) e Stride
│   │   ├── ColorSpaces.cs           -> Modelos de cores (RGB, HSV, HSL, YCbCr, CMYK, BT.709, BT.601, Sépia)
│   │   ├── ImageSampleGenerator.cs  -> Gerador procedual de cenas de calibração ótica e testes de PDI
│   │   └── StudyGuideData.cs        -> Central de estudos embutida com teoria e códigos comentados
│   ├── 📂 ImageProcessing/
│   │   ├── PointAndHistograms.cs    -> Brilho, contraste, Gamma, Equalização CDF, Normalização Min-Max
│   │   ├── SpatialFilters.cs        -> Convoluções 2D, Gaussian, Unsharp Mask, Mediana, Canny de 5 etapas
│   │   ├── Morphology.cs            -> Binarização de Otsu, Erosão, Dilatação, Abertura, Fechamento
│   │   ├── GeometricTransforms.cs   -> Mapeamento Inverso, Interpolação Bilinear/Bicúbica, Swirl, Ripple, Fisheye
│   │   └── FrequencyAndProcedural.cs-> DFT 2D + FFTShift, Perlin Noise, Terrenos fBm, Voronoi, Mandelbrot/Julia
│   ├── 📂 Graphics2D/
│   │   ├── Matrix2D.cs              -> Álgebra Linear 2D (Matrizes 3x3 Homogêneas, Composição Afim)
│   │   └── Rasterizer2D.cs          -> Bresenham, DDA, Wu Anti-aliased, Círculo/Elipse Ponto Médio, Bézier, Scanline
│   ├── 📂 Graphics3D/
│   │   ├── Math3D.cs                -> Vetores 3D/4D, Matrizes 4x4 MVP, Quaternions, Raios
│   │   ├── SoftwareRenderer3D.cs    -> Pipeline 3D CPU (Back-face Culling, Z-Buffer, Baricêntricas)
│   │   ├── WpfViewport3DManager.cs  -> Viewport3D Hardware (Câmera Orbital Arcball, Projeções, Phong)
│   │   ├── HierarchicalModeling.cs  -> Grafo de Cena (Scene Graph) e Robô Articulado com Cinemática Direta
│   │   └── Raytracer3D.cs           -> Ray Tracer de Whitted (Sombras, Reflexões, Refração Snell/Fresnel)
│   └── 📂 UI/
│       ├── MainWindow.xaml          -> Interface escura em alta resolução, responsiva em tela cheia
│       └── MainWindow.xaml.cs       -> Controlador de eventos e cronômetro de alta precisão
```

---

## 🎯 Destaques de Engenharia e Desempenho

| Métrica / Técnica | Implementação no Projeto | Benefício |
| :--- | :--- | :--- |
| **Acesso Direto à Memória** | `unsafe byte*` sobre buffer `Bgra32` | Elimina cópias intermediárias e overhead do GDI+ |
| **Paralelismo Multinúcleo** | `Parallel.For(0, Height, y => ...)` | Processamento em tempo real a **60+ FPS** em CPU moderna |
| **Câmera Orbital Arcball** | Coordenadas esféricas ($r, \theta, \phi$) | Rotação suave de 360° com arrasto de mouse e zoom óptico |
| **Renderizador em Software** | Pipeline gráfico completo do zero em C# | Compreensão total da GPU sem abstrações opacas |
| **Ray Tracer Físico** | Resolução analítica de equações quadráticas | Reflexões fotorrealistas e refração em vidro com Lei de Snell |

---

## 📄 Licença

Este projeto é disponibilizado sob a licença [MIT](./LICENSE), sendo livre para fins educacionais, acadêmicos e comerciais.
