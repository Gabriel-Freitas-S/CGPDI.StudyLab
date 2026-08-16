# Graph Report - CGPDI.StudyLab  (2026-08-16)

## Corpus Check
- 115 files · ~117,427 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1486 nodes · 3255 edges · 110 communities (71 shown, 39 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 256 edges (avg confidence: 0.81)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `690573bf`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- DirectBitmap
- MainWindow
- BorderlessWindow
- Vec3
- WpfViewport3DManager
- .UpdateStatus
- LiveCodeCompiler
- .Parse
- BorderlessWindow
- StudyTopic
- Slider
- CodeStudioWindow
- .UpdateContextualTopBar
- ProjectStudioControl
- UserControl
- dependencies
- Button
- Application
- .GetPlainText
- BorderlessWindow
- .SetPixel
- Computer Graphics and PDI Course Plan
- Mandatory test rule
- Rasterizer2D
- ColorSpaces
- .Btn3D_LoadRobot_Click
- Window
- Specular Reflection and Snell Refraction Documentation
- CGPDI.StudyLab.Core
- .Lock
- UpdateManager
- HierarchicalRobotArm
- FrequencyAndProcedural
- GeometricTransforms
- CGPDI.StudyLab.Tests
- cenario-universitario-sem-admin.md
- MainWindow
- TextBox
- .BtnStudioQuizOption_Click
- Graphics3DTests
- BtnPopoutStudio
- Morphology
- CGPDI.StudyLab.Views
- InteractiveLesson
- .SliderFree_ValueChanged
- Regras do Projeto CGPDI.StudyLab
- MathRendererAndSyntaxTests
- Changelog — CGPDI StudyLab
- .GeometricTransforms_Scale_CenterPixelPreservesColor
- MainWindowUiTests
- Segurança com Snyk (obrigatória)
- CSharpSyntaxHighlighter
- Qualidade com SonarQube (obrigatória)
- MathFormulaRenderer
- DirectBitmap Class Documentation
- Namespace conventions
- .GetTemplates
- Workflow: snyk-security
- UpdateDialogUiTests
- .SliderStudio_ValueChanged
- Software 3D Renderer (CPU) Documentation
- tsconfig.json
- CGPDI.StudyLab — Regras específicas do Antigravity
- WPF and XAML Rendering Explanation
- Edge Detection Documentation
- Morphological Operations and Otsu Documentation
- command
- CGPDI StudyLab Logo (SVG)
- Workflow: sonarqube-quality
- AlgorithmCodeSnippets.cs
- Flood Fill (Queue-based)
- Geometric Transformations Documentation
- graphify.js
- Graphify Knowledge Graph Rule
- .Clear
- RtbFreeCode
- ToggleButton
- TabItemFreeLiveXaml
- content.config.ts
- Auto-Update System (UpdateManager.cs)
- CodeQL Security Analysis Workflow
- Documentation Deployment Workflow
- _timer3D auto-rotation timer
- CGPDI StudyLab Logo (PNG)
- .UpdateHistogram
- .SetCode
- TabStudioEditor
- .MainTabControl_SelectionChanged
- Project Folder Structure Documentation
- Linear Algebra and Matrices for CG2D
- Synthetic Sample Image Generator
- Color Space Models (RGB, HSV, YCbCr, CMYK)
- Sepia Photographic Effect (Matrix Transform)
- Documentation Landing Page (StudyLab Overview)
- Debugging and Tips for Beginners
- Command Line Interface Guide
- Visual Studio Installation Guide
- Interactive Mode and Playground Guide
- Introduction to .NET and C# for Students
- Frequency Domain and Noise Documentation
- Fourier Transform Frequency Domain Analysis
- Spatial Filters and Convolution Documentation
- Point Operations and Histograms Documentation
- CbResolution
- TxtFreeConsole
- LstProjectTemplates

## God Nodes (most connected - your core abstractions)
1. `BorderlessWindow` - 240 edges
2. `MainWindow` - 193 edges
3. `DirectBitmap` - 99 edges
4. `BorderlessWindow` - 71 edges
5. `CodeStudioWindow` - 43 edges
6. `CGPDI.StudyLab.Core` - 40 edges
7. `UserControl` - 37 edges
8. `WpfViewport3DManager` - 34 edges
9. `TextBlock` - 34 edges
10. `LiveCodeCompiler` - 27 edges

## Surprising Connections (you probably didn't know these)
- `dotnet build (CI)` --semantically_similar_to--> `dotnet build command`  [INFERRED] [semantically similar]
  .github/workflows/release-app.yml → GEMINI.md
- `dotnet test (CI)` --semantically_similar_to--> `dotnet test command`  [INFERRED] [semantically similar]
  .github/workflows/release-app.yml → GEMINI.md
- `Mandatory test rule` --semantically_similar_to--> `Mandatory test rule`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md
- `Mandatory test rule` --semantically_similar_to--> `Mandatory test rule`  [INFERRED] [semantically similar]
  GEMINI.md → AGENTS.md
- `WindowStyle=None window pattern` --semantically_similar_to--> `WindowStyle=None window pattern`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Build and test quality gate** — github_workflows_release_app_ci_test_gate [EXTRACTED 0.95]
- **Studio and Laboratory window flow** — agents_rules_project_mainwindow, agents_rules_project_projectstudiowindow, agents_rules_project_codestudiowindow [EXTRACTED 0.95]
- **2D Rasterization Algorithm Family** — docs_src_content_docs_cg2d_algoritmos_de_linhas_bresenham, docs_src_content_docs_cg2d_circulos_elipses_e_curvas_midpoint_circle, docs_src_content_docs_cg2d_preenchimento_e_recorte_scanline, docs_src_content_docs_cg2d_algoritmos_de_linhas_xiaolin_wu [INFERRED 0.85]
- **PDI Image Processing Filter Pipeline** — docs_src_content_docs_pdi_filtros_espaciais_e_convolucoes_convolution, docs_src_content_docs_pdi_deteccao_de_bordas_e_canny_canny_edge, docs_src_content_docs_pdi_morfologia_matematica_e_otsu_otsu, docs_src_content_docs_pdi_operacoes_pontuais_e_histogramas_histogram [INFERRED 0.85]
- **Ray Tracing Full Optical Simulation Model** — docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_phong_model, docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_shadow_rays, docs_src_content_docs_raytracing_reflexao_refracao_snell_snell_law, docs_src_content_docs_raytracing_reflexao_refracao_snell_fresnel [INFERRED 0.85]
- **High-Performance CPU Rendering Core** — docs_src_content_docs_core_directbitmap_setpixel, docs_src_content_docs_core_directbitmap_parallel_for, docs_src_content_docs_core_fundamentos_de_memoria_unsafe_pointers, docs_src_content_docs_core_fundamentos_de_memoria_stride [INFERRED 0.95]

## Communities (110 total, 39 thin omitted)

### Community 0 - "DirectBitmap"
Cohesion: 0.19
Nodes (7): BitmapSource, byte, DirectBitmap, bool, SpatialFilters, IDisposable, WriteableBitmap

### Community 1 - "MainWindow"
Cohesion: 0.05
Nodes (11): MainWindow, bool, DispatcherTimer, double, int, List, Point, RoutedEventArgs (+3 more)

### Community 2 - "BorderlessWindow"
Cohesion: 0.03
Nodes (102): Description, Arrow, Border, BorderlessWindow, BrdQuizFeedback, BrdStudyTopicQuizFeedback, ColLabCode, ColLabPlayground (+94 more)

### Community 3 - "Vec3"
Cohesion: 0.09
Nodes (16): Mat4x4, Ray3D, Vec3, Vec4, double, MaterialRay, PlaneObject, PointLight (+8 more)

### Community 4 - "WpfViewport3DManager"
Cohesion: 0.07
Nodes (20): AmbientLight, WpfViewport3DManager, bool, Color, double, GeometryModel3D, Model3DGroup, MouseButtonEventArgs (+12 more)

### Community 5 - ".UpdateStatus"
Cohesion: 0.12
Nodes (5): BtnFocusCode, BtnResetColumns, RbCameraOrthographic, RbCameraPerspective, RadioButton

### Community 6 - "LiveCodeCompiler"
Cohesion: 0.11
Nodes (19): Action, CustomScriptGlobals, CustomScriptResult, EvaluationReport, LiveCodeCompiler, TestResult, XamlEvaluationResult, GeneratedRegex (+11 more)

### Community 7 - ".Parse"
Cohesion: 0.11
Nodes (17): Brush, ChangelogDocumentBuilder, SolidColorBrush, ChangelogBlock, ChangelogEntry, ChangelogInlineSegment, ChangelogParser, ChangelogSectionKind (+9 more)

### Community 8 - "BorderlessWindow"
Cohesion: 0.09
Nodes (15): BorderlessWindow, bool, Button, EventArgs, MouseButtonEventArgs, RoutedEventArgs, BorderlessWindow, BtnMaximize (+7 more)

### Community 9 - "StudyTopic"
Cohesion: 0.11
Nodes (15): DocReference, StudyGuideData, StudyQuiz, StudyTopic, List, Cmb3DShapes, CmbInterpolation, CmbSoftMesh (+7 more)

### Community 10 - "Slider"
Cohesion: 0.13
Nodes (19): Slider3DAmbient, Slider3DSpecular, SliderBrightness, SliderContrast, SliderGamma, SliderLab1, SliderLab2, SliderLab3 (+11 more)

### Community 11 - "CodeStudioWindow"
Cohesion: 0.11
Nodes (10): CodeStudioWindow, bool, DispatcherTimer, int, KeyEventArgs, List, RoutedEventArgs, SolidColorBrush (+2 more)

### Community 13 - "ProjectStudioControl"
Cohesion: 0.14
Nodes (10): ProjectStudioControl, bool, DispatcherTimer, EventArgs, List, RoutedEventArgs, SelectionChangedEventArgs, SolidColorBrush (+2 more)

### Community 14 - "UserControl"
Cohesion: 0.14
Nodes (22): BtnBorder, ImgFreeSimulation, PnlFreeLiveXamlContainer, TabBorder, TxtFreeParam1, TxtFreeParam2, TxtFreeParam3, TxtFreeParam4 (+14 more)

### Community 15 - "dependencies"
Cohesion: 0.09
Nodes (22): astro, @astrojs/starlight, dependencies, astro, @astrojs/starlight, katex, mermaid, rehype-katex (+14 more)

### Community 16 - "Button"
Cohesion: 0.09
Nodes (16): BtnGoToLaboratorioLesson, BtnGoToStudyTheory, BtnMaximize, BtnNextLesson, BtnPrevLesson, BtnQuizOpt0, BtnQuizOpt1, BtnQuizOpt2 (+8 more)

### Community 17 - "Application"
Cohesion: 0.12
Nodes (20): IsSubmenuOpen, Application, Arrow, Border, BtnBorder, ContentSite, DropDownBorder, DropDownScrollViewer (+12 more)

### Community 18 - ".GetPlainText"
Cohesion: 0.12
Nodes (17): CSharpSyntaxHighlighter, GeneratedRegex, Match, Regex, RichTextBox, Run, SolidColorBrush, XamlSyntaxHighlighter (+9 more)

### Community 19 - "BorderlessWindow"
Cohesion: 0.06
Nodes (54): BorderlessWindow, BrdStudioQuizFeedback, BtnBorder, ColStudioCanvas, ColStudioCode, ColStudioSplitter1, ColStudioSplitter2, ColStudioTrack (+46 more)

### Community 20 - ".SetPixel"
Cohesion: 0.30
Nodes (4): InteractiveLabManager, Color, StringBuilder, StringBuilder

### Community 21 - "Computer Graphics and PDI Course Plan"
Cohesion: 0.11
Nodes (19): Curriculum Mapping Table (Official Syllabus to Code), Study Guide for T1, T2, T3 Assessments, 2D Homogeneous Coordinates and Affine Transforms, Bresenham Line Drawing Algorithm, Line Drawing Algorithms Documentation, Xiaolin Wu Anti-Aliased Line Algorithm, Cubic Bezier Curves (de Casteljau), Circles, Ellipses and Bezier Curves Documentation (+11 more)

### Community 22 - "Mandatory test rule"
Cohesion: 0.14
Nodes (17): graphify knowledge graph rules, Mandatory test rule, WindowStyle=None window pattern, Mandatory test rule, WindowStyle=None window pattern, Google Antigravity agent, Dark palette colors, dotnet build command (+9 more)

### Community 23 - "Rasterizer2D"
Cohesion: 0.18
Nodes (9): Edge, OutCodes, Rasterizer2D, Color, double, int, Point, OutCodes (+1 more)

### Community 24 - "ColorSpaces"
Cohesion: 0.20
Nodes (3): ColorSpaces, GrayscaleMethod, Color

### Community 25 - ".Btn3D_LoadRobot_Click"
Cohesion: 0.22
Nodes (4): Chk3DAutoRotate, ChkRobotAnim, ChkSoftWireframe, CheckBox

### Community 26 - "Window"
Cohesion: 0.05
Nodes (40): CancellationTokenSource, UpdateSettings, UpdateSettingsStore, List, TimeSpan, Version, UpdateSettingsTests, Fact (+32 more)

### Community 27 - "Specular Reflection and Snell Refraction Documentation"
Cohesion: 0.15
Nodes (13): 4-DOF Articulated Robot Arm Demo, Hierarchical Solar System Simulation, Parent-Child Matrix Propagation in Scene Graph, Scene Graph Theory and Forward Kinematics, Ray Tracing Fundamentals and Physics of Light, Phong Illumination Model, Shadow Rays for Visibility Testing, Analytical Ray Intersection Documentation (+5 more)

### Community 28 - "CGPDI.StudyLab.Core"
Cohesion: 0.13
Nodes (5): CGPDI.StudyLab.Graphics3D, CGPDI.StudyLab.Core, CGPDI.StudyLab.Graphics2D, CGPDI.StudyLab.ImageProcessing, CGPDI.StudyLab.Tests.UnitTests

### Community 29 - ".Lock"
Cohesion: 0.14
Nodes (5): ImageSampleGenerator, PointAndHistograms, MouseButtonEventArgs, ImageProcessingTests, Fact

### Community 30 - "UpdateManager"
Cohesion: 0.07
Nodes (21): CancellationToken, App, Task, AppIconHelper, ReleaseInfo, UpdateManager, string, Task (+13 more)

### Community 31 - "HierarchicalRobotArm"
Cohesion: 0.17
Nodes (9): HierarchicalRobotArm, SceneNode3D, Color, GeometryModel3D, List, Model3DGroup, Point3D, RotateTransform3D (+1 more)

### Community 34 - "CGPDI.StudyLab.Tests"
Cohesion: 0.13
Nodes (14): net10.0-windows, Microsoft.NET.Sdk, CGPDI.StudyLab.Tests, net10.0-windows, Microsoft.NET.Sdk, coverlet.collector (6.0.4), FluentAssertions (8.0.1), Microsoft.CodeAnalysis.CSharp.Scripting (5.6.0) (+6 more)

### Community 35 - "cenario-universitario-sem-admin.md"
Cohesion: 0.20
Nodes (9): 1. O serviço de atualização fere alguma política de segurança corporativa/acadêmica?, 2. E se a faculdade precisar "congelar" a versão para dias de prova ou trabalhos avaliativos?, 🚀 A Solução do CGPDI StudyLab, 🔄 Como Funciona a Atualização Sem Administrador (Zero-Admin)?, 📦 Modelos de Instalação Disponíveis, 🛑 O Problema: Por que o Visual Studio Community Gera Fricção nos Laboratórios?, 🛡️ Pesquisa de Segurança & Políticas de TI (Compliance), Principais Dores Enfrentadas por Professores e Alunos: (+1 more)

### Community 36 - "MainWindow"
Cohesion: 0.22
Nodes (9): CodeStudioWindow, MainWindow, ProjectStudioWindow, WpfViewport3DManager.RotateCamera, CodeStudioWindow, MainWindow, ProjectStudioWindow, _timer3D auto-rotation timer (+1 more)

### Community 37 - "TextBox"
Cohesion: 0.22
Nodes (6): TxtCompilerReport, TxtLabConsole, TxtLabExplanation, TxtSearchStudy, TextChangedEventArgs, TextBox

### Community 38 - ".BtnStudioQuizOption_Click"
Cohesion: 0.24
Nodes (7): BtnMaximize, BtnStudioQuizOpt0, BtnStudioQuizOpt1, BtnStudioQuizOpt2, BtnStudioToggleCanvas, BtnStudioToggleTrack, Button

### Community 42 - "CGPDI.StudyLab.Views"
Cohesion: 0.16
Nodes (6): LaboratorioUiTests, UIFact, ProjectStudioUiTests, UIFact, CGPDI.StudyLab.Views, CGPDI.StudyLab.Tests.UiTests

### Community 43 - "InteractiveLesson"
Cohesion: 0.24
Nodes (5): InteractiveLesson, LessonType, QuizOption, List, Button

### Community 44 - ".SliderFree_ValueChanged"
Cohesion: 0.43
Nodes (6): SliderFree1, SliderFree2, SliderFree3, SliderFree4, RoutedPropertyChangedEventArgs, Slider

### Community 45 - "Regras do Projeto CGPDI.StudyLab"
Cohesion: 0.29
Nodes (6): Comandos obrigatórios, Consistência de estilo (NÃO quebrar o que funciona), Convenções de código, Fluxo de janelas / Estúdio / Laboratório, Regra de testes (obrigatória), Regras do Projeto CGPDI.StudyLab

### Community 46 - "MathRendererAndSyntaxTests"
Cohesion: 0.36
Nodes (3): MathRendererAndSyntaxTests, Fact, WpfFact

### Community 47 - "Changelog — CGPDI StudyLab"
Cohesion: 0.14
Nodes (13): Adicionado, Adicionado, Adicionado, Adicionado, Changelog — CGPDI StudyLab, Corrigido, Removido, Segurança (+5 more)

### Community 50 - "Segurança com Snyk (obrigatória)"
Cohesion: 0.40
Nodes (4): Comandos, Regras, Segurança com Snyk (obrigatória), Troubleshooting (docs.snyk.io)

### Community 51 - "CSharpSyntaxHighlighter"
Cohesion: 0.33
Nodes (6): CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter, CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter

### Community 52 - "Qualidade com SonarQube (obrigatória)"
Cohesion: 0.40
Nodes (4): Comandos, Métricas monitoradas, Qualidade com SonarQube (obrigatória), Regras

### Community 53 - "MathFormulaRenderer"
Cohesion: 0.18
Nodes (9): MathFormulaRenderer, GeneratedRegex, Match, Regex, Run, SearchValues, SolidColorBrush, Dictionary (+1 more)

### Community 54 - "DirectBitmap Class Documentation"
Cohesion: 0.40
Nodes (6): DirectBitmap Class Documentation, Parallel.For Multicore Pixel Processing, DirectBitmap.SetPixel (Unsafe Pointer Pixel Write), Image Memory Model (Stride, BGRA32), Stride Memory Layout for 2D Images, Unsafe Byte Pointer Memory Access

### Community 55 - "Namespace conventions"
Cohesion: 0.40
Nodes (5): DirectBitmap, LiveCodeCompiler, Namespace conventions, DirectBitmap, LiveCodeCompiler

### Community 56 - ".GetTemplates"
Cohesion: 0.40
Nodes (3): ProjectTemplate, ProjectTemplatesManager, List

### Community 59 - ".SliderStudio_ValueChanged"
Cohesion: 0.47
Nodes (5): SliderStudio1, SliderStudio2, SliderStudio3, RoutedPropertyChangedEventArgs, Slider

### Community 60 - "Software 3D Renderer (CPU) Documentation"
Cohesion: 0.40
Nodes (5): Back-face Culling Algorithm, Software 3D Renderer (CPU) Documentation, Z-Buffer Depth Testing Algorithm, Arcball Orbital Camera Control, WPF Viewport3D Hardware Rendering Documentation

### Community 61 - "tsconfig.json"
Cohesion: 0.40
Nodes (4): compilerOptions, moduleResolution, extends, astro/tsconfigs/strict

### Community 62 - "CGPDI.StudyLab — Regras específicas do Antigravity"
Cohesion: 0.40
Nodes (4): Antes de qualquer mudança, CGPDI.StudyLab — Regras específicas do Antigravity, Consistência de estilo (prioridade máxima), Gate de qualidade

### Community 63 - "WPF and XAML Rendering Explanation"
Cohesion: 0.50
Nodes (4): Architecture Overview (Layered, Parallel Processing), WPF Viewport3D Hardware Rendering, WPF and XAML Rendering Explanation, WriteableBitmap Lock/Unlock Cycle

### Community 64 - "Edge Detection Documentation"
Cohesion: 0.50
Nodes (4): Canny Edge Detector (5-Step Algorithm), Edge Detection Documentation, Sobel Edge Detection Operator, 2D Spatial Convolution and Filters

### Community 65 - "Morphological Operations and Otsu Documentation"
Cohesion: 0.50
Nodes (4): Morphological Operations and Otsu Documentation, Mathematical Morphology (Erosion and Dilation), Otsu Automatic Thresholding, Histogram Equalization and Point Operations

### Community 66 - "command"
Cohesion: 0.14
Nodes (13): mcp, snyk, plugin, $schema, command, enabled, type, C:\\tools\\snyk\\snyk.exe (+5 more)

### Community 67 - "CGPDI StudyLab Logo (SVG)"
Cohesion: 0.67
Nodes (3): CGPDI StudyLab Full Logo with Text (SVG), CGPDI StudyLab Logo (SVG), Documentation Site Logo (SVG)

### Community 70 - "Flood Fill (Queue-based)"
Cohesion: 0.67
Nodes (3): Cohen-Sutherland Line Clipping Algorithm, Flood Fill (Queue-based), Scanline Polygon Fill Algorithm (AET)

### Community 71 - "Geometric Transformations Documentation"
Cohesion: 0.67
Nodes (3): Bilinear Spatial Interpolation, Geometric Transformations Documentation, Geometric Transformations Inverse Mapping

### Community 74 - ".Clear"
Cohesion: 0.16
Nodes (6): Color, Matrix3x3, double, Point, Rasterization2DTests, Fact

### Community 75 - "RtbFreeCode"
Cohesion: 0.67
Nodes (3): RtbFreeCode, RtbFreeXamlCode, RichTextBox

### Community 76 - "ToggleButton"
Cohesion: 0.67
Nodes (3): ToggleButton, IsDropDownOpen, ToggleButton

### Community 86 - ".UpdateHistogram"
Cohesion: 0.25
Nodes (4): CanvasHistogram, Color, SizeChangedEventArgs, Canvas

### Community 87 - ".SetCode"
Cohesion: 0.18
Nodes (4): LstStudioLessons, Button, SelectionChangedEventArgs, ListBox

### Community 88 - "TabStudioEditor"
Cohesion: 0.67
Nodes (3): TabStudioEditor, TabStudioVisualizer, TabControl

### Community 89 - ".MainTabControl_SelectionChanged"
Cohesion: 0.40
Nodes (4): MainTabControl, TabLabEditor, TabLabVisualizer, TabControl

## Knowledge Gaps
- **182 isolated node(s):** `$schema`, `.opencode/plugins/graphify.js`, `type`, `C:\\tools\\snyk\\snyk.exe`, `mcp` (+177 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **39 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `MainWindow` connect `MainWindow` to `DirectBitmap`, `BorderlessWindow`, `Vec3`, `WpfViewport3DManager`, `.UpdateStatus`, `BorderlessWindow`, `StudyTopic`, `Slider`, `.UpdateContextualTopBar`, `Button`, `.GetPlainText`, `.Btn3D_LoadRobot_Click`, `CGPDI.StudyLab.Core`, `.Lock`, `HierarchicalRobotArm`, `TextBox`, `InteractiveLesson`, `.UpdateHistogram`, `.MainTabControl_SelectionChanged`?**
  _High betweenness centrality (0.247) - this node is a cross-community bridge._
- **Why does `CGPDI.StudyLab.Core` connect `CGPDI.StudyLab.Core` to `Window`, `Vec3`, `AlgorithmCodeSnippets.cs`, `LiveCodeCompiler`, `.Parse`, `StudyTopic`, `Morphology`, `InteractiveLesson`, `CGPDI.StudyLab.Views`, `MathFormulaRenderer`, `ColorSpaces`, `.GetTemplates`, `UpdateManager`?**
  _High betweenness centrality (0.142) - this node is a cross-community bridge._
- **Why does `BorderlessWindow` connect `BorderlessWindow` to `.MainTabControl_SelectionChanged`, `MainWindow`, `.UpdateStatus`, `TextBox`, `StudyTopic`, `Slider`, `ToggleButton`, `.UpdateContextualTopBar`, `Button`, `.UpdateHistogram`, `.Btn3D_LoadRobot_Click`?**
  _High betweenness centrality (0.141) - this node is a cross-community bridge._
- **What connects `$schema`, `.opencode/plugins/graphify.js`, `type` to the rest of the system?**
  _182 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `MainWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.04968589377498572 - nodes in this community are weakly interconnected._
- **Should `BorderlessWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.031220255092328193 - nodes in this community are weakly interconnected._
- **Should `Vec3` be split into smaller, more focused modules?**
  _Cohesion score 0.09098639455782313 - nodes in this community are weakly interconnected._