# 🎓 CGPDI.StudyLab — Laboratório Universitário de Computação Gráfica & PDI (.NET 10 WPF)

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?logo=c-sharp)](https://docs.microsoft.com/dotnet/csharp/)
[![WPF DirectX](https://img.shields.io/badge/UI-WPF%20%2F%20DirectX-0078D6?logo=windows)](https://learn.microsoft.com/dotnet/desktop/wpf/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Um ecossistema educacional e prático de alta performance para o estudo de **Processamento Digital de Imagens (PDI)**, **Computação Gráfica 2D (Rasterização dos Primeiros Princípios)**, **Computação Gráfica 3D (Pipeline em Software e Aceleração por Hardware)**, **Modelagem Hierárquica / Cinemática Direta** e **Renderização Realística (Ray Tracing)**.

Desenvolvido integralmente em conformidade com o **Plano de Ensino Universitário** para cursos de Bacharelado em Ciência/Engenharia da Computação.

---

## 📑 Sumário da Wiki & Documentação

Toda a documentação teórica e explicações passo a passo do código estão organizadas na pasta [`wiki/`](./wiki/):

1. [**Home & Visão Geral da Arquitetura**](./wiki/Home.md)
2. [**1. Fundamentos de Memória & DirectBitmap**](./wiki/1-Fundamentos-e-Manipulacao-de-Memoria.md)
3. [**2. Processamento Digital de Imagens (PDI Completo)**](./wiki/2-Processamento-Digital-de-Imagens.md)
4. [**3. Computação Gráfica 2D & Rasterização**](./wiki/3-Computacao-Grafica-2D-e-Rasterizacao.md)
5. [**4. Computação Gráfica 3D & Pipeline Gráfico**](./wiki/4-Computacao-Grafica-3D-e-Pipeline.md)
6. [**5. Modelagem Hierárquica & Cinemática Direta**](./wiki/5-Modelagem-Hierarquica-e-Cinematica-Direta.md)
7. [**6. Ray Tracing & Renderização Realística**](./wiki/6-Ray-Tracing-e-Renderizacao-Realistica.md)
8. [**7. Mapeamento Completo do Plano de Ensino**](./wiki/7-Mapeamento-do-Plano-de-Ensino.md)

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) ou superior instalado.
* Sistema Operacional Windows 10/11 (necessário para o subsistema WPF / DirectX).

### Linha de Comando (PowerShell / Terminal)
```powershell
# 1. Clone ou navegue até o repositório
cd D:\source\repos\teste\CGPDI.StudyLab

# 2. Restaure as dependências e compile
dotnet build

# 3. Execute a aplicação
dotnet run
```

---

## 🌟 Funcionalidades e Módulos do Sistema

```
📦 Solução CGPDI.StudyLab
├── 📂 Core/
│   ├── DirectBitmap.cs          -> Manipulação de memória direta via ponteiros (unsafe byte*) e Stride
│   ├── ColorSpaces.cs           -> Modelos de cores (RGB, HSV, HSL, YCbCr, CMYK, BT.709, BT.601, Sépia)
│   ├── ImageSampleGenerator.cs  -> Gerador procedual de cenas de calibração ótica e testes de PDI
│   └── StudyGuideData.cs        -> Central de estudos embutida com teoria e códigos comentados
├── 📂 ImageProcessing/
│   ├── PointAndHistograms.cs    -> Brilho, contraste, Gamma, Equalização CDF, Normalização Min-Max
│   ├── SpatialFilters.cs        -> Convoluções 2D, Gaussian, Unsharp Mask, Mediana, Canny de 5 etapas
│   ├── Morphology.cs            -> Binarização de Otsu, Erosão, Dilatação, Abertura, Fechamento
│   ├── GeometricTransforms.cs   -> Mapeamento Inverso, Interpolação Bilinear/Bicúbica, Swirl, Ripple, Fisheye
│   └── FrequencyAndProcedural.cs-> DFT 2D + FFTShift, Perlin Noise, Terrenos fBm, Voronoi, Mandelbrot/Julia
├── 📂 Graphics2D/
│   ├── Matrix2D.cs              -> Álgebra Linear 2D (Matrizes 3x3 Homogêneas, Composição Afim)
│   └── Rasterizer2D.cs          -> Bresenham, DDA, Wu Anti-aliased, Círculo/Elipse Ponto Médio, Bézier, Scanline
├── 📂 Graphics3D/
│   ├── Math3D.cs                -> Vetores 3D/4D, Matrizes 4x4 MVP, Quaternions, Raios
│   ├── SoftwareRenderer3D.cs    -> Pipeline 3D CPU (Back-face Culling, Z-Buffer, Baricêntricas)
│   ├── WpfViewport3DManager.cs  -> Viewport3D Hardware (Câmera Orbital Arcball, Projeções, Phong)
│   ├── HierarchicalModeling.cs  -> Grafo de Cena (Scene Graph) e Robô Articulado com Cinemática Direta
│   └── Raytracer3D.cs           -> Ray Tracer de Whitted (Sombras, Reflexões, Refração Snell/Fresnel)
└── 📂 UI/
    ├── MainWindow.xaml          -> Interface escura em alta resolução, responsiva em tela cheia
    └── MainWindow.xaml.cs       -> Controlador de eventos e cronômetro de alta precisão
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
