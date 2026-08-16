# Graph Report - CGPDI.StudyLab  (2026-08-16)

## Corpus Check
- 102 files · ~108,649 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1276 nodes · 2860 edges · 102 communities (66 shown, 36 thin omitted)
- Extraction: 91% EXTRACTED · 9% INFERRED · 0% AMBIGUOUS · INFERRED: 244 edges (avg confidence: 0.81)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `175c7b88`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- DirectBitmap
- MainWindow
- BorderlessWindow
- Vec3
- WpfViewport3DManager
- Window
- .RunTestsAndEvaluateAsync
- BorderlessWindow
- Mesh3D
- Slider
- CodeStudioWindow
- .UpdateContextualTopBar
- ProjectStudioControl
- UserControl
- dependencies
- Button
- Application
- .SetCode
- BorderlessWindow
- .SetPixel
- Computer Graphics and PDI Course Plan
- Mandatory test rule
- Rasterizer2D
- ColorSpaces
- .UpdateStatus
- MathFormulaRenderer
- Specular Reflection and Snell Refraction Documentation
- CGPDI.StudyLab.Core
- .Lock
- .UpdateHistogram
- InteractiveLesson
- .Clear
- GeometricTransforms
- CGPDI.StudyLab.Tests
- FrequencyAndProcedural
- MainWindow
- TextBox
- .BtnStudioQuizOption_Click
- .BtnRunCode_Click
- RoutedEventArgs
- Morphology
- CGPDI.StudyLab.Views
- StudyTopic
- .SliderFree_ValueChanged
- Regras do Projeto CGPDI.StudyLab
- Matrix3x3
- .ImgDisplay2D_MouseDown
- .ChkRobotAnim_CheckedChanged
- MainWindowUiTests
- .ExecuteCustomScriptAsync
- CSharpSyntaxHighlighter
- MathRendererAndSyntaxTests
- TabItemFreeLiveXaml
- DirectBitmap Class Documentation
- Namespace conventions
- .GetTemplates
- QuizAndCurriculumTests.cs
- UpdateManagerTests.cs
- .SliderStudio_ValueChanged
- Software 3D Renderer (CPU) Documentation
- tsconfig.json
- CGPDI.StudyLab — Regras específicas do Antigravity
- WPF and XAML Rendering Explanation
- Edge Detection Documentation
- Morphological Operations and Otsu Documentation
- opencode.json
- CGPDI StudyLab Logo (SVG)
- SoftwareRenderer3D
- RtbFreeCode
- Flood Fill (Queue-based)
- Geometric Transformations Documentation
- graphify.js
- Graphify Knowledge Graph Rule
- CbResolution
- ToggleButton
- content.config.ts
- Auto-Update System (UpdateManager.cs)
- CodeQL Security Analysis Workflow
- Documentation Deployment Workflow
- _timer3D auto-rotation timer
- CGPDI StudyLab Logo (PNG)
- .LstStudioLessons_SelectionChanged
- TabStudioEditor
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

## God Nodes (most connected - your core abstractions)
1. `BorderlessWindow` - 240 edges
2. `MainWindow` - 193 edges
3. `DirectBitmap` - 99 edges
4. `BorderlessWindow` - 71 edges
5. `CodeStudioWindow` - 40 edges
6. `UserControl` - 37 edges
7. `CGPDI.StudyLab.Core` - 34 edges
8. `WpfViewport3DManager` - 34 edges
9. `TextBlock` - 34 edges
10. `ProjectStudioControl` - 25 edges

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

## Communities (102 total, 36 thin omitted)

### Community 0 - "DirectBitmap"
Cohesion: 0.17
Nodes (7): BitmapSource, byte, DirectBitmap, bool, SpatialFilters, IDisposable, WriteableBitmap

### Community 1 - "MainWindow"
Cohesion: 0.05
Nodes (10): MainWindow, bool, DispatcherTimer, double, int, List, Point, RoutedEventArgs (+2 more)

### Community 2 - "BorderlessWindow"
Cohesion: 0.03
Nodes (97): Description, Arrow, Border, BorderlessWindow, BrdQuizFeedback, BrdStudyTopicQuizFeedback, ColLabCode, ColLabPlayground (+89 more)

### Community 3 - "Vec3"
Cohesion: 0.13
Nodes (12): Mat4x4, Ray3D, Vec3, Vec4, double, MaterialRay, PlaneObject, PointLight (+4 more)

### Community 4 - "WpfViewport3DManager"
Cohesion: 0.08
Nodes (20): AmbientLight, WpfViewport3DManager, bool, Color, double, GeometryModel3D, Model3DGroup, MouseButtonEventArgs (+12 more)

