# Graph Report - CGPDI.StudyLab  (2026-08-16)

## Corpus Check
- 33 files · ~112,884 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1268 nodes · 2632 edges · 109 communities (68 shown, 41 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 153 edges (avg confidence: 0.82)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- .Lock
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
- .SetCode
- Button
- Academic Documentation & CG2D Algorithms
- CGPDI.StudyLab.Graphics3D
- ProjectStudioControl
- .Apply2DTransform
- Hierarchy & Ray Tracing Docs
- Wiki Curriculum Chapters
- .RenderToTextBlock
- .Update2DTheory
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
- DirectBitmap
- .Clear
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
- .SetCode
- content.config.ts
- Auto-Update System (UpdateManager.cs)
- build_release.ps1
- AssemblyInfo.cs
- CGPDI StudyLab Logo (PNG)
- astro.config.mjs
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
- .HsvToRgb
- ImgStudioSimulation
- PbStudioProgress
- GeometricTransforms
- TextBox
- ToggleButton
- Morphology
- Regras do Projeto CGPDI.StudyLab
- TabItemStudioLiveXaml
- Border
- CGPDI.StudyLab — Regras específicas do Antigravity
- Border
- opencode.json
- Edge
- BtnPopoutStudio
- CbResolution
- ImgFreeSimulation
- LstProjectTemplates
- TxtFreeConsole
- Community 102
- Community 103
- Community 104
- Community 105
- Community 106
- Community 107
- Community 108

## God Nodes (most connected - your core abstractions)
1. `Window` - 242 edges
2. `MainWindow` - 197 edges
3. `DirectBitmap` - 90 edges
4. `Window` - 74 edges
5. `TextBlock` - 66 edges
6. `CodeStudioWindow` - 45 edges
7. `UserControl` - 37 edges
8. `WpfViewport3DManager` - 34 edges
9. `CGPDI.StudyLab.Core` - 33 edges
10. `ProjectStudioControl` - 25 edges

## Surprising Connections (you probably didn't know these)
- `CGPDI StudyLab Logo (SVG)` --semantically_similar_to--> `Documentation Site Logo (SVG)`  [INFERRED] [semantically similar]
  CGPDI.StudyLab/Assets/logo.svg → docs/src/assets/logo.svg
- `Mandatory test rule` --semantically_similar_to--> `Mandatory test rule`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md
- `WindowStyle=None window pattern` --semantically_similar_to--> `WindowStyle=None window pattern`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md
- `ProjectStudioWindow` --semantically_similar_to--> `ProjectStudioWindow`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md
- `CodeStudioWindow` --semantically_similar_to--> `CodeStudioWindow`  [INFERRED] [semantically similar]
  AGENTS.md → .agents/rules/project.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Studio and Laboratory window flow** — agents_rules_project_mainwindow, agents_rules_project_projectstudiowindow, agents_rules_project_codestudiowindow [EXTRACTED 0.95]
- **Build and test quality gate** — github_workflows_release_app_ci_test_gate [EXTRACTED 0.95]
- **Wiki Curriculum Chapter Sequence** — wiki_1_fundamentos_e_manipulacao_de_memoria_memory_chapter, wiki_2_processamento_digital_de_imagens_pdi_chapter, wiki_3_computacao_grafica_2d_e_rasterizacao_cg2d_chapter, wiki_4_computacao_grafica_3d_e_pipeline_cg3d_chapter, wiki_5_modelagem_hierarquica_e_cinematica_direta_hierarchy_chapter, wiki_6_ray_tracing_e_renderizacao_realistica_raytracing_chapter [EXTRACTED 0.95]
- **2D Rasterization Algorithm Family** — docs_src_content_docs_cg2d_algoritmos_de_linhas_bresenham, docs_src_content_docs_cg2d_circulos_elipses_e_curvas_midpoint_circle, docs_src_content_docs_cg2d_preenchimento_e_recorte_scanline, docs_src_content_docs_cg2d_algoritmos_de_linhas_xiaolin_wu [INFERRED 0.85]
- **PDI Image Processing Filter Pipeline** — docs_src_content_docs_pdi_filtros_espaciais_e_convolucoes_convolution, docs_src_content_docs_pdi_deteccao_de_bordas_e_canny_canny_edge, docs_src_content_docs_pdi_morfologia_matematica_e_otsu_otsu, docs_src_content_docs_pdi_operacoes_pontuais_e_histogramas_histogram [INFERRED 0.85]
- **Ray Tracing Full Optical Simulation Model** — docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_phong_model, docs_src_content_docs_raytracing_fundamentos_e_fisica_da_luz_shadow_rays, docs_src_content_docs_raytracing_reflexao_refracao_snell_snell_law, docs_src_content_docs_raytracing_reflexao_refracao_snell_fresnel [INFERRED 0.85]
- **High-Performance CPU Rendering Core** — docs_src_content_docs_core_directbitmap_setpixel, docs_src_content_docs_core_directbitmap_parallel_for, docs_src_content_docs_core_fundamentos_de_memoria_unsafe_pointers, docs_src_content_docs_core_fundamentos_de_memoria_stride [INFERRED 0.95]

## Communities (109 total, 41 thin omitted)

### Community 0 - ".Lock"
Cohesion: 0.05
Nodes (28): BitmapSource, byte, DirectBitmap, bool, Color, ImageSampleGenerator, InteractiveLabManager, Color (+20 more)

### Community 1 - ".UpdateContextualTopBar"
Cohesion: 0.07
Nodes (7): Description, Window, Title, Url, RoutedEventArgs, Func, InterpolationMode

### Community 2 - "MainWindow"
Cohesion: 0.04
Nodes (51): TxtChallengeGoal, TxtDocCategory, TxtDocComplexity, TxtDocExplanation, TxtDocMath, TxtDocSummary, TxtDocTitle, TxtDocWhereToTest (+43 more)

### Community 3 - "Vec3"
Cohesion: 0.09
Nodes (16): Mat4x4, Ray3D, Vec3, Vec4, double, MaterialRay, PlaneObject, PointLight (+8 more)

### Community 4 - "WpfViewport3DManager"
Cohesion: 0.07
Nodes (20): AmbientLight, WpfViewport3DManager, bool, Color, double, MouseButtonEventArgs, Point, Viewport3DAndXamlTests (+12 more)

### Community 5 - "CGPDI.StudyLab.Core"
Cohesion: 0.06
Nodes (34): CancellationToken, CancellationTokenSource, ReleaseInfo, UpdateManager, string, Task, PbLessonProgress, PnlContextualTopActions (+26 more)

### Community 6 - "Window"
Cohesion: 0.13
Nodes (19): Action, CustomScriptGlobals, CustomScriptResult, EvaluationReport, LiveCodeCompiler, TestResult, XamlEvaluationResult, DirectBitmap (+11 more)

### Community 7 - "UserControl"
Cohesion: 0.06
Nodes (22): MathFormulaRenderer, SolidColorBrush, Cmb3DShapes, CmbInterpolation, CmbSoftMesh, LstInteractiveLessons, LstStudyTopics, MainTabControl (+14 more)

### Community 8 - "Window"
Cohesion: 0.08
Nodes (18): MainStudioControl, ProjectStudioControl, LaboratorioUiTests, UIFact, MainWindowUiTests, UIFact, ProjectStudioUiTests, UIFact (+10 more)

### Community 9 - ".SetPixel"
Cohesion: 0.08
Nodes (11): BtnMaximize, MainWindow, bool, DispatcherTimer, double, int, List, Point (+3 more)

### Community 10 - "CodeStudioWindow"
Cohesion: 0.10
Nodes (24): Slider3DAmbient, Slider3DSpecular, SliderBrightness, SliderContrast, SliderGamma, SliderLab1, SliderLab2, SliderLab3 (+16 more)

### Community 11 - "Window"
Cohesion: 0.12
Nodes (8): CodeStudioWindow, bool, DirectBitmap, DispatcherTimer, int, List, RoutedEventArgs, TextChangedEventArgs

### Community 13 - ".RunTestsAndEvaluateAsync"
Cohesion: 0.12
Nodes (11): LstProjectTemplates, TabStudioEditor, ProjectStudioControl, bool, DirectBitmap, DispatcherTimer, List, RoutedEventArgs (+3 more)

### Community 14 - "Application"
Cohesion: 0.08
Nodes (25): Category, BtnBorder, CbResolution, ImgFreeSimulation, PnlFreeLiveXamlContainer, TabBorder, TxtFreeConsole, TxtFreeParam1 (+17 more)

### Community 15 - "dependencies"
Cohesion: 0.08
Nodes (24): astro, astro-mermaid, @astrojs/starlight, dependencies, astro, astro-mermaid, @astrojs/starlight, katex (+16 more)

### Community 16 - "ProjectStudioWindow"
Cohesion: 0.09
Nodes (15): BtnFocusCode, BtnGoToLaboratorioLesson, BtnGoToStudyTheory, BtnNextLesson, BtnPrevLesson, BtnQuizOpt0, BtnQuizOpt1, BtnQuizOpt2 (+7 more)

### Community 17 - ".SetCode"
Cohesion: 0.11
Nodes (23): IsDropDownOpen, IsSubmenuOpen, Application, Arrow, Border, BtnBorder, ContentSite, DropDownBorder (+15 more)

### Community 18 - "Button"
Cohesion: 0.13
Nodes (11): CSharpSyntaxHighlighter, SolidColorBrush, EventArgs, MathRendererAndSyntaxTests, Fact, WpfFact, EventArgs, EventArgs (+3 more)

### Community 19 - "Academic Documentation & CG2D Algorithms"
Cohesion: 0.14
Nodes (16): Module, BrdStudioQuizFeedback, BtnBorder, ImgStudioSimulation, PnlStudioLiveXamlContainer, TabBorder, TxtStudioCompilerReport, TxtStudioConsole (+8 more)

### Community 20 - "CGPDI.StudyLab.Graphics3D"
Cohesion: 0.15
Nodes (4): BtnRunTests, BtnRunUserCode, RbCameraOrthographic, RbCameraPerspective

### Community 21 - "ProjectStudioControl"
Cohesion: 0.11
Nodes (19): Curriculum Mapping Table (Official Syllabus to Code), Study Guide for T1, T2, T3 Assessments, 2D Homogeneous Coordinates and Affine Transforms, Bresenham Line Drawing Algorithm, Line Drawing Algorithms Documentation, Xiaolin Wu Anti-Aliased Line Algorithm, Cubic Bezier Curves (de Casteljau), Circles, Ellipses and Bezier Curves Documentation (+11 more)

### Community 22 - ".Apply2DTransform"
Cohesion: 0.14
Nodes (17): graphify knowledge graph rules, Mandatory test rule, WindowStyle=None window pattern, Mandatory test rule, WindowStyle=None window pattern, Google Antigravity agent, Dark palette colors, dotnet build command (+9 more)

### Community 23 - "Hierarchy & Ray Tracing Docs"
Cohesion: 0.17
Nodes (9): HierarchicalRobotArm, SceneNode3D, Color, GeometryModel3D, List, Model3DGroup, Point3D, RotateTransform3D (+1 more)

### Community 24 - "Wiki Curriculum Chapters"
Cohesion: 0.16
Nodes (4): ColorSpaces, GrayscaleMethod, Color, GrayscaleMethod

### Community 25 - ".RenderToTextBlock"
Cohesion: 0.14
Nodes (14): RtbDocSnippet, RtbLabCode, RtbLabEditableCode, RtbLabXamlCode, RtbTheory2DCodeSnippet, RtbTheory3DCodeSnippet, RtbTheoryCodeSnippet, RtbTheoryRayCodeSnippet (+6 more)

### Community 27 - ".Update2DTheory"
Cohesion: 0.15
Nodes (13): 4-DOF Articulated Robot Arm Demo, Hierarchical Solar System Simulation, Parent-Child Matrix Propagation in Scene Graph, Scene Graph Theory and Forward Kinematics, Ray Tracing Fundamentals and Physics of Light, Phong Illumination Model, Shadow Rays for Visibility Testing, Analytical Ray Intersection Documentation (+5 more)

### Community 28 - "TextBox"
Cohesion: 0.23
Nodes (4): AlgorithmCodeSnippets, CGPDI.StudyLab.Core, CGPDI.StudyLab.ImageProcessing, string

### Community 29 - ".MainTabControl_SelectionChanged"
Cohesion: 0.20
Nodes (4): QuizAndCurriculumTests, Fact, CGPDI.StudyLab.Graphics3D, CGPDI.StudyLab.Tests.UnitTests

### Community 30 - ".GetTemplates"
Cohesion: 0.17
Nodes (12): TabItem2D, TabItem3D, TabItemCentralEstudos, TabItemEstudio, TabItemLabLiveXaml, TabItemLaboratorio, TabItemLabXaml, TabItemPdi (+4 more)

### Community 31 - ".ChkRobotAnim_CheckedChanged"
Cohesion: 0.21
Nodes (12): BGRA32 Pixel Format (Wiki), Chapter 1: Memory Fundamentals and Unsafe Pointers, Chapter 2: Digital Image Processing, Whitted Ray Tracer Algorithm (Wiki), Chapter 3: 2D Computer Graphics and Rasterization, Chapter 4: 3D Computer Graphics and Pipeline, MVP Pipeline Implementation Reference (Wiki), Chapter 5: Hierarchical Modeling and Forward Kinematics (+4 more)

### Community 32 - ".UpdateStatus"
Cohesion: 0.18
Nodes (4): Matrix3x3, double, Point, CGPDI.StudyLab.Graphics2D

### Community 33 - "DirectBitmap Class Documentation"
Cohesion: 0.18
Nodes (11): ColLabCode, ColLabPlayground, ColLabSplitter1, ColLabSplitter2, ColLabTrack, ColumnDefinition, ColStudioCanvas, ColStudioCode (+3 more)

### Community 34 - "CGPDI.StudyLab.Tests"
Cohesion: 0.22
Nodes (9): CGPDI.StudyLab, CGPDI.StudyLab.Tests, net10.0-windows, FluentAssertions (8.0.1), Microsoft.NET.Test.Sdk (17.13.0), xunit (2.9.3), xunit.runner.visualstudio (3.0.2), Xunit.StaFact (1.2.69) (+1 more)

### Community 35 - "ColorSpaces"
Cohesion: 0.31
Nodes (4): XamlSyntaxHighlighter, Regex, RichTextBox, SolidColorBrush

### Community 36 - "Software 3D Renderer (CPU) Documentation"
Cohesion: 0.22
Nodes (9): CodeStudioWindow, MainWindow, ProjectStudioWindow, WpfViewport3DManager.RotateCamera, CodeStudioWindow, MainWindow, ProjectStudioWindow, _timer3D auto-rotation timer (+1 more)

### Community 37 - "tsconfig.json"
Cohesion: 0.22
Nodes (6): TxtCompilerReport, TxtLabConsole, TxtLabExplanation, TxtSearchStudy, TextChangedEventArgs, TextBox

### Community 38 - "DirectBitmap"
Cohesion: 0.28
Nodes (6): BtnStudioQuizOpt0, BtnStudioQuizOpt1, BtnStudioQuizOpt2, BtnStudioToggleCanvas, BtnStudioToggleTrack, Button

### Community 39 - ".Clear"
Cohesion: 0.25
Nodes (4): LstStudioLessons, Button, InteractiveLesson, SelectionChangedEventArgs

### Community 40 - ".BtnMaximize_Click"
Cohesion: 0.43
Nodes (3): AppIconHelper, ImageSource, RenderTargetBitmap

### Community 41 - "WPF and XAML Rendering Explanation"
Cohesion: 0.25
Nodes (8): Border, BrdQuizFeedback, BrdStudyTopicQuizFeedback, DropDownBorder, ItemBorder, PnlLabLiveXamlContainer, TabBorder, Border

### Community 42 - "Edge Detection Documentation"
Cohesion: 0.25
Nodes (4): CanvasHistogram, Color, SizeChangedEventArgs, Canvas

### Community 45 - "App Build and Release Pipeline"
Cohesion: 0.29
Nodes (6): Comandos obrigatórios, Consistência de estilo (NÃO quebrar o que funciona), Convenções de código, Fluxo de janelas / Estúdio / Laboratório, Regra de testes (obrigatória), Regras do Projeto CGPDI.StudyLab

### Community 46 - "CGPDI StudyLab Logo (SVG)"
Cohesion: 0.29
Nodes (4): Application, App, CGPDI.StudyLab, StartupEventArgs

### Community 47 - "CGPDI.StudyLab.Views"
Cohesion: 0.52
Nodes (5): DocReference, StudyGuideData, StudyQuiz, StudyTopic, List

### Community 48 - "Flood Fill (Queue-based)"
Cohesion: 0.29
Nodes (4): Chk3DAutoRotate, ChkRobotAnim, ChkSoftWireframe, CheckBox

### Community 49 - "Geometric Transformations Documentation"
Cohesion: 0.29
Nodes (6): ImgDisplay, ImgDisplay2D, ImgDisplay3DSoft, ImgLabSimulation, MouseButtonEventArgs, Image

### Community 51 - ".SetCode"
Cohesion: 0.33
Nodes (6): CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter, CSharpSyntaxHighlighter, MathFormulaRenderer, XamlSyntaxHighlighter

### Community 52 - "content.config.ts"
Cohesion: 0.53
Nodes (4): InteractiveLesson, LessonType, QuizOption, List

### Community 54 - "build_release.ps1"
Cohesion: 0.40
Nodes (6): DirectBitmap Class Documentation, Parallel.For Multicore Pixel Processing, DirectBitmap.SetPixel (Unsafe Pointer Pixel Write), Image Memory Model (Stride, BGRA32), Stride Memory Layout for 2D Images, Unsafe Byte Pointer Memory Access

### Community 55 - "AssemblyInfo.cs"
Cohesion: 0.40
Nodes (5): DirectBitmap, LiveCodeCompiler, Namespace conventions, DirectBitmap, LiveCodeCompiler

### Community 56 - "CGPDI StudyLab Logo (PNG)"
Cohesion: 0.50
Nodes (3): ProjectTemplate, ProjectTemplatesManager, List

### Community 57 - "astro.config.mjs"
Cohesion: 0.40
Nodes (5): Splitter1, Splitter2, GridSplitter, StudioSplitter1, StudioSplitter2

### Community 59 - "Linear Algebra and Matrices for CG2D"
Cohesion: 0.40
Nodes (4): SliderStudio1, SliderStudio2, SliderStudio3, RoutedPropertyChangedEventArgs

### Community 60 - "Synthetic Sample Image Generator"
Cohesion: 0.40
Nodes (5): Back-face Culling Algorithm, Software 3D Renderer (CPU) Documentation, Z-Buffer Depth Testing Algorithm, Arcball Orbital Camera Control, WPF Viewport3D Hardware Rendering Documentation

### Community 61 - "Color Space Models (RGB, HSV, YCbCr, CMYK)"
Cohesion: 0.40
Nodes (4): compilerOptions, moduleResolution, extends, astro/tsconfigs/strict

### Community 62 - "Sepia Photographic Effect (Matrix Transform)"
Cohesion: 0.40
Nodes (4): Antes de qualquer mudança, CGPDI.StudyLab — Regras específicas do Antigravity, Consistência de estilo (prioridade máxima), Gate de qualidade

### Community 63 - "Documentation Landing Page (StudyLab Overview)"
Cohesion: 0.50
Nodes (4): Architecture Overview (Layered, Parallel Processing), WPF Viewport3D Hardware Rendering, WPF and XAML Rendering Explanation, WriteableBitmap Lock/Unlock Cycle

### Community 64 - "Debugging and Tips for Beginners"
Cohesion: 0.50
Nodes (4): Canny Edge Detector (5-Step Algorithm), Edge Detection Documentation, Sobel Edge Detection Operator, 2D Spatial Convolution and Filters

### Community 65 - "Command Line Interface Guide"
Cohesion: 0.50
Nodes (4): Morphological Operations and Otsu Documentation, Mathematical Morphology (Erosion and Dilation), Otsu Automatic Thresholding, Histogram Equalization and Point Operations

### Community 66 - "Visual Studio Installation Guide"
Cohesion: 0.50
Nodes (3): plugin, $schema, .opencode/plugins/graphify.js

### Community 67 - "Interactive Mode and Playground Guide"
Cohesion: 0.67
Nodes (3): CGPDI StudyLab Full Logo with Text (SVG), CGPDI StudyLab Logo (SVG), Documentation Site Logo (SVG)

### Community 68 - "Introduction to .NET and C# for Students"
Cohesion: 0.67
Nodes (3): IcMicrosoftRefs, ItemsControl, IcStudioMsRefs

### Community 70 - "Fourier Transform Frequency Domain Analysis"
Cohesion: 0.67
Nodes (3): Cohen-Sutherland Line Clipping Algorithm, Flood Fill (Queue-based), Scanline Polygon Fill Algorithm (AET)

### Community 71 - "Spatial Filters and Convolution Documentation"
Cohesion: 0.67
Nodes (3): Bilinear Spatial Interpolation, Geometric Transformations Documentation, Geometric Transformations Inverse Mapping

## Knowledge Gaps
- **128 isolated node(s):** `OutCode`, `GrayscaleMethod`, `IsSubmenuOpen`, `Path`, `name` (+123 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **41 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `CGPDI.StudyLab.Core` connect `TextBox` to `.UpdateStatus`, `ColorSpaces`, `Vec3`, `CGPDI.StudyLab.Core`, `Window`, `UserControl`, `.BtnMaximize_Click`, `Window`, `Morphological Operations and Otsu Documentation`, `Graphify Knowledge Graph Rule`, `CGPDI StudyLab Logo (SVG)`, `CGPDI.StudyLab.Views`, `Button`, `content.config.ts`, `Wiki Curriculum Chapters`, `CGPDI StudyLab Logo (PNG)`, `.MainTabControl_SelectionChanged`?**
  _High betweenness centrality (0.270) - this node is a cross-community bridge._
- **Why does `MainWindow` connect `.SetPixel` to `.UpdateContextualTopBar`, `WpfViewport3DManager`, `tsconfig.json`, `CGPDI.StudyLab.Core`, `UserControl`, `Edge Detection Documentation`, `CodeStudioWindow`, `Slider`, `CGPDI StudyLab Logo (SVG)`, `ProjectStudioWindow`, `Flood Fill (Queue-based)`, `Button`, `Geometric Transformations Documentation`, `CGPDI.StudyLab.Graphics3D`, `.BtnStudioQuizOption_Click`?**
  _High betweenness centrality (0.255) - this node is a cross-community bridge._
- **Why does `Window` connect `.UpdateContextualTopBar` to `MainWindow`, `CGPDI.StudyLab.Core`, `UserControl`, `Window`, `.SetPixel`, `CodeStudioWindow`, `Slider`, `Application`, `ProjectStudioWindow`, `.SetCode`, `Academic Documentation & CG2D Algorithms`, `CGPDI.StudyLab.Graphics3D`, `.RenderToTextBlock`, `.BtnStudioQuizOption_Click`, `.GetTemplates`, `DirectBitmap Class Documentation`, `tsconfig.json`, `WPF and XAML Rendering Explanation`, `Edge Detection Documentation`, `Flood Fill (Queue-based)`, `Geometric Transformations Documentation`, `astro.config.mjs`, `Introduction to .NET and C# for Students`, `StudioSplitter1`, `TabStudioEditor`, `StudyTopic`, `.SliderFree_ValueChanged`?**
  _High betweenness centrality (0.190) - this node is a cross-community bridge._
- **What connects `OutCode`, `GrayscaleMethod`, `IsSubmenuOpen` to the rest of the system?**
  _128 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `.Lock` be split into smaller, more focused modules?**
  _Cohesion score 0.05365126676602087 - nodes in this community are weakly interconnected._
- **Should `.UpdateContextualTopBar` be split into smaller, more focused modules?**
  _Cohesion score 0.06736842105263158 - nodes in this community are weakly interconnected._
- **Should `MainWindow` be split into smaller, more focused modules?**
  _Cohesion score 0.0392156862745098 - nodes in this community are weakly interconnected._