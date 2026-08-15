---
title: Visão Geral da Arquitetura do Software
description: Como os módulos de memória, PDI, 2D, 3D e interface gráfica se conectam para atingir 60+ FPS em tempo real.
---

O **CGPDI.StudyLab** foi arquitetado seguindo o padrão de **Engenharia de Software em Camadas (Layered Architecture)**, separando rigidamente os cálculos matemáticos puros da renderização de tela e da interface com o usuário.

---

## 🏛️ Diagrama em Camadas

```mermaid
graph TB
    subgraph UI_Layer["🎨 Camada de Interface de Usuário (WPF)"]
        MainWindowXAML["MainWindow.xaml\n(Layout, Sliders, Controles, Viewport3D)"]
        MainWindowCS["MainWindow.xaml.cs\n(Controlador de Eventos & Cronômetro de Performance)"]
    end

    subgraph Service_Layer["🧮 Camada de Algoritmos & Processamento"]
        PDI["ImageProcessing/\n(Point, Filters, Morphology, Geometry, Frequency)"]
        G2D["Graphics2D/\n(Matrix2D, Rasterizer2D)"]
        G3D["Graphics3D/\n(Math3D, SoftwareRenderer3D, Raytracer3D, HierarchicalModeling)"]
    end

    subgraph Core_Layer["🧠 Camada Núcleo de Alta Performance"]
        DirectBitmap["DirectBitmap.cs\n(unsafe byte*, Bgra32, Stride, Lock/Unlock)"]
        ColorSpaces["ColorSpaces.cs\n(RGB, HSV, YCbCr, CMYK, BT.709/BT.601)"]
        SampleGen["ImageSampleGenerator.cs\n(Padrões de Teste Ótico)"]
    end

    subgraph Hardware_Layer["⚡ Camada de Aceleração de Hardware"]
        DirectX["DirectX 9/11 / Driver de GPU\n(Exibição na Tela em Tempo Real)"]
        MultiCore["CPU Multi-Core SIMD (Parallel.For)"]
    end

    MainWindowXAML --> MainWindowCS
    MainWindowCS --> Service_Layer
    Service_Layer --> Core_Layer
    Core_Layer --> MultiCore
    Core_Layer --> DirectX
```

---

## 🔄 Fluxo de Execução de um Algoritmo

Quando você clica em um botão na interface (por exemplo, aplicar o filtro **Gaussiano** ou renderizar uma **Cena 3D**):

1. **Disparo do Evento:** O usuário interage com um botão ou arrasta um slider no `MainWindow.xaml`.
2. **Coleta de Parâmetros:** O método manipulador em `MainWindow.xaml.cs` lê os valores dos sliders (como $\sigma = 1.5$ ou raio $r = 3$).
3. **Início do Cronômetro:** Um cronômetro de precisão nanométrica (`System.Diagnostics.Stopwatch`) é iniciado.
4. **Processamento Paralelo em Memória:**
   - O algoritmo correspondente (ex: `SpatialFilters.ApplyGaussianBlur`) é chamado.
   - O `DirectBitmap` de origem e destino têm seus buffers bloqueados com `.Lock()`.
   - O comando `Parallel.For` distribui as linhas da imagem entre todos os núcleos da CPU.
   - Nenhum objeto novo é alocado no laço para evitar trabalho ao *Garbage Collector*.
5. **Notificação de Redesenho:** O `DirectBitmap.Unlock(markDirty: true)` avisa o WPF que os pixels mudaram através de `AddDirtyRect`.
6. **Aceleração por DirectX:** O WPF envia o buffer `Bgra32` diretamente para a memória da placa de vídeo para exibição instantânea.
7. **Exibição do Tempo:** O cronômetro é parado e a interface exibe o tempo exato em milissegundos (ex: `⏱️ 1.2 ms (833 FPS)`).

---

## ⚡ Por que a Aplicação é Tão Rápida?

Três princípios de engenharia garantem que o laboratório rode com taxas de centenas de quadros por segundo:

1. **Acesso Unsafe a Ponteiros:** Elimina validações redundantes de checagem de limites feitas pelo C# padrão a cada pixel.
2. **Localidade de Cache da CPU:** O acesso à memória é sempre feito de forma estritamente sequencial linha a linha (horizontalmente), garantindo taxa máxima de acerto no cache L1/L2/L3 do processador.
3. **Zero Alocação no Laço Crítico:** Variáveis auxiliares são mantidas na pilha de execução (*Stack*) e não no *Heap*, mantendo a coleta de lixo em zero durante animações.

---

👉 **Próximo Passo:** Explore a [Estrutura Detalhada de Pastas e Arquivos](/CGPDI.StudyLab/arquitetura/estrutura-de-pastas/).