### Community 5 - "Window"
Cohesion: 0.07
Nodes (31): CancellationToken, CancellationTokenSource, ReleaseInfo, UpdateManager, string, Task, border, BtnApply (+23 more)

### Community 6 - ".RunTestsAndEvaluateAsync"
Cohesion: 0.17
Nodes (15): Action, CustomScriptGlobals, CustomScriptResult, EvaluationReport, LiveCodeCompiler, TestResult, XamlEvaluationResult, List (+7 more)

### Community 8 - "BorderlessWindow"
Cohesion: 0.09
Nodes (15): BorderlessWindow, bool, Button, EventArgs, MouseButtonEventArgs, RoutedEventArgs, BorderlessWindow, BtnMaximize (+7 more)

### Community 9 - "Mesh3D"
Cohesion: 0.10
Nodes (15): Mesh3D, Color, List, Cmb3DShapes, CmbInterpolation, CmbSoftMesh, LstInteractiveLessons, LstStudyTopics (+7 more)

### Community 10 - "Slider"
Cohesion: 0.07
Nodes (27): HierarchicalRobotArm, SceneNode3D, Color, GeometryModel3D, List, Model3DGroup, Point3D, Slider3DAmbient (+19 more)

### Community 11 - "CodeStudioWindow"
Cohesion: 0.11
Nodes (8): CodeStudioWindow, bool, DispatcherTimer, int, KeyEventArgs, List, RoutedEventArgs, TextChangedEventArgs

### Community 13 - "ProjectStudioControl"
Cohesion: 0.16
Nodes (9): LstProjectTemplates, ProjectStudioControl, bool, DispatcherTimer, EventArgs, List, SelectionChangedEventArgs, TextChangedEventArgs (+1 more)

### Community 14 - "UserControl"
Cohesion: 0.12
Nodes (24): BtnBorder, ImgFreeSimulation, PnlFreeLiveXamlContainer, TabBorder, TxtFreeConsole, TxtFreeParam1, TxtFreeParam2, TxtFreeParam3 (+16 more)

### Community 15 - "dependencies"
Cohesion: 0.08
Nodes (24): astro, astro-mermaid, @astrojs/starlight, dependencies, astro, astro-mermaid, @astrojs/starlight, katex (+16 more)

### Community 16 - "Button"
Cohesion: 0.09
Nodes (16): BtnGoToLaboratorioLesson, BtnGoToStudyTheory, BtnMaximize, BtnNextLesson, BtnPrevLesson, BtnQuizOpt0, BtnQuizOpt1, BtnQuizOpt2 (+8 more)

### Community 17 - "Application"
Cohesion: 0.08
Nodes (25): IsSubmenuOpen, Application, Arrow, Border, BtnBorder, ContentSite, DropDownBorder, DropDownScrollViewer (+17 more)

### Community 18 - ".SetCode"
Cohesion: 0.16
Nodes (12): CSharpSyntaxHighlighter, Regex, RichTextBox, SolidColorBrush, XamlSyntaxHighlighter, Regex, RichTextBox, SolidColorBrush (+4 more)

### Community 19 - "BorderlessWindow"
Cohesion: 0.06
Nodes (54): BorderlessWindow, BrdStudioQuizFeedback, BtnBorder, ColStudioCanvas, ColStudioCode, ColStudioSplitter1, ColStudioSplitter2, ColStudioTrack (+46 more)

### Community 20 - ".SetPixel"
Cohesion: 0.32
Nodes (3): InteractiveLabManager, Color, StringBuilder

### Community 21 - "Computer Graphics and PDI Course Plan"
Cohesion: 0.11
Nodes (19): Curriculum Mapping Table (Official Syllabus to Code), Study Guide for T1, T2, T3 Assessments, 2D Homogeneous Coordinates and Affine Transforms, Bresenham Line Drawing Algorithm, Line Drawing Algorithms Documentation, Xiaolin Wu Anti-Aliased Line Algorithm, Cubic Bezier Curves (de Casteljau), Circles, Ellipses and Bezier Curves Documentation (+11 more)

### Community 22 - "Mandatory test rule"
Cohesion: 0.14
Nodes (17): graphify knowledge graph rules, Mandatory test rule, WindowStyle=None window pattern, Mandatory test rule, WindowStyle=None window pattern, Google Antigravity agent, Dark palette colors, dotnet build command (+9 more)

### Community 23 - "Rasterizer2D"
Cohesion: 0.18
Nodes (9): Edge, OutCode, Rasterizer2D, Color, double, int, Point, OutCode (+1 more)

