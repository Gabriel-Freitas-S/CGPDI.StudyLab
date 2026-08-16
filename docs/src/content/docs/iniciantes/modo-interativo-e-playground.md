---
title: Modo Interativo & Laboratório Guiado (C# e WPF Passo a Passo)
description: Guia completo da esteira pedagógica interativa para aprender e revisar conceitos de C#, WPF, memória e computação gráfica de forma progressiva.
---

O **Laboratório Interativo (Aba 6)** do [`CGPDI.StudyLab`](https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab) foi desenvolvido especialmente para estudantes e desenvolvedores que desejam **revisar C# moderno (.NET 10)**, compreender a arquitetura do **WPF (Windows Presentation Foundation)** e praticar conceitos de **Processamento Digital de Imagens e Computação Gráfica** através de experimentação manual e visual.

---

## 🗺️ Como Funciona a Trilha de Aprendizado

A esteira de estudos organiza-se em três pilares integrados:

```mermaid
graph LR
    A["1. Teoria & Referências Microsoft"] --> B["2. Playground Interativo com Sliders"]
    B --> C["3. Simulação Passo a Passo no Canvas"]
    C --> D["4. Quiz de Fixação com Feedback Imediato"]
```

1. **Navegação Progressiva:** Botões `⬅️ Anterior` e `Próximo ➡️` com barra de progresso em tempo real (`Passo X de 10`).
2. **Experimentação Manual:** Controles deslizantes dedicados para alterar parâmetros matemáticos e observar a reação imediata do algoritmo.
3. **Execução Passo a Passo (`▶️`):** O usuário avança ciclo a ciclo para entender a movimentação de ponteiros, kernels e raios 3D.
4. **Quiz de Validação:** Questões de múltipla escolha com explicações pedagógicas sobre a mecânica interna do compilador JIT e da GPU.

---

## 📚 Mapa Curricular das 12 Lições Interativas

| # | Lição | Módulo | Conceito Central |
| :--- | :--- | :--- | :--- |
| **01** | **Bytes & Formato BGRA32** | Memória & C# | Tipos primitivos (`byte`, `uint`), deslocamento de bits e arranjo de 4 bytes por pixel. |
| **02** | **Data Binding & INotifyPropertyChanged** | C# Reativo | Padrão MVVM, eventos de notificação e sincronização bidirecional entre ViewModel e View. |
| **03** | **Ponteiros Não Gerenciados (`unsafe`)** | Memória & C# | Aritmética de ponteiros, cálculo de `Stride` e endereçamento linear `(y * Stride) + (x * 4)`. |
| **04** | **Dependency Properties & Layout WPF** | WPF Internals | Ciclo `MeasureOverride` / `ArrangeOverride` e árvore visual com propriedades de dependência. |
| **05** | **Ciclo de Vida do `WriteableBitmap`** | WPF & DirectX | Sincronização entre CPU e GPU com `Lock()`, manipulação de BackBuffer e `AddDirtyRect()`. |
| **06** | **Convolução Espacial 2D (Box Blur 3x3)** | PDI | Filtros espaciais, matriz de vizinhança $3\times3$, produto convolucional e normalização. |
| **07** | **Limiarização Automática de Otsu** | PDI | Binarização estatística maximizando a variância inter-classes $\sigma_B^2(t)$ em $O(256)$. |
| **08** | **Reta de Bresenham** | CG 2D | Rasterização discreta com números inteiros puros e variável de decisão de erro $e$. |
| **09** | **Álgebra Linear 2D & Matrizes $3\times3$** | CG 2D | Coordenadas homogêneas unificando translação, rotação e escala em matrizes afins. |
| **10** | **Pipeline MVP 3D & Divisão Perspectiva** | CG 3D | A jornada do vértice 3D até a tela 2D e o papel da divisão projetiva por $W = Z$. |
| **11** | **Modelagem Hierárquica & Grafo de Cena** | CG 3D | Cinemática direta e propagação matricial em cadeia pai-filho ($M_{\text{global}} = M_{\text{pai}} \times M_{\text{local}}$). |
| **12** | **Ray Tracing & Interseção Analítica Esfera** | Render Realística | Solução analítica da equação quadrática $at^2 + bt + c = 0$, normais unitárias e modelo Phong. |

---

## 🔬 Estúdio de Código C#, Compilação Roslyn & Renderização Dinâmica

O **Estúdio de Código Dedicado** (`CodeStudioWindow`) foi projetado para máxima imersão:

### 1. Janela Dedicada em Tela Cheia & Modo Foco
* Clique no botão **`🗖 Estúdio em Nova Janela (Tela Cheia)`** na barra superior para abrir uma janela autônoma maximizável, ideal para múltiplos monitores.
* Use o botão **`🗖 Modo Foco`** para recolher as barras laterais e dedicar 100% da tela ao editor de código C# e aos testes unitários.

### 2. Compilação ao Vivo com Microsoft Roslyn
* O estudante pode digitar livremente código C# no editor.
* Ao clicar em **`🚀 Compilar & Executar`** ou **`🧪 Rodar Testes`**, o motor `Microsoft.CodeAnalysis.CSharp.Scripting` avalia a função em milissegundos.
* **Renderização Dinâmica no Canvas:** Ao alterar qualquer valor numérico, cor, matriz ou fórmula geométrica no código, **o Canvas gráfico e o mapa de memória RAM são redesenhados imediatamente com base no seu código customizado!**

### 3. Bateria de Testes Automatizados & Gabaritos Oficiais
* Cada lição conta com asserções unitárias que validam entradas, saídas esperadas e casos de borda matemáticos com feedback imediato (verde para passou ✅, vermelho para falhou ❌).
* A aba **💡 Gabarito** oferece a solução oficial testada e explicada linha por linha, permitindo carregamento no editor com 1 clique.

### 4. Quizzes de Fixação Responsivos
* Perguntas conceituais com formatação automática de quebra de linha (`TextWrapping`), permitindo leitura limpa de respostas longas e diagnósticos teóricos completos.

---

## 📖 Referências Oficiais da Microsoft Learn

Cada lição no aplicativo e nesta documentação está conectada a recursos oficiais da Microsoft:

<div class="ms-ref-card">
  <h4><a href="https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/integral-numeric-types" target="_blank" rel="noopener">Tipos Numéricos Integrais no C# (byte, uint, int)</a></h4>
  <p>Tamanhos em memória, faixas de valores e operadores de deslocamento de bits (bit shifting).</p>
</div>

<div class="ms-ref-card">
  <h4><a href="https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/unsafe-code" target="_blank" rel="noopener">Código Não Seguro e Ponteiros no C# (unsafe / fixed)</a></h4>
  <p>Aritmética de ponteiros, alocação de blocos de memória e instruções para alta performance.</p>
</div>

<div class="ms-ref-card">
  <h4><a href="https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/introduction-to-wpf#wpf-architecture" target="_blank" rel="noopener">Visão Geral da Arquitetura do WPF</a></h4>
  <p>Como o PresentationCore gerencia a árvore visual e comunica-se com a camada milcore/DirectX.</p>
</div>

<div class="ms-ref-card">
  <h4><a href="https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.imaging.writeablebitmap" target="_blank" rel="noopener">Classe WriteableBitmap e Controle de Áreas Sujas (AddDirtyRect)</a></h4>
  <p>Gerenciamento do buffer traseiro e renderização acelerada por hardware no Windows.</p>
</div>

---

👉 **Próximo Passo:** Explore o guia de [Instalação e Compilação no Visual Studio](/iniciantes/instalacao-visual-studio/).
