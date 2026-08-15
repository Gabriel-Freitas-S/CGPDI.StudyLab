using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Views
{
    /// <summary>
    /// Janela dedicada em tela cheia (Modo Estúdio) para programação, testes automatizados e laboratório guiado.
    /// </summary>
    public partial class CodeStudioWindow : Window
    {
        private List<InteractiveLesson> _lessons = new List<InteractiveLesson>();
        private DirectBitmap? _studioBitmap;
        private int _simulationStepIndex = 0;

        public CodeStudioWindow(int initialLessonNumber = 1)
        {
            InitializeComponent();
            Icon = AppIconHelper.GetAppIcon();
            InitializeStudio(initialLessonNumber);
        }

        private void InitializeStudio(int initialLessonNumber)
        {
            _studioBitmap = new DirectBitmap(512, 512);
            ImgStudioSimulation.Source = _studioBitmap.Bitmap;

            _lessons = InteractiveLabManager.GetLessons();
            LstStudioLessons.ItemsSource = _lessons;

            int targetIndex = Math.Clamp(initialLessonNumber - 1, 0, _lessons.Count - 1);
            LstStudioLessons.SelectedIndex = targetIndex;
        }

        private void LoadLesson(InteractiveLesson lesson)
        {
            _simulationStepIndex = 0;

            int currentStep = lesson.Number;
            int totalSteps = _lessons.Count;
            TxtStudioProgressHeader.Text = $"Trilha de Estudos (Passo {currentStep} de {totalSteps})";
            TxtStudioPercent.Text = $"{(currentStep * 100 / totalSteps)}%";
            PbStudioProgress.Maximum = totalSteps;
            PbStudioProgress.Value = currentStep;

            TxtTopLessonTitle.Text = $"[Passo {currentStep}/12] {lesson.Title}";
            TxtStudioSummary.Text = lesson.Summary;
            TxtStudioTheory.Text = lesson.Theory;
            TxtStudioOfficialCode.Text = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
            TxtStudioExplanation.Text = lesson.CodeExplanation;

            // Missão e Editor
            TxtStudioGoal.Text = lesson.ChallengeGoal;
            TxtStudioEditableCode.Text = lesson.StarterTemplate;
            TxtStudioTestReport.Text = "Pronto para testar. Escreva seu código e clique em '🚀 Compilar & Executar' ou '🧪 Rodar Testes'.";
            TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));

            ConfigureStudioSliders(lesson);
            LoadStudioQuiz(lesson);

            IcStudioMsRefs.ItemsSource = lesson.MicrosoftReferences;
            UpdateStudioSimulation();
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
                    TxtStudioParam1.Text = "Valor da Propriedade (0-255):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 140;
                    TxtStudioParam2.Text = "(Data Binding Ativo):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 1;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.PointerStrideOffset:
                    TxtStudioParam1.Text = "Coluna X (0 a 7):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 7; SliderStudio1.Value = 3;
                    TxtStudioParam2.Text = "Linha Y (0 a 7):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 7; SliderStudio2.Value = 2;
                    TxtStudioParam3.Text = "(Não utilizado nesta lição):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.WpfXamlAndDependencyProps:
                    TxtStudioParam1.Text = "Escala de Layout Desejada (%):";
                    SliderStudio1.Minimum = 20; SliderStudio1.Maximum = 200; SliderStudio1.Value = 100;
                    TxtStudioParam2.Text = "Espaço Disponível Pai:";
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
                    TxtStudioParam1.Text = "Posição X da Máscara 3x3:";
                    SliderStudio1.Minimum = 1; SliderStudio1.Maximum = 6; SliderStudio1.Value = 2;
                    TxtStudioParam2.Text = "Posição Y da Máscara 3x3:";
                    SliderStudio2.Minimum = 1; SliderStudio2.Maximum = 6; SliderStudio2.Value = 2;
                    TxtStudioParam3.Text = "Divisor de Normalização:";
                    SliderStudio3.Minimum = 1; SliderStudio3.Maximum = 16; SliderStudio3.Value = 9;
                    break;

                case LessonType.OtsuThresholdSearch:
                    TxtStudioParam1.Text = "Limiar de Teste T (0-255):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 255; SliderStudio1.Value = 128;
                    TxtStudioParam2.Text = "(Não utilizado):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 1; SliderStudio2.Value = 0;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.BresenhamStepByStep:
                    TxtStudioParam1.Text = "Coordenada Destino X1 (0-15):";
                    SliderStudio1.Minimum = 0; SliderStudio1.Maximum = 15; SliderStudio1.Value = 14;
                    TxtStudioParam2.Text = "Coordenada Destino Y1 (0-15):";
                    SliderStudio2.Minimum = 0; SliderStudio2.Maximum = 15; SliderStudio2.Value = 6;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;

                case LessonType.MatrixTransform2D:
                    TxtStudioParam1.Text = "Translação X (Tx: -50 a +50):";
                    SliderStudio1.Minimum = -50; SliderStudio1.Maximum = 50; SliderStudio1.Value = 20;
                    TxtStudioParam2.Text = "Translação Y (Ty: -50 a +50):";
                    SliderStudio2.Minimum = -50; SliderStudio2.Maximum = 50; SliderStudio2.Value = -10;
                    TxtStudioParam3.Text = "Ângulo de Rotação (Graus):";
                    SliderStudio3.Minimum = -180; SliderStudio3.Maximum = 180; SliderStudio3.Value = 35;
                    break;

                case LessonType.PipelineMVP3D:
                    TxtStudioParam1.Text = "Distância Z da Câmera (2 a 12):";
                    SliderStudio1.Minimum = 2; SliderStudio1.Maximum = 12; SliderStudio1.Value = 5;
                    TxtStudioParam2.Text = "Campo de Visão (FOV: 30-110°):";
                    SliderStudio2.Minimum = 30; SliderStudio2.Maximum = 110; SliderStudio2.Value = 60;
                    TxtStudioParam3.Text = "Rotação Y do Cubo (Graus):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 360; SliderStudio3.Value = 45;
                    break;

                case LessonType.HierarchicalSceneGraph:
                    TxtStudioParam1.Text = "Rotação da Base (Graus):";
                    SliderStudio1.Minimum = -90; SliderStudio1.Maximum = 90; SliderStudio1.Value = 25;
                    TxtStudioParam2.Text = "Ângulo do Ombro (Graus):";
                    SliderStudio2.Minimum = -60; SliderStudio2.Maximum = 60; SliderStudio2.Value = 40;
                    TxtStudioParam3.Text = "Ângulo do Cotovelo (Graus):";
                    SliderStudio3.Minimum = -90; SliderStudio3.Maximum = 90; SliderStudio3.Value = -30;
                    break;

                case LessonType.RayTracingIntersection:
                    TxtStudioParam1.Text = "Posição Y do Raio (-2 a +2):";
                    SliderStudio1.Minimum = -2.0; SliderStudio1.Maximum = 2.0; SliderStudio1.Value = 0.2;
                    TxtStudioParam2.Text = "Raio da Esfera (0.5 a 1.8):";
                    SliderStudio2.Minimum = 0.5; SliderStudio2.Maximum = 1.8; SliderStudio2.Value = 1.0;
                    TxtStudioParam3.Text = "(Não utilizado):";
                    SliderStudio3.Minimum = 0; SliderStudio3.Maximum = 1; SliderStudio3.Value = 0;
                    break;
            }

            SliderStudio1.ValueChanged += SliderStudio_ValueChanged;
            SliderStudio2.ValueChanged += SliderStudio_ValueChanged;
            SliderStudio3.ValueChanged += SliderStudio_ValueChanged;
        }

        private void LoadStudioQuiz(InteractiveLesson lesson)
        {
            TxtStudioQuizQuestion.Text = lesson.QuizQuestion;
            BrdStudioQuizFeedback.Visibility = Visibility.Collapsed;

            var buttons = new[] { BtnStudioQuiz0, BtnStudioQuiz1, BtnStudioQuiz2 };
            for (int i = 0; i < 3; i++)
            {
                if (i < lesson.QuizOptions.Count)
                {
                    buttons[i].Visibility = Visibility.Visible;
                    buttons[i].Content = $"{(char)('A' + i)}) {lesson.QuizOptions[i].Text}";
                    buttons[i].Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E2C"));
                    buttons[i].IsEnabled = true;
                }
                else
                {
                    buttons[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UpdateStudioSimulation()
        {
            if (_studioBitmap == null || LstStudioLessons?.SelectedItem is not InteractiveLesson lesson) return;

            TxtStudioVal1.Text = $"[ {SliderStudio1.Value:F0} ]";
            TxtStudioVal2.Text = $"[ {SliderStudio2.Value:F0} ]";
            TxtStudioVal3.Text = $"[ {SliderStudio3.Value:F0} ]";

            var log = new StringBuilder();
            InteractiveLabManager.RenderSimulation(
                _studioBitmap,
                lesson,
                SliderStudio1.Value,
                SliderStudio2.Value,
                SliderStudio3.Value,
                0,
                _simulationStepIndex,
                log);

            TxtStudioRamConsole.Text = log.ToString();
        }

        private void SliderStudio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateStudioSimulation();
        }

        private void LstStudioLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstStudioLessons.SelectedItem is InteractiveLesson lesson)
            {
                LoadLesson(lesson);
            }
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedIndex > 0)
                LstStudioLessons.SelectedIndex--;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons.SelectedIndex < _lessons.Count - 1)
                LstStudioLessons.SelectedIndex++;
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                BtnToggleMaximize_Click(sender, e);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_StateChanged(object? sender, EventArgs e)
        {
            if (BtnStudioMaximize != null)
            {
                BtnStudioMaximize.Content = WindowState == WindowState.Maximized ? "🗗 Restaurar" : "🗖 Maximizar";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.F11)
            {
                BtnToggleMaximize_Click(sender, e);
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnToggleMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnStudioToggleTrack_Click(object sender, RoutedEventArgs e)
        {
            if (ColStudioTrack.Width.Value > 0)
            {
                ColStudioTrack.Width = new GridLength(0);
                ColStudioSplitter1.Width = new GridLength(0);
                BtnStudioToggleTrack.Content = "▶ Trilha";
            }
            else
            {
                ColStudioTrack.Width = new GridLength(410);
                ColStudioSplitter1.Width = new GridLength(5);
                BtnStudioToggleTrack.Content = "◀ Trilha";
            }
        }

        private void BtnStudioToggleCanvas_Click(object sender, RoutedEventArgs e)
        {
            if (ColStudioCanvas.Width.Value > 0)
            {
                ColStudioCanvas.Width = new GridLength(0);
                ColStudioSplitter2.Width = new GridLength(0);
                BtnStudioToggleCanvas.Content = "▶ Canvas";
            }
            else
            {
                ColStudioCanvas.Width = new GridLength(430);
                ColStudioSplitter2.Width = new GridLength(5);
                BtnStudioToggleCanvas.Content = "◀ Canvas";
            }
        }

        private void BtnStudioFocusCode_Click(object sender, RoutedEventArgs e)
        {
            ColStudioTrack.Width = new GridLength(0);
            ColStudioSplitter1.Width = new GridLength(0);
            ColStudioCanvas.Width = new GridLength(0);
            ColStudioSplitter2.Width = new GridLength(0);
            BtnStudioToggleTrack.Content = "▶ Trilha";
            BtnStudioToggleCanvas.Content = "▶ Canvas";
            TxtStudioStatus.Text = "Modo Foco ativado: 100% da tela dedicada ao editor de código C# e testes unitários.";
        }

        private void BtnStudioResetPanels_Click(object sender, RoutedEventArgs e)
        {
            ColStudioTrack.Width = new GridLength(410);
            ColStudioSplitter1.Width = new GridLength(5);
            ColStudioCanvas.Width = new GridLength(430);
            ColStudioSplitter2.Width = new GridLength(5);
            BtnStudioToggleTrack.Content = "◀ Trilha";
            BtnStudioToggleCanvas.Content = "◀ Canvas";
            TxtStudioStatus.Text = "Disposição padrão dos 3 painéis restaurada.";
        }

        private async void BtnStudioCompileRun_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons?.SelectedItem is not InteractiveLesson lesson) return;

            BtnStudioCompileRun.IsEnabled = false;
            BtnStudioRunTests.IsEnabled = false;
            TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
            TxtStudioTestReport.Text = "⏳ Compilando com Roslyn e executando testes em tempo real...";

            var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(
                lesson,
                TxtStudioEditableCode.Text,
                _studioBitmap,
                SliderStudio1.Value,
                SliderStudio2.Value,
                SliderStudio3.Value);

            BtnStudioCompileRun.IsEnabled = true;
            BtnStudioRunTests.IsEnabled = true;

            DisplayCompilerReport(report);
        }

        private async void BtnStudioRunTests_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons?.SelectedItem is not InteractiveLesson lesson) return;

            BtnStudioCompileRun.IsEnabled = false;
            BtnStudioRunTests.IsEnabled = false;
            TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
            TxtStudioTestReport.Text = "⏳ Executando bateria de testes unitários automatizados...";

            var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(
                lesson,
                TxtStudioEditableCode.Text,
                null,
                SliderStudio1.Value,
                SliderStudio2.Value,
                SliderStudio3.Value);

            BtnStudioCompileRun.IsEnabled = true;
            BtnStudioRunTests.IsEnabled = true;

            DisplayCompilerReport(report);
        }

        private void DisplayCompilerReport(EvaluationReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"⏱️ Tempo de Execução: {report.ExecutionTimeMs:F1} ms");

            bool isCustomSuccess = !report.Success && string.IsNullOrEmpty(report.CompilerError) && report.RenderApplied;
            if (report.Success)
            {
                sb.AppendLine("📊 Status: ✅ APROVADO EM TODOS OS TESTES (100% Compatível)");
            }
            else if (isCustomSuccess)
            {
                sb.AppendLine("📊 Status: 🧪 CÓDIGO PERSONALIZADO EXECUTADO COM SUCESSO NO CANVAS");
            }
            else
            {
                sb.AppendLine("📊 Status: ❌ ERRO DE COMPILAÇÃO OU ASSERÇÃO");
            }

            sb.AppendLine(new string('-', 60));

            if (report.Tests.Count > 0)
            {
                sb.AppendLine("🧪 RESULTADOS DOS TESTES UNITÁRIOS:");
                foreach (var t in report.Tests)
                {
                    sb.AppendLine($" {(t.Passed ? "✅" : "⚠️")} {t.Name}");
                    sb.AppendLine($"    • Esperado: {t.Expected}");
                    sb.AppendLine($"    • Obtido:   {t.Actual}");
                    sb.AppendLine($"    • Detalhe:  {t.Details}");
                }
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(report.CompilerError))
            {
                sb.AppendLine("❌ DIAGNÓSTICO DO COMPILADOR ROSLYN:");
                sb.AppendLine(report.CompilerError);
            }

            if (report.RenderApplied)
            {
                sb.AppendLine("\n🎨 Resultado visual renderizado diretamente no Canvas pelo seu código!");
            }

            TxtStudioTestReport.Text = sb.ToString();
            TxtStudioTestReport.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(report.Success ? "#86EFAC" : (isCustomSuccess ? "#38BDF8" : "#FCA5A5")));

            TxtStudioStatus.Text = report.Success ? "Código compilado e testado com 100% de sucesso!" : (isCustomSuccess ? "Código personalizado renderizado no Canvas." : "Erros encontrados no código do aluno.");
        }

        private void BtnStudioBlankCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons?.SelectedItem is InteractiveLesson lesson)
            {
                TxtStudioEditableCode.Text = lesson.BlankTemplate;
                TxtStudioTestReport.Text = "Modo 'Em Branco' ativado. Escreva o algoritmo do zero e clique em '🚀 Compilar & Executar'.";
                TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
            }
        }

        private void BtnStudioStarterCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons?.SelectedItem is InteractiveLesson lesson)
            {
                TxtStudioEditableCode.Text = lesson.StarterTemplate;
                TxtStudioTestReport.Text = "Template inicial carregado com comentários orientadores (TODOs).";
                TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
            }
        }

        private void BtnStudioRestoreSolution_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudioLessons?.SelectedItem is InteractiveLesson lesson)
            {
                TxtStudioEditableCode.Text = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
                TxtStudioTestReport.Text = "Gabarito oficial de referência carregado no editor. Clique em '🚀 Compilar & Executar' ou '🧪 Rodar Testes'.";
                TxtStudioTestReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
            }
        }

        private void BtnStudioZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (TxtStudioEditableCode.FontSize < 34) TxtStudioEditableCode.FontSize += 1.5;
        }

        private void BtnStudioZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (TxtStudioEditableCode.FontSize > 10) TxtStudioEditableCode.FontSize -= 1.5;
        }

        private void BtnStudioCopyCode_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtStudioEditableCode.Text))
            {
                Clipboard.SetText(TxtStudioEditableCode.Text);
                TxtStudioStatus.Text = "Código copiado para a área de transferência!";
            }
        }

        private void BtnStudioStep_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex++;
            UpdateStudioSimulation();
        }

        private void BtnStudioRunAll_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 999;
            UpdateStudioSimulation();
        }

        private void BtnStudioReset_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 0;
            UpdateStudioSimulation();
        }

        private void BtnStudioQuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tagStr && int.TryParse(tagStr, out int optIdx) &&
                LstStudioLessons?.SelectedItem is InteractiveLesson lesson &&
                optIdx >= 0 && optIdx < lesson.QuizOptions.Count)
            {
                var opt = lesson.QuizOptions[optIdx];
                BrdStudioQuizFeedback.Visibility = Visibility.Visible;
                TxtStudioQuizFeedback.Text = $"{(opt.IsCorrect ? "✅ Correto!" : "❌ Incorreto.")} {opt.Explanation}";
                BrdStudioQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(opt.IsCorrect ? "#1E2A20" : "#2A1E20"));
                BrdStudioQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(opt.IsCorrect ? "#2E7D32" : "#C62828"));
            }
        }

        private void BtnOpenMsRef_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url && !string.IsNullOrEmpty(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Não foi possível abrir o link:\n{ex.Message}", "Erro ao abrir URL", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