### Community 24 - "ColorSpaces"
Cohesion: 0.20
Nodes (3): ColorSpaces, GrayscaleMethod, Color

### Community 25 - ".UpdateStatus"
Cohesion: 0.12
Nodes (5): BtnFocusCode, BtnResetColumns, RbCameraOrthographic, RbCameraPerspective, RadioButton

### Community 26 - "MathFormulaRenderer"
Cohesion: 0.29
Nodes (4): MathFormulaRenderer, SolidColorBrush, Dictionary, InlineCollection

### Community 27 - "Specular Reflection and Snell Refraction Documentation"
Cohesion: 0.15
Nodes (13): 4-DOF Articulated Robot Arm Demo, Hierarchical Solar System Simulation, Parent-Child Matrix Propagation in Scene Graph, Scene Graph Theory and Forward Kinematics, Ray Tracing Fundamentals and Physics of Light, Phong Illumination Model, Shadow Rays for Visibility Testing, Analytical Ray Intersection Documentation (+5 more)

### Community 28 - "CGPDI.StudyLab.Core"
Cohesion: 0.13
Nodes (8): AlgorithmCodeSnippets, string, CGPDI.StudyLab.Graphics3D, CGPDI.StudyLab.Core, CGPDI.StudyLab.Graphics2D, CGPDI.StudyLab.ImageProcessing, CGPDI.StudyLab.Tests.UnitTests, CGPDI.StudyLab

### Community 29 - ".Lock"
Cohesion: 0.13
Nodes (7): ImageSampleGenerator, PointAndHistograms, SliderBrightness, SliderContrast, SliderGamma, ImageProcessingTests, Fact

### Community 30 - ".UpdateHistogram"
Cohesion: 0.25
Nodes (4): CanvasHistogram, Color, SizeChangedEventArgs, Canvas

### Community 31 - "InteractiveLesson"
Cohesion: 0.24
Nodes (5): InteractiveLesson, LessonType, QuizOption, List, Button

### Community 32 - ".Clear"
Cohesion: 0.19
Nodes (5): Color, GeometricTransformsAndMath3DTests, Fact, Rasterization2DTests, Fact

### Community 34 - "CGPDI.StudyLab.Tests"
Cohesion: 0.15
Nodes (12): net10.0-windows, Microsoft.NET.Sdk, CGPDI.StudyLab.Tests, net10.0-windows, Microsoft.NET.Sdk, FluentAssertions (8.0.1), Microsoft.CodeAnalysis.CSharp.Scripting (5.6.0), Microsoft.NET.Test.Sdk (17.13.0) (+4 more)

### Community 36 - "MainWindow"
Cohesion: 0.22
Nodes (9): CodeStudioWindow, MainWindow, ProjectStudioWindow, WpfViewport3DManager.RotateCamera, CodeStudioWindow, MainWindow, ProjectStudioWindow, _timer3D auto-rotation timer (+1 more)

### Community 37 - "TextBox"
Cohesion: 0.22
Nodes (6): TxtCompilerReport, TxtLabConsole, TxtLabExplanation, TxtSearchStudy, TextChangedEventArgs, TextBox

### Community 38 - ".BtnStudioQuizOption_Click"
Cohesion: 0.32
Nodes (7): BtnMaximize, BtnStudioQuizOpt0, BtnStudioQuizOpt1, BtnStudioQuizOpt2, BtnStudioToggleCanvas, BtnStudioToggleTrack, Button

### Community 40 - "RoutedEventArgs"
Cohesion: 0.40
Nodes (3): BtnPopoutStudio, RoutedEventArgs, Button

### Community 42 - "CGPDI.StudyLab.Views"
Cohesion: 0.22
Nodes (4): LaboratorioUiTests, UIFact, CGPDI.StudyLab.Views, CGPDI.StudyLab.Tests.UiTests

### Community 43 - "StudyTopic"
Cohesion: 0.52
Nodes (5): DocReference, StudyGuideData, StudyQuiz, StudyTopic, List

### Community 44 - ".SliderFree_ValueChanged"
Cohesion: 0.43
Nodes (6): SliderFree1, SliderFree2, SliderFree3, SliderFree4, RoutedPropertyChangedEventArgs, Slider

### Community 45 - "Regras do Projeto CGPDI.StudyLab"
Cohesion: 0.29
Nodes (6): Comandos obrigatórios, Consistência de estilo (NÃO quebrar o que funciona), Convenções de código, Fluxo de janelas / Estúdio / Laboratório, Regra de testes (obrigatória), Regras do Projeto CGPDI.StudyLab

### Community 46 - "Matrix3x3"
Cohesion: 0.29
Nodes (3): Matrix3x3, double, Point

### Community 47 - ".ImgDisplay2D_MouseDown"
Cohesion: 0.29
Nodes (6): ImgDisplay, ImgDisplay2D, ImgDisplay3DSoft, ImgLabSimulation, MouseButtonEventArgs, Image

### Community 48 - ".ChkRobotAnim_CheckedChanged"
Cohesion: 0.29
Nodes (4): Chk3DAutoRotate, ChkRobotAnim, ChkSoftWireframe, CheckBox

### Community 51 - "CSharpSyntaxHighlighter"
Cohesion: 0.33
Nodes (6): CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter, CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter

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

### Community 66 - "opencode.json"
Cohesion: 0.50
Nodes (3): plugin, $schema, .opencode/plugins/graphify.js

### Community 67 - "CGPDI StudyLab Logo (SVG)"
Cohesion: 0.67
Nodes (3): CGPDI StudyLab Full Logo with Text (SVG), CGPDI StudyLab Logo (SVG), Documentation Site Logo (SVG)

### Community 69 - "RtbFreeCode"
Cohesion: 0.67
Nodes (3): RtbFreeCode, RtbFreeXamlCode, RichTextBox

### Community 70 - "Flood Fill (Queue-based)"
Cohesion: 0.67
Nodes (3): Cohen-Sutherland Line Clipping Algorithm, Flood Fill (Queue-based), Scanline Polygon Fill Algorithm (AET)

### Community 71 - "Geometric Transformations Documentation"
Cohesion: 0.67
Nodes (3): Bilinear Spatial Interpolation, Geometric Transformations Documentation, Geometric Transformations Inverse Mapping

### Community 76 - "ToggleButton"
Cohesion: 0.67
Nodes (3): ToggleButton, IsDropDownOpen, ToggleButton

### Community 87 - ".LstStudioLessons_SelectionChanged"
Cohesion: 0.29
Nodes (4): LstStudioLessons, Button, SelectionChangedEventArgs, ListBox

### Community 88 - "TabStudioEditor"
Cohesion: 0.50
Nodes (3): TabStudioEditor, TabStudioVisualizer, TabControl

## Knowledge Gaps
- **153 isolated node(s):** `$schema`, `.opencode/plugins/graphify.js`, `net10.0-windows`, `Microsoft.NET.Test.Sdk (17.13.0)`, `xunit (2.9.3)` (+148 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **36 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `MainWindow` connect `MainWindow` to `DirectBitmap`, `BorderlessWindow`, `WpfViewport3DManager`, `.Apply2DTransform`, `BorderlessWindow`, `Mesh3D`, `Slider`, `.UpdateContextualTopBar`, `Button`, `.SetCode`, `.UpdateStatus`, `CGPDI.StudyLab.Core`, `.Lock`, `.UpdateHistogram`, `InteractiveLesson`, `TextBox`, `Matrix3x3`, `.ImgDisplay2D_MouseDown`, `.ChkRobotAnim_CheckedChanged`?**
  _High betweenness centrality (0.287) - this node is a cross-community bridge._
- **Why does `BorderlessWindow` connect `BorderlessWindow` to `MainWindow`, `TextBox`, `.Apply2DTransform`, `Mesh3D`, `Slider`, `ToggleButton`, `.UpdateContextualTopBar`, `.ImgDisplay2D_MouseDown`, `Button`, `.ChkRobotAnim_CheckedChanged`, `.UpdateStatus`, `.Lock`, `.UpdateHistogram`?**
  _High betweenness centrality (0.201) - this node is a cross-community bridge._
- **Why does `DirectBitmap` connect `DirectBitmap` to `.Clear`, `GeometricTransforms`, `MainWindow`, `Vec3`, `SoftwareRenderer3D`, `FrequencyAndProcedural`, `.RunTestsAndEvaluateAsync`, `Morphology`, `CodeStudioWindow`, `.UpdateContextualTopBar`, `ProjectStudioControl`, `.ExecuteCustomScriptAsync`, `.SetPixel`, `Rasterizer2D`, `CGPDI.StudyLab.Core`, `.Lock`?**
  _High betweenness centrality (0.157) - this node is a cross-community bridge._
- **What connects `$schema`, `.opencode/plugins/graphify.js`, `net10.0-windows` to the rest of the system?**
  _153 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `MainWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.05304982817869416 - nodes in this community are weakly interconnected._
- **Should `BorderlessWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.03282137597306964 - nodes in this community are weakly interconnected._
- **Should `Vec3` be split into smaller, more focused modules?**
  _Cohesion score 0.12762762762762764 - nodes in this community are weakly interconnected._