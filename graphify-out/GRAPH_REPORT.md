# Graph Report - CGPDI.StudyLab  (2026-08-16)

## Corpus Check
- 104 files · ~111,638 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1209 nodes · 2778 edges · 90 communities (62 shown, 28 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 215 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `222c3c30`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- DirectBitmap
- .UpdateContextualTopBar
- MainWindow
- Vec3
- WpfViewport3DManager
- CGPDI.StudyLab.Core
- Window
- UserControl
- Window
- .SetPixel
- CodeStudioWindow
- Window
- Slider
- .RunTestsAndEvaluateAsync
- Application
- dependencies
- ProjectStudioWindow
- .GetPlainText
- Button
- Academic Documentation & CG2D Algorithms
- HierarchicalRobotArm
- ProjectStudioControl
- .Apply2DTransform
- Hierarchy & Ray Tracing Docs
- Wiki Curriculum Chapters
- .RenderToTextBlock
- .BtnStudioQuizOption_Click
- .UpdateHistogram
- TextBox
- .MainTabControl_SelectionChanged
- .GetTemplates
- .ChkRobotAnim_CheckedChanged
- .UpdateStatus
- DirectBitmap Class Documentation
- CGPDI.StudyLab.Tests
- ColorSpaces
- Software 3D Renderer (CPU) Documentation
- tsconfig.json
- MathRendererAndSyntaxTests
- AppIconHelper
- .BtnMaximize_Click
- WPF and XAML Rendering Explanation
- Edge Detection Documentation
- Morphological Operations and Otsu Documentation
- Graphify Knowledge Graph Rule
- App Build and Release Pipeline
- CGPDI StudyLab Logo (SVG)
- CGPDI.StudyLab.Views
- Flood Fill (Queue-based)
- Geometric Transformations Documentation
- graphify.js
- .LstStudioLessons_SelectionChanged
- content.config.ts
- Auto-Update System (UpdateManager.cs)
- CGPDI StudyLab Logo (PNG)
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
- AlgorithmCodeSnippets.cs
- StudioSplitter1
- TabStudioEditor
- StudyTopic
- .SliderFree_ValueChanged
- .SliderStudio_ValueChanged
- IcStudioMsRefs
- MainWindow.xaml.cs
- ImgStudioSimulation
- PbStudioProgress
- ProjectStudioUiTests
- QuizAndCurriculumTests.cs
- RichTextBox
- TextBox
- ToggleButton
- TabStudioEditor
- TabItemStudioLiveXaml

## God Nodes (most connected - your core abstractions)
1. `Window` - 242 edges
2. `MainWindow` - 197 edges
3. `DirectBitmap` - 99 edges
4. `Window` - 74 edges
5. `CodeStudioWindow` - 45 edges
6. `UserControl` - 37 edges
7. `TextBlock` - 34 edges
8. `WpfViewport3DManager` - 33 edges
9. `CGPDI.StudyLab.Core` - 31 edges
10. `ProjectStudioControl` - 25 edges

## Surprising Connections (you probably didn't know these)
- `CGPDI StudyLab Logo (SVG)` --semantically_similar_to--> `Documentation Site Logo (SVG)`  [INFERRED] [semantically similar]
  CGPDI.StudyLab/Assets/logo.svg → docs/src/assets/logo.svg
- `Computer Graphics and PDI Course Plan` --references--> `CGPDI StudyLab Project Overview`  [INFERRED]
  Plano de Ensino.md → README.md
- `2D Homogeneous Coordinates and Affine Transforms` --semantically_similar_to--> `MVP Matrix Pipeline (Model-View-Projection)`  [INFERRED] [semantically similar]
  docs/src/content/docs/cg2d/algebra-linear-e-matrizes.md → docs/src/content/docs/cg3d/matematica-vetorial-e-mvp.md
- `Bresenham Line Drawing Algorithm` --semantically_similar_to--> `Midpoint Circle Algorithm (8-way Symmetry)`  [INFERRED] [semantically similar]
  docs/src/content/docs/cg2d/algoritmos-de-linhas.md → docs/src/content/docs/cg2d/circulos-elipses-e-curvas.md
- `Curriculum Mapping Table (Official Syllabus to Code)` --references--> `Computer Graphics and PDI Course Plan`  [EXTRACTED]
  docs/src/content/docs/academico/mapeamento-do-plano.md → Plano de Ensino.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **CI/CD GitHub Actions Workflows** — _github_workflows_codeql_codeql_analysis, _github_workflows_deploy_docs_docs_deployment, _github_workflows_release_app_release_pipeline [EXTRACTED 0.95]
- **Wiki Curriculum Chapter Sequence** — wiki_1_fundamentos_e_manipulacao_de_memoria_memory_chapter, wiki_2_processamento_digital_de_imagens_pdi_chapter, wiki_3_computacao_grafica_2d_e_rasterizacao_cg2d_chapter, wiki_4_computacao_grafica_3d_e_pipeline_cg3d_chapter, wiki_5_modelagem_hierarquica_e_cinematica_direta_hierarchy_chapter, wiki_6_ray_tracing_e_renderizacao_realistica_raytracing_chapter [EXTRACTED 0.95]
- **2D Rasterization Algorithm Family** — docs_src_content_docs_cg2d_algoritmos_de_linhas_bresenham, docs_src_content_docs_cg2d_circulos_elipses_e_curvas_midpoint_circle, docs_src_content_docs_cg2d_preenchimento_e_recorte_scanline, docs_src_content_docs_cg2d_algoritmos_de_linhas_xiaolin_wu [INFERRED 0.85]
- **PDI Image Processing Filter Pipeline** — docs_src_content_docs_pdi_filtros_espaciais_e_convolucoes_convolution, docs_src_content_docs_pdi_deteccao_de_bordas_e_canny_canny_edge, docs_src_content_docs_pdi_morfologia_matematica_e_otsu_otsu, docs_src_content_docs_pdi_operacoes_pontuais_e_histogramas_histogram [INFERRED 0.85]
- **Ray Tracing Full Optical Simulation Model** — docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_phong_model, docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_shadow_rays, docs_src_content_docs_raytracing_reflexao_refracao_snell_snell_law, docs_src_content_docs_raytracing_reflexao_refracao_snell_fresnel [INFERRED 0.85]
- **High-Performance CPU Rendering Core** — docs_src_content_docs_core_directbitmap_setpixel, docs_src_content_docs_core_directbitmap_parallel_for, docs_src_content_docs_core_fundamentos_de_memoria_unsafe_pointers, docs_src_content_docs_core_fundamentos_de_memoria_stride [INFERRED 0.95]

## Communities (90 total, 28 thin omitted)

### Community 0 - "DirectBitmap"
Cohesion: 0.06
Nodes (22): BitmapSource, byte, DirectBitmap, bool, Color, ImageSampleGenerator, FrequencyAndProcedural, int (+14 more)

### Community 2 - "MainWindow"
Cohesion: 0.05
Nodes (9): MainWindow, bool, DispatcherTimer, double, int, List, Point, RoutedEventArgs (+1 more)

### Community 3 - "Vec3"
Cohesion: 0.09
Nodes (16): Mat4x4, Ray3D, Vec3, Vec4, double, MaterialRay, PlaneObject, PointLight (+8 more)

### Community 4 - "WpfViewport3DManager"
Cohesion: 0.09
Nodes (17): AmbientLight, WpfViewport3DManager, bool, Color, double, GeometryModel3D, Model3DGroup, MouseButtonEventArgs (+9 more)

### Community 5 - "CGPDI.StudyLab.Core"
Cohesion: 0.18
Nodes (4): CGPDI.StudyLab.Core, CGPDI.StudyLab.Graphics2D, CGPDI.StudyLab.ImageProcessing, CGPDI.StudyLab.Tests.UnitTests

### Community 6 - "Window"
Cohesion: 0.07
Nodes (32): CancellationToken, CancellationTokenSource, ReleaseInfo, UpdateManager, string, Task, border, BtnApply (+24 more)

### Community 7 - "UserControl"
Cohesion: 0.09
Nodes (31): BtnBorder, CbResolution, ImgFreeSimulation, PnlFreeLiveXamlContainer, RtbFreeCode, RtbFreeXamlCode, TabBorder, TabItemFreeLiveXaml (+23 more)

### Community 8 - "Window"
Cohesion: 0.03
Nodes (102): Description, Arrow, Border, BrdQuizFeedback, BrdStudyTopicQuizFeedback, ColLabCode, ColLabPlayground, ColLabSplitter1 (+94 more)

### Community 9 - ".SetPixel"
Cohesion: 0.10
Nodes (17): InteractiveLabManager, InteractiveLesson, LessonType, QuizOption, Color, List, StringBuilder, Edge (+9 more)

### Community 10 - "CodeStudioWindow"
Cohesion: 0.11
Nodes (8): CodeStudioWindow, bool, DispatcherTimer, int, KeyEventArgs, List, RoutedEventArgs, TextChangedEventArgs

### Community 11 - "Window"
Cohesion: 0.10
Nodes (32): BrdStudioQuizFeedback, BtnBorder, ColStudioCanvas, ColStudioCode, ColStudioSplitter1, ColStudioSplitter2, ColStudioTrack, PnlStudioLiveXamlContainer (+24 more)

### Community 12 - "Slider"
Cohesion: 0.13
Nodes (19): Slider3DAmbient, Slider3DSpecular, SliderBrightness, SliderContrast, SliderGamma, SliderLab1, SliderLab2, SliderLab3 (+11 more)

### Community 13 - ".RunTestsAndEvaluateAsync"
Cohesion: 0.17
Nodes (15): Action, CustomScriptGlobals, CustomScriptResult, EvaluationReport, LiveCodeCompiler, TestResult, XamlEvaluationResult, List (+7 more)

### Community 14 - "Application"
Cohesion: 0.12
Nodes (20): IsSubmenuOpen, Application, Arrow, Border, BtnBorder, ContentSite, DropDownBorder, DropDownScrollViewer (+12 more)

### Community 15 - "dependencies"
Cohesion: 0.08
Nodes (24): astro, astro-mermaid, @astrojs/starlight, dependencies, astro, astro-mermaid, @astrojs/starlight, katex (+16 more)

### Community 16 - "ProjectStudioWindow"
Cohesion: 0.18
Nodes (9): BtnMaximize, StudioControl, Window, ProjectStudioWindow, KeyEventArgs, MouseButtonEventArgs, RoutedEventArgs, Button (+1 more)

### Community 17 - ".GetPlainText"
Cohesion: 0.18
Nodes (11): CSharpSyntaxHighlighter, Regex, RichTextBox, SolidColorBrush, XamlSyntaxHighlighter, Regex, RichTextBox, SolidColorBrush (+3 more)

### Community 18 - "Button"
Cohesion: 0.07
Nodes (17): BtnFocusCode, BtnGoToLaboratorioLesson, BtnGoToStudyTheory, BtnMaximize, BtnNextLesson, BtnPrevLesson, BtnQuizOpt0, BtnQuizOpt1 (+9 more)

### Community 19 - "Academic Documentation & CG2D Algorithms"
Cohesion: 0.11
Nodes (19): Curriculum Mapping Table (Official Syllabus to Code), Study Guide for T1, T2, T3 Assessments, 2D Homogeneous Coordinates and Affine Transforms, Bresenham Line Drawing Algorithm, Line Drawing Algorithms Documentation, Xiaolin Wu Anti-Aliased Line Algorithm, Cubic Bezier Curves (de Casteljau), Circles, Ellipses and Bezier Curves Documentation (+11 more)

### Community 20 - "HierarchicalRobotArm"
Cohesion: 0.12
Nodes (11): HierarchicalRobotArm, SceneNode3D, Color, GeometryModel3D, List, Model3DGroup, Graphics3DTests, Fact (+3 more)

### Community 21 - "ProjectStudioControl"
Cohesion: 0.12
Nodes (12): BtnPopoutStudio, LstProjectTemplates, ProjectStudioControl, bool, DispatcherTimer, EventArgs, List, RoutedEventArgs (+4 more)

### Community 22 - ".Apply2DTransform"
Cohesion: 0.19
Nodes (5): Matrix3x3, double, Point, Rasterization2DTests, Fact

### Community 23 - "Hierarchy & Ray Tracing Docs"
Cohesion: 0.15
Nodes (13): 4-DOF Articulated Robot Arm Demo, Hierarchical Solar System Simulation, Parent-Child Matrix Propagation in Scene Graph, Scene Graph Theory and Forward Kinematics, Ray Tracing Fundamentals and Physics of Light, Phong Illumination Model, Shadow Rays for Visibility Testing, Analytical Ray Intersection Documentation (+5 more)

### Community 24 - "Wiki Curriculum Chapters"
Cohesion: 0.21
Nodes (12): BGRA32 Pixel Format (Wiki), Chapter 1: Memory Fundamentals and Unsafe Pointers, Chapter 2: Digital Image Processing, Whitted Ray Tracer Algorithm (Wiki), Chapter 3: 2D Computer Graphics and Rasterization, Chapter 4: 3D Computer Graphics and Pipeline, MVP Pipeline Implementation Reference (Wiki), Chapter 5: Hierarchical Modeling and Forward Kinematics (+4 more)

### Community 25 - ".RenderToTextBlock"
Cohesion: 0.11
Nodes (13): MathFormulaRenderer, SolidColorBrush, Cmb3DShapes, CmbInterpolation, CmbSoftMesh, LstInteractiveLessons, LstStudyTopics, SelectionChangedEventArgs (+5 more)

### Community 26 - ".BtnStudioQuizOption_Click"
Cohesion: 0.28
Nodes (6): BtnStudioQuizOpt0, BtnStudioQuizOpt1, BtnStudioQuizOpt2, BtnStudioToggleCanvas, BtnStudioToggleTrack, Button

### Community 27 - ".UpdateHistogram"
Cohesion: 0.25
Nodes (4): CanvasHistogram, Color, SizeChangedEventArgs, Canvas

### Community 28 - "TextBox"
Cohesion: 0.22
Nodes (6): TxtCompilerReport, TxtLabConsole, TxtLabExplanation, TxtSearchStudy, TextChangedEventArgs, TextBox

### Community 29 - ".MainTabControl_SelectionChanged"
Cohesion: 0.40
Nodes (4): MainTabControl, TabLabEditor, TabLabVisualizer, TabControl

### Community 30 - ".GetTemplates"
Cohesion: 0.40
Nodes (3): ProjectTemplate, ProjectTemplatesManager, List

### Community 31 - ".ChkRobotAnim_CheckedChanged"
Cohesion: 0.29
Nodes (4): Chk3DAutoRotate, ChkRobotAnim, ChkSoftWireframe, CheckBox

### Community 32 - ".UpdateStatus"
Cohesion: 0.12
Nodes (4): BtnResetColumns, RbCameraOrthographic, RbCameraPerspective, RadioButton

### Community 33 - "DirectBitmap Class Documentation"
Cohesion: 0.40
Nodes (6): DirectBitmap Class Documentation, Parallel.For Multicore Pixel Processing, DirectBitmap.SetPixel (Unsafe Pointer Pixel Write), Image Memory Model (Stride, BGRA32), Stride Memory Layout for 2D Images, Unsafe Byte Pointer Memory Access

### Community 34 - "CGPDI.StudyLab.Tests"
Cohesion: 0.17
Nodes (11): net10.0-windows, Microsoft.NET.Sdk, CGPDI.StudyLab.Tests, net10.0-windows, Microsoft.NET.Sdk, FluentAssertions (8.0.1), Microsoft.CodeAnalysis.CSharp.Scripting (5.6.0), Microsoft.NET.Test.Sdk (17.13.0) (+3 more)

### Community 35 - "ColorSpaces"
Cohesion: 0.20
Nodes (3): ColorSpaces, GrayscaleMethod, Color

### Community 36 - "Software 3D Renderer (CPU) Documentation"
Cohesion: 0.40
Nodes (5): Back-face Culling Algorithm, Software 3D Renderer (CPU) Documentation, Z-Buffer Depth Testing Algorithm, Arcball Orbital Camera Control, WPF Viewport3D Hardware Rendering Documentation

### Community 37 - "tsconfig.json"
Cohesion: 0.40
Nodes (4): compilerOptions, moduleResolution, extends, astro/tsconfigs/strict

### Community 38 - "MathRendererAndSyntaxTests"
Cohesion: 0.38
Nodes (3): MathRendererAndSyntaxTests, Fact, WpfFact

### Community 39 - "AppIconHelper"
Cohesion: 0.43
Nodes (3): AppIconHelper, ImageSource, RenderTargetBitmap

### Community 41 - "WPF and XAML Rendering Explanation"
Cohesion: 0.50
Nodes (4): Architecture Overview (Layered, Parallel Processing), WPF Viewport3D Hardware Rendering, WPF and XAML Rendering Explanation, WriteableBitmap Lock/Unlock Cycle

### Community 42 - "Edge Detection Documentation"
Cohesion: 0.50
Nodes (4): Canny Edge Detector (5-Step Algorithm), Edge Detection Documentation, Sobel Edge Detection Operator, 2D Spatial Convolution and Filters

### Community 43 - "Morphological Operations and Otsu Documentation"
Cohesion: 0.50
Nodes (4): Morphological Operations and Otsu Documentation, Mathematical Morphology (Erosion and Dilation), Otsu Automatic Thresholding, Histogram Equalization and Point Operations

### Community 44 - "Graphify Knowledge Graph Rule"
Cohesion: 0.67
Nodes (3): Graphify Knowledge Graph Rule, Graphify Workflow, AGENTS.md Graphify Configuration

### Community 45 - "App Build and Release Pipeline"
Cohesion: 0.67
Nodes (3): CodeQL Security Analysis Workflow, Documentation Deployment Workflow, App Build and Release Pipeline

### Community 46 - "CGPDI StudyLab Logo (SVG)"
Cohesion: 0.67
Nodes (3): CGPDI StudyLab Full Logo with Text (SVG), CGPDI StudyLab Logo (SVG), Documentation Site Logo (SVG)

### Community 47 - "CGPDI.StudyLab.Views"
Cohesion: 0.16
Nodes (6): LaboratorioUiTests, UIFact, MainWindowUiTests, UIFact, CGPDI.StudyLab.Views, CGPDI.StudyLab.Tests.UiTests

### Community 48 - "Flood Fill (Queue-based)"
Cohesion: 0.67
Nodes (3): Cohen-Sutherland Line Clipping Algorithm, Flood Fill (Queue-based), Scanline Polygon Fill Algorithm (AET)

### Community 49 - "Geometric Transformations Documentation"
Cohesion: 0.67
Nodes (3): Bilinear Spatial Interpolation, Geometric Transformations Documentation, Geometric Transformations Inverse Mapping

### Community 51 - ".LstStudioLessons_SelectionChanged"
Cohesion: 0.29
Nodes (4): LstStudioLessons, Button, SelectionChangedEventArgs, ListBox

### Community 74 - "StudioSplitter1"
Cohesion: 0.67
Nodes (3): StudioSplitter1, StudioSplitter2, GridSplitter

### Community 75 - "TabStudioEditor"
Cohesion: 0.50
Nodes (3): TabStudioEditor, TabStudioVisualizer, TabControl

### Community 76 - "StudyTopic"
Cohesion: 0.52
Nodes (5): DocReference, StudyGuideData, StudyQuiz, StudyTopic, List

### Community 77 - ".SliderFree_ValueChanged"
Cohesion: 0.43
Nodes (6): SliderFree1, SliderFree2, SliderFree3, SliderFree4, RoutedPropertyChangedEventArgs, Slider

### Community 78 - ".SliderStudio_ValueChanged"
Cohesion: 0.47
Nodes (5): SliderStudio1, SliderStudio2, SliderStudio3, RoutedPropertyChangedEventArgs, Slider

### Community 80 - "MainWindow.xaml.cs"
Cohesion: 0.20
Nodes (4): App, CGPDI.StudyLab.Graphics3D, CGPDI.StudyLab, StartupEventArgs

### Community 85 - "RichTextBox"
Cohesion: 0.50
Nodes (4): RtbStudioCode, RtbStudioEditableCode, RtbStudioXamlCode, RichTextBox

### Community 86 - "TextBox"
Cohesion: 0.50
Nodes (4): TxtStudioCompilerReport, TxtStudioConsole, TxtStudioExplanation, TextBox

### Community 87 - "ToggleButton"
Cohesion: 0.67
Nodes (3): ToggleButton, IsDropDownOpen, ToggleButton

### Community 88 - "TabStudioEditor"
Cohesion: 0.67
Nodes (3): TabStudioEditor, TabStudioVisualizer, TabControl

## Knowledge Gaps
- **134 isolated node(s):** `net10.0-windows`, `Microsoft.NET.Test.Sdk (17.13.0)`, `xunit (2.9.3)`, `xunit.runner.visualstudio (3.0.2)`, `Xunit.StaFact (1.2.69)` (+129 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **28 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `MainWindow` connect `MainWindow` to `DirectBitmap`, `.UpdateContextualTopBar`, `Vec3`, `WpfViewport3DManager`, `Window`, `Window`, `.SetPixel`, `Slider`, `.GetPlainText`, `Button`, `HierarchicalRobotArm`, `.Apply2DTransform`, `.RenderToTextBlock`, `.UpdateHistogram`, `TextBox`, `.MainTabControl_SelectionChanged`, `.ChkRobotAnim_CheckedChanged`, `.UpdateStatus`, `MainWindow.xaml.cs`?**
  _High betweenness centrality (0.287) - this node is a cross-community bridge._
- **Why does `Window` connect `Window` to `.UpdateStatus`, `.UpdateContextualTopBar`, `MainWindow`, `DirectBitmap`, `Slider`, `Button`, `.Apply2DTransform`, `ToggleButton`, `.RenderToTextBlock`, `.UpdateHistogram`, `TextBox`, `.MainTabControl_SelectionChanged`, `.ChkRobotAnim_CheckedChanged`?**
  _High betweenness centrality (0.214) - this node is a cross-community bridge._
- **Why does `DirectBitmap` connect `DirectBitmap` to `.UpdateContextualTopBar`, `MainWindow`, `Vec3`, `CGPDI.StudyLab.Core`, `.SetPixel`, `CodeStudioWindow`, `.RunTestsAndEvaluateAsync`, `ProjectStudioControl`?**
  _High betweenness centrality (0.200) - this node is a cross-community bridge._
- **What connects `net10.0-windows`, `Microsoft.NET.Test.Sdk (17.13.0)`, `xunit (2.9.3)` to the rest of the system?**
  _134 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `DirectBitmap` be split into smaller, more focused modules?**
  _Cohesion score 0.05694011768778124 - nodes in this community are weakly interconnected._
- **Should `.UpdateContextualTopBar` be split into smaller, more focused modules?**
  _Cohesion score 0.11088709677419355 - nodes in this community are weakly interconnected._
- **Should `MainWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.0546984572230014 - nodes in this community are weakly interconnected._