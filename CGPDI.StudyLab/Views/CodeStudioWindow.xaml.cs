using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Views
{
    public partial class CodeStudioWindow : BorderlessWindow
    {
        private DirectBitmap _labBitmap = null!;
        private List<InteractiveLesson> _interactiveLessons = new();
        private int _simulationStepIndex = 0;
        private bool _isInitializing = true;
        private bool _isHighlighting = false;
        private readonly DispatcherTimer _highlightDebounceTimer;

        public CodeStudioWindow(int initialLessonNumber = 1)
        {
            InitializeComponent();

            _highlightDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
            _highlightDebounceTimer.Tick += HighlightDebounceTimer_Tick;

            InitLab(initialLessonNumber);
            Loaded += CodeStudioWindow_Loaded;
        }

        private void CodeStudioWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CenterOnScreen();
            _isInitializing = false;
            RtbStudioEditableCode.TextChanged += RtbStudioEditableCode_TextChanged;
            RtbStudioXamlCode.TextChanged += RtbStudioXamlCode_TextChanged;
            UpdateStudioSimulation();
            LoadDefaultXamlSnippet();
        }

        private void InitLab(int initialLessonNumber)
        {
            _labBitmap = new DirectBitmap(512, 512);
            ImgStudioSimulation.Source = _labBitmap.Bitmap;

            _interactiveLessons = InteractiveLabManager.GetLessons();
            LstStudioLessons.ItemsSource = _interactiveLessons;
            if (_interactiveLessons.Count > 0)
            {
                int idx = Math.Clamp(initialLessonNumber - 1, 0, _interactiveLessons.Count - 1);
                LstStudioLessons.SelectedIndex = idx;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 || (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                BtnStudioRunCode_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void RtbStudioEditableCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isHighlighting || _isInitializing) return;
            _highlightDebounceTimer.Stop();
            _highlightDebounceTimer.Start();
        }

        private void RtbStudioXamlCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isHighlighting || _isInitializing) return;
            _highlightDebounceTimer.Stop();
            _highlightDebounceTimer.Start();
        }

        private void HighlightDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _highlightDebounceTimer.Stop();
            if (_isHighlighting) return;
            _isHighlighting = true;

            try
            {
                if (TabStudioEditor.SelectedIndex == 1)
                {
                    int offset = XamlSyntaxHighlighter.GetCaretCharIndex(RtbStudioXamlCode);
                    string xaml = XamlSyntaxHighlighter.GetPlainText(RtbStudioXamlCode);
                    XamlSyntaxHighlighter.Highlight(RtbStudioXamlCode, xaml);
                    XamlSyntaxHighlighter.SetCaretCharIndex(RtbStudioXamlCode, offset);
                }
                else if (TabStudioEditor.SelectedIndex == 0)
                {
                    int offset = CSharpSyntaxHighlighter.GetCaretCharIndex(RtbStudioEditableCode);
                    string code = CSharpSyntaxHighlighter.GetPlainText(RtbStudioEditableCode);
                    CSharpSyntaxHighlighter.Highlight(RtbStudioEditableCode, code);
                    CSharpSyntaxHighlighter.SetCaretCharIndex(RtbStudioEditableCode, offset);
                }
            }
            catch
            {
                // Fallback seguro
            }
            finally
            {
                _isHighlighting = false;
            }
        }

        private void LstStudioLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is not InteractiveLesson lesson) return;

            _simulationStepIndex = 0;

            int currentStep = lesson.Number;
            int totalSteps = _interactiveLessons.Count;
            TxtStudioProgress.Text = $"Progresso: {currentStep} de {totalSteps} ({(currentStep * 100 / totalSteps)}%)";
            TxtStudioModule.Text = lesson.Module.ToUpper();
            PbStudioProgress.Maximum = totalSteps;
            PbStudioProgress.Value = currentStep;

            TxtStudioLessonHeader.Text = $"Lição {currentStep}: {lesson.Title}";
            TxtStudioLessonTitle.Text = lesson.Title;
            TxtStudioLessonSummary.Text = lesson.Summary;

            TxtStudioChallengeGoal.Text = lesson.ChallengeGoal;

            _isHighlighting = true;
            CSharpSyntaxHighlighter.SetCode(RtbStudioEditableCode, lesson.StarterTemplate);

            string solCode = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
            CSharpSyntaxHighlighter.SetCode(RtbStudioCode, solCode);
            if (!string.IsNullOrEmpty(lesson.XamlSnippet))
            {
                XamlSyntaxHighlighter.SetCode(RtbStudioXamlCode, lesson.XamlSnippet);
            }
            _isHighlighting = false;

            TxtStudioExplanation.Text = lesson.CodeExplanation;

            TxtStudioCompilerReport.Text = "Pronto para executar. Clique em 'Compilar e Executar C#' ou 'Executar Testes'.";
            TxtStudioCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));

            ConfigureStudioSliders(lesson);
            LoadStudioQuiz(lesson);

            IcStudioMsRefs.ItemsSource = lesson.MicrosoftReferences;

            if (!_isInitializing)
            {
                UpdateStudioSimulation();
            }
        }

        private void ConfigureStudioSliders(InteractiveLesson lesson)
        {
            SliderStudio1.ValueChanged -= SliderStudio_ValueChanged;
            SliderStudio2.ValueChanged -= SliderStudio_ValueChanged;
            SliderStudio3.ValueChanged -= SliderStudio_ValueChanged;

            switch (lesson.Type)
            {
                case LessonType.BgraMemoryLayout:
                    TxtStudioParam1.Text = "Canal Azul (Blue: 0-255):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 255;
                    TxtStudioParam2.Text = "Canal Verde (Green: 0-255):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 255; SliderStudio2.Value = 120;
                    TxtStudioParam3.Text = "Canal Vermelho (Red: 0-255):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 255; SliderStudio3.Value = 30;
                    break;

                case LessonType.CSharpPropertiesAndNotify:
                    TxtStudioParam1.Text = "Valor Propriedade (0-255):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 140;
                    TxtStudioParam2.Text = "(Data Binding Reativo):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 1;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.PointerStrideOffset:
                    TxtStudioParam1.Text = "Coluna X (0 a 7):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 7; SliderStudio1.Value = 3;
                    TxtStudioParam2.Text = "Linha Y (0 a 7):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 7; SliderStudio2.Value = 2;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.WpfXamlAndDependencyProps:
                    TxtStudioParam1.Text = "Escala Desejada (%):";
                    SliderStudio1.Minimum = 20; SliderStudio1.Maximum = 200; SliderStudio1.Value = 100;
                    TxtStudioParam2.Text = "Espaço Disponível:";
                    SliderStudio2.Minimum = 100; SliderStudio2.Maximum = 450; SliderStudio2.Value = 300;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.WriteableBitmapLifecycle:
                    TxtStudioParam1.Text = "Etapa do Ciclo (1 a 4):";
                    SliderStudio1.Minimum = 1; SliderStudio1.Maximum = 4; SliderStudio1.Value = 1;
                    TxtStudioParam2.Text = "(Buffer Traseiro GPU):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 1;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.ConvolutionStepByStep:
                    TxtStudioParam1.Text = "Posição X Máscara 3x3:";
                    SliderStudio1.Minimum = 1; SliderStudio1.Maximum = 6; SliderStudio1.Value = 2;
                    TxtStudioParam2.Text = "Posição Y Máscara 3x3:";
                    SliderStudio2.Minimum = 1; SliderStudio2.Maximum = 6; SliderStudio2.Value = 2;
                    TxtStudioParam3.Text = "Divisor Normalização:";
                    SliderStudio3.Minimum = 1; SliderStudio3.Maximum = 16; SliderStudio3.Value = 9;
                    break;

                case LessonType.OtsuThresholdSearch:
                    TxtStudioParam1.Text = "Limiar de Corte T (0-255):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 118;
                    TxtStudioParam2.Text = "(Automático):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 0;
                    TxtStudioParam3.Text = "(Automático):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.BresenhamStepByStep:
                    TxtStudioParam1.Text = "Destino X1 (3 a 15):";
                    SliderStudio1.Minimum = 3; SliderStudio1.Maximum = 15; SliderStudio1.Value = 12;
                    TxtStudioParam2.Text = "Destino Y1 (2 a 12):";
                    SliderStudio2.Minimum = 2; SliderStudio2.Maximum = 12; SliderStudio2.Value = 8;
                    TxtStudioParam3.Text = "(Automático):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.MatrixTransform2D:
                    TxtStudioParam1.Text = "Translação X (-100 a +100):";
                    SliderStudio1.Minimum = -100; SliderStudio1.Maximum = 100; SliderStudio1.Value = 0;
                    TxtStudioParam2.Text = "Rotação Angular (0-360°):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 360; SliderStudio2.Value = 45;
                    TxtStudioParam3.Text = "Escala (0.5x a 2.5x):";
                    SliderStudio3.Minimum = 0.5; SliderStudio3.Maximum = 2.5; SliderStudio3.Value = 1.2;
                    break;

                case LessonType.PipelineMVP3D:
                    TxtStudioParam1.Text = "Rotação Y Modelo (0-360°):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 360; SliderStudio1.Value = 45;
                    TxtStudioParam2.Text = "Distância Z Câmera:";
                    SliderStudio2.Minimum = 1.5; SliderStudio2.Maximum = 8.0; SliderStudio2.Value = 3.5;
                    TxtStudioParam3.Text = "Campo de Visão (FOV):";
                    SliderStudio3.Minimum = 30; SliderStudio3.Maximum = 120; SliderStudio3.Value = 60;
                    break;

                case LessonType.HierarchicalSceneGraph:
                    TxtStudioParam1.Text = "Rotação Base (Eixo Y):";
                    SliderStudio1.Minimum = -90; SliderStudio1.Maximum = 90; SliderStudio1.Value = 15;
                    TxtStudioParam2.Text = "Ombro (Eixo Z):";
                    SliderStudio2.Minimum = -60; SliderStudio2.Maximum = 60; SliderStudio2.Value = 35;
                    TxtStudioParam3.Text = "Cotovelo (Eixo Z):";
                    SliderStudio3.Minimum = -90; SliderStudio3.Maximum = 90; SliderStudio3.Value = -40;
                    break;

                case LessonType.RayTracingIntersection:
                    TxtStudioParam1.Text = "Posição Raio (Offset Y):";
                    SliderStudio1.Minimum = -120; SliderStudio1.Maximum = 120; SliderStudio1.Value = -20;
                    TxtStudioParam2.Text = "(Automático):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 0;
                    TxtStudioParam3.Text = "(Automático):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                default:
                    TxtStudioParam1.Text = "Parâmetro 1:";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 128;
                    TxtStudioParam2.Text = "Parâmetro 2:";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 255; SliderStudio2.Value = 128;
                    TxtStudioParam3.Text = "Parâmetro 3:";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 255; SliderStudio3.Value = 128;
                    break;
            }

            TxtStudioVal1.Text = $"[ {SliderStudio1.Value:F0} ]";
            TxtStudioVal2.Text = $"[ {SliderStudio2.Value:F0} ]";
            TxtStudioVal3.Text = $"[ {SliderStudio3.Value:F0} ]";

            SliderStudio1.ValueChanged += SliderStudio_ValueChanged;
            SliderStudio2.ValueChanged += SliderStudio_ValueChanged;
            SliderStudio3.ValueChanged += SliderStudio_ValueChanged;
        }

        private void LoadStudioQuiz(InteractiveLesson lesson)
        {
            TxtStudioQuizQuestion.Text = lesson.QuizQuestion;
            BrdStudioQuizFeedback.Visibility = Visibility.Collapsed;

            ResetQuizButton(BtnStudioQuizOpt0);
            ResetQuizButton(BtnStudioQuizOpt1);
            ResetQuizButton(BtnStudioQuizOpt2);

            if (lesson.QuizOptions.Count > 0)
            {
                BtnStudioQuizOpt0.Content = "A) " + lesson.QuizOptions[0].Text;
                BtnStudioQuizOpt0.Visibility = Visibility.Visible;
            }
            else BtnStudioQuizOpt0.Visibility = Visibility.Collapsed;

            if (lesson.QuizOptions.Count > 1)
            {
                BtnStudioQuizOpt1.Content = "B) " + lesson.QuizOptions[1].Text;
                BtnStudioQuizOpt1.Visibility = Visibility.Visible;
            }
            else BtnStudioQuizOpt1.Visibility = Visibility.Collapsed;

            if (lesson.QuizOptions.Count > 2)
            {
                BtnStudioQuizOpt2.Content = "C) " + lesson.QuizOptions[2].Text;
                BtnStudioQuizOpt2.Visibility = Visibility.Visible;
            }
            else BtnStudioQuizOpt2.Visibility = Visibility.Collapsed;
        }

        private void ResetQuizButton(Button btn)
        {
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E2C"));
            btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33334A"));
            btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8FAFC"));
        }

        private void BtnStudioQuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is not InteractiveLesson lesson || sender is not Button btn) return;

            if (!int.TryParse(btn.Tag?.ToString(), out int optIndex) || optIndex >= lesson.QuizOptions.Count) return;

            var opt = lesson.QuizOptions[optIndex];
            BrdStudioQuizFeedback.Visibility = Visibility.Visible;

            ResetQuizButton(BtnStudioQuizOpt0);
            ResetQuizButton(BtnStudioQuizOpt1);
            ResetQuizButton(BtnStudioQuizOpt2);

            if (opt.IsCorrect)
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1B5E20"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                BrdStudioQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2A20"));
                BrdStudioQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
                TxtStudioQuizFeedback.Text = "✅ RESPOSTA CORRETA!\n" + opt.Explanation;
                TxtStudioQuizFeedback.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
            }
            else
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C1A1A"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935"));
                BrdStudioQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A1E1E"));
                BrdStudioQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7D2E2E"));
                TxtStudioQuizFeedback.Text = "❌ RESPOSTA INCORRETA.\n" + opt.Explanation;
                TxtStudioQuizFeedback.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCA5A5"));
            }
        }

        private void SliderStudio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;

            if (TxtStudioVal1 != null) TxtStudioVal1.Text = $"[ {SliderStudio1.Value:F0} ]";
            if (TxtStudioVal2 != null) TxtStudioVal2.Text = $"[ {SliderStudio2.Value:F0} ]";
            if (TxtStudioVal3 != null) TxtStudioVal3.Text = $"[ {SliderStudio3.Value:F0} ]";

            UpdateStudioSimulation();
        }

        private void UpdateStudioSimulation()
        {
            if (_labBitmap == null || LstStudioLessons.SelectedItem is not InteractiveLesson lesson) return;

            var sw = Stopwatch.StartNew();
            var log = new StringBuilder();

            InteractiveLabManager.RenderSimulation(
                _labBitmap,
                lesson,
                SliderStudio1.Value,
                SliderStudio2.Value,
                SliderStudio3.Value,
                255,
                _simulationStepIndex,
                log);

            sw.Stop();
            TxtStudioConsole.Text = log.ToString();
            TxtStudioStats.Text = $"Tempo: {sw.Elapsed.TotalMilliseconds:F1} ms • Resolução: {_labBitmap.Width}×{_labBitmap.Height}";
            TxtStudioStatus.Text = $"Simulação da Lição {lesson.Number} atualizada.";
        }

        private void BtnStudioStep_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex++;
            UpdateStudioSimulation();
        }

        private void BtnStudioRunAll_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 25;
            UpdateStudioSimulation();
        }

        private void BtnStudioResetSim_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 0;
            if (LstStudioLessons.SelectedItem is InteractiveLesson lesson)
            {
                ConfigureStudioSliders(lesson);
            }
            UpdateStudioSimulation();
        }

        private async void BtnStudioRunCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is not InteractiveLesson lesson) return;

            TxtStudioStatus.Text = "Compilando código C# com Roslyn...";
            string userCode = CSharpSyntaxHighlighter.GetPlainText(RtbStudioEditableCode);

            double p1 = SliderStudio1.Value;
            double p2 = SliderStudio2.Value;
            double p3 = SliderStudio3.Value;

            var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(lesson, userCode, _labBitmap, p1, p2, p3);

            TxtStudioCompilerReport.Text = report.FeedbackReport;
            TxtStudioCompilerReport.Foreground = report.Success
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCA5A5"));

            TxtStudioConsole.Text = string.IsNullOrEmpty(report.ConsoleLogs)
                ? $"[Execução Concluída] Tempo: {report.ExecutionTimeMs:F1} ms."
                : $"[Logs da Lição {lesson.Number}]:\n{report.ConsoleLogs}";

            TxtStudioStatus.Text = report.Success ? "Compilação e execução bem-sucedidas!" : "Erros detectados no código.";
            TxtStudioStats.Text = $"Tempo: {report.ExecutionTimeMs:F1} ms • Resolução: {_labBitmap.Width}×{_labBitmap.Height}";
        }

        private async void BtnStudioRunTests_Click(object sender, RoutedEventArgs e)
        {
            BtnStudioRunCode_Click(sender, e);
        }

        private void BtnStudioGabarito_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is not InteractiveLesson lesson) return;
            string sol = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
            _isHighlighting = true;
            CSharpSyntaxHighlighter.SetCode(RtbStudioEditableCode, sol);
            _isHighlighting = false;
        }

        private void BtnStudioStarter_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is not InteractiveLesson lesson) return;
            _isHighlighting = true;
            CSharpSyntaxHighlighter.SetCode(RtbStudioEditableCode, lesson.StarterTemplate);
            _isHighlighting = false;
        }

        private void BtnStudioRenderXaml_Click(object sender, RoutedEventArgs e)
        {
            string xaml = XamlSyntaxHighlighter.GetPlainText(RtbStudioXamlCode);
            var result = LiveCodeCompiler.EvaluateXaml(xaml);

            if (result.Success && result.Element != null)
            {
                PnlStudioLiveXamlContainer.Child = result.Element;
                TabStudioVisualizer.SelectedItem = TabItemStudioLiveXaml;
                TxtStudioStatus.Text = $"XAML renderizado com sucesso ({result.ExecutionTimeMs:F1} ms).";
            }
            else
            {
                TxtStudioStatus.Text = "Erro ao analisar o código XAML.";
                MessageBox.Show(result.ErrorMessage, "Erro de Análise XAML", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnStudioCopyXaml_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(XamlSyntaxHighlighter.GetPlainText(RtbStudioXamlCode));
            TxtStudioStatus.Text = "Código XAML copiado para a área de transferência.";
        }

        private void BtnStudioCopyOfficial_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CSharpSyntaxHighlighter.GetPlainText(RtbStudioCode));
            TxtStudioStatus.Text = "Código oficial copiado para a área de transferência.";
        }

        private void BtnStudioCopyConsole_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TxtStudioConsole.Text);
            TxtStudioStatus.Text = "Logs copiados para a área de transferência.";
        }

        private void LoadDefaultXamlSnippet()
        {
            string xaml = @"<Viewbox Margin=""10"">
    <Canvas Width=""400"" Height=""300"">
        <Rectangle Canvas.Left=""20"" Canvas.Top=""20"" Width=""360"" Height=""260"" RadiusX=""8"" RadiusY=""8"" Fill=""#1E293B"" Stroke=""#3B82F6"" StrokeThickness=""1.5""/>
        <Ellipse Canvas.Left=""150"" Canvas.Top=""100"" Width=""100"" Height=""100"" Fill=""#38BDF8""/>
        <TextBlock Canvas.Left=""80"" Canvas.Top=""220"" Text=""Laboratório C# &amp; WPF"" Foreground=""#FFFFFF"" FontSize=""16"" FontWeight=""Bold""/>
    </Canvas>
</Viewbox>";
            _isHighlighting = true;
            XamlSyntaxHighlighter.SetCode(RtbStudioXamlCode, xaml);
            _isHighlighting = false;
        }

        private void BtnStudioPrevLesson_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedIndex > 0)
            {
                LstStudioLessons.SelectedIndex--;
            }
        }

        private void BtnStudioNextLesson_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedIndex < _interactiveLessons.Count - 1)
            {
                LstStudioLessons.SelectedIndex++;
            }
        }

        private void BtnStudioToggleTrack_Click(object sender, RoutedEventArgs e)
        {
            if (ColStudioTrack.Width.Value > 0)
            {
                ColStudioTrack.Width = new GridLength(0);
                ColStudioSplitter1.Width = new GridLength(0);
                BtnStudioToggleTrack.Content = "Exibir Trilha";
            }
            else
            {
                ColStudioTrack.Width = new GridLength(380);
                ColStudioSplitter1.Width = new GridLength(5);
                BtnStudioToggleTrack.Content = "Ocultar Trilha";
            }
        }

        private void BtnStudioToggleCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (ColStudioCanvas.Width.Value > 0)
            {
                ColStudioCanvas.Width = new GridLength(0);
                ColStudioSplitter2.Width = new GridLength(0);
                BtnStudioToggleCanvas.Content = "Exibir Visualizador";
            }
            else
            {
                ColStudioCanvas.Width = new GridLength(420);
                ColStudioSplitter2.Width = new GridLength(5);
                BtnStudioToggleCanvas.Content = "Ocultar Visualizador";
            }
        }

        private void BtnStudioFocusCode_Click(object sender, RoutedEventArgs e)
        {
            ColStudioTrack.Width = new GridLength(0);
            ColStudioSplitter1.Width = new GridLength(0);
            ColStudioCanvas.Width = new GridLength(0);
            ColStudioSplitter2.Width = new GridLength(0);
            BtnStudioToggleTrack.Content = "Exibir Trilha";
            BtnStudioToggleCanvas.Content = "Exibir Visualizador";
        }

        private void BtnStudioResetPanels_Click(object sender, RoutedEventArgs e)
        {
            ColStudioTrack.Width = new GridLength(380);
            ColStudioSplitter1.Width = new GridLength(5);
            ColStudioCanvas.Width = new GridLength(420);
            ColStudioSplitter2.Width = new GridLength(5);
            BtnStudioToggleTrack.Content = "Ocultar Trilha";
            BtnStudioToggleCanvas.Content = "Ocultar Visualizador";
        }

        private void BtnOpenMsRef_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url && !string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                }
                catch { }
            }
        }
    }
}
