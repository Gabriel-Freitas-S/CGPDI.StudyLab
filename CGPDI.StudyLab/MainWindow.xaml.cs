using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Graphics2D;
using CGPDI.StudyLab.Graphics3D;
using CGPDI.StudyLab.ImageProcessing;
using CGPDI.StudyLab.Views;

namespace CGPDI.StudyLab
{
    /// <summary>
    /// Lógica de interação para MainWindow.xaml
    /// Estúdio Integrado de Processamento Digital de Imagens e Computação Gráfica 2D/3D.
    /// </summary>
    public partial class MainWindow : BorderlessWindow
    {
        private DirectBitmap _originalImage = null!;
        private DirectBitmap _currentImage = null!;
        private DirectBitmap _canvas2D = null!;
        private WpfViewport3DManager _viewport3D = null!;
        private DispatcherTimer _timer3D = null!;

        // Estado para transformações 2D homogêneas
        private System.Windows.Point[] _poly2DVertices = Array.Empty<System.Windows.Point>();
        private Matrix3x3 _current2DMatrix = Matrix3x3.Identity;

        // Base de dados pedagógica de estudos
        private List<StudyTopic> _allStudyTopics = new List<StudyTopic>();

        // Estado do Laboratório Interativo (C# e WPF Guiado)
        private List<InteractiveLesson> _interactiveLessons = new List<InteractiveLesson>();
        private int _currentLessonIndex = 0;
        private int _simulationStepIndex = 0;
        private DirectBitmap _labBitmap = null!;

        private readonly DispatcherTimer _highlightDebounceTimerLab;
        private bool _isLabHighlighting = false;

        public MainWindow()
        {
            InitializeComponent();
            Icon = AppIconHelper.GetAppIcon();

            _highlightDebounceTimerLab = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _highlightDebounceTimerLab.Tick += HighlightDebounceTimerLab_Tick;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MaximizeOnOpen();

            // 1. Inicializa Imagem de Calibração PDI Padrão
            LoadSampleImage(ImageSampleGenerator.GenerateCalibrationScene(512, 512));

            // 2. Inicializa Canvas de Computação Gráfica 2D
            _canvas2D = new DirectBitmap(512, 512);
            _canvas2D.Lock();
            _canvas2D.Clear(Color.FromRgb(20, 20, 26));
            _canvas2D.Unlock(true);
            ImgDisplay2D.Source = _canvas2D.Bitmap;
            Reset2DPolygon();

            // 3. Inicializa Gerenciador 3D Hardware (WPF Viewport3D)
            _viewport3D = new WpfViewport3DManager(ViewportMain);

            // Timer para rotação automática suave do 3D
            _timer3D = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            _timer3D.Tick += (s, ev) =>
            {
                _viewport3D?.RotateCamera(1.0);
            };

            // 4. Renderiza cena inicial 3D em Software
            RenderSoftware3D();

            // 5. Inicializa Central de Estudos e Documentação Passo a Passo
            _allStudyTopics = StudyGuideData.GetTopics();
            if (LstStudyTopics != null)
            {
                LstStudyTopics.ItemsSource = _allStudyTopics;
                if (_allStudyTopics.Count > 0)
                {
                    LstStudyTopics.SelectedIndex = 0;
                }
            }

            // 6. Inicializa Laboratório Interativo (Aprenda C# & WPF Passo a Passo)
            _labBitmap = new DirectBitmap(512, 512);
            ImgLabSimulation.Source = _labBitmap.Bitmap;
            _interactiveLessons = InteractiveLabManager.GetLessons();
            if (LstInteractiveLessons != null)
            {
                LstInteractiveLessons.ItemsSource = _interactiveLessons;
                if (_interactiveLessons.Count > 0)
                {
                    LstInteractiveLessons.SelectedIndex = 0;
                }
            }

            if (RtbLabXamlCode != null)
            {
                BtnLabXamlTemplateBasic_Click(this, new RoutedEventArgs());
            }

            if (RtbLabEditableCode != null)
            {
                RtbLabEditableCode.TextChanged += RtbLabEditableCode_TextChanged;
            }
            if (RtbLabXamlCode != null)
            {
                RtbLabXamlCode.TextChanged += RtbLabXamlCode_TextChanged;
            }

            // 7. Conecta Popout do Estúdio de Projetos (Aba 7)
            if (MainStudioControl != null)
            {
                MainStudioControl.PopoutRequested += (s, ev) => BtnOpenProjectStudio_Click(s ?? this, new RoutedEventArgs());
            }

            // Inicializa textos e códigos de teoria nas abas
            if (TxtTheoryTitle != null) TxtTheoryTitle.Text = "Cena de Calibração PDI Padrão";
            if (TxtTheoryMath != null) MathFormulaRenderer.RenderToTextBlock(TxtTheoryMath, "• Conversão perceptual BGRA32 e processamento direto via ponteiros unsafe na memória RAM.");
            if (RtbTheoryCodeSnippet != null) CSharpSyntaxHighlighter.SetCode(RtbTheoryCodeSnippet, AlgorithmCodeSnippets.GrayscaleCode);

            Update2DTheory("Rasterização 2D e Primitivas Gráficas", "• Reta de Bresenham: dx - dy com inteiros.\n• Círculo Ponto-Médio: 8 octantes simétricos com d = 1 - r.", AlgorithmCodeSnippets.BresenhamLineCode);

            Update3DTheory("Pipeline 3D e Matrizes de Transformação", "• Transformação MVP: v_{clip} = M_{proj} × M_{view} × M_{model} × v_{local}.\n• Iluminação de Blinn-Phong: I = I_a·k_a + I_d·k_d·(N·L) + I_s·k_s·(N·H)^α.", AlgorithmCodeSnippets.Pipeline3DMVPCode);

            UpdateRayTheory("Fundamentos de Ray Tracing Fotorrealista", "• Equação Paramétrica do Raio: r(t) = O + t·D, t > 0.\n• Interseção com Esfera: |O + t·D - C|^2 = R^2 ⟹ Bhaskara.\n• Reflexão Especular: R = D - 2(D·N)N.", AlgorithmCodeSnippets.RayTracingSphereCode);

            UpdateContextualTopBar();
            UpdateStatus("Ambiente carregado com sucesso. Selecione qualquer algoritmo para testar.", 0);

            // 8. Verificação não bloqueante de atualizações no GitHub Releases
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                var release = await UpdateManager.CheckForUpdatesAsync();
                if (release == null) return;

                // Respeita "lembrar mais tarde" (7 dias) e versões ignoradas pelo usuário
                var settings = UpdateSettingsStore.Load();
                if (!settings.ShouldNotifyFor(release.Version)) return;

                await Dispatcher.InvokeAsync(() =>
                {
                    var dlg = new UpdateDialogWindow(release, settings) { Owner = this };
                    dlg.ShowDialog();
                });
            });
        }

        private void RtbLabEditableCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLabHighlighting) return;
            _highlightDebounceTimerLab.Stop();
            _highlightDebounceTimerLab.Start();
        }

        private void RtbLabXamlCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isLabHighlighting) return;
            _highlightDebounceTimerLab.Stop();
            _highlightDebounceTimerLab.Start();
        }

        private void HighlightDebounceTimerLab_Tick(object? sender, EventArgs e)
        {
            _highlightDebounceTimerLab.Stop();
            if (_isLabHighlighting) return;
            _isLabHighlighting = true;

            try
            {
                if (TabLabEditor != null && TabLabEditor.SelectedIndex == 1 && RtbLabXamlCode != null)
                {
                    int offset = XamlSyntaxHighlighter.GetCaretCharIndex(RtbLabXamlCode);
                    string xaml = XamlSyntaxHighlighter.GetPlainText(RtbLabXamlCode);
                    XamlSyntaxHighlighter.Highlight(RtbLabXamlCode, xaml);
                    XamlSyntaxHighlighter.SetCaretCharIndex(RtbLabXamlCode, offset);
                }
                else if (RtbLabEditableCode != null)
                {
                    int offset = CSharpSyntaxHighlighter.GetCaretCharIndex(RtbLabEditableCode);
                    string code = CSharpSyntaxHighlighter.GetPlainText(RtbLabEditableCode);
                    CSharpSyntaxHighlighter.Highlight(RtbLabEditableCode, code);
                    CSharpSyntaxHighlighter.SetCaretCharIndex(RtbLabEditableCode, offset);
                }
            }
            catch
            {
                // Fallback seguro
            }
            finally
            {
                _isLabHighlighting = false;
            }
        }

        #region Utilitários de Atualização e Medição de Desempenho

        private void ExecuteAlgorithm(string title, string mathDescription, Func<DirectBitmap> algorithmFunc, string? codeSnippet = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                DirectBitmap result = algorithmFunc();
                sw.Stop();

                _currentImage?.Dispose();
                _currentImage = result;
                ImgDisplay.Source = _currentImage.Bitmap;

                double ms = sw.Elapsed.TotalMilliseconds;
                UpdateStatus($"Algoritmo '{title}' executado com sucesso.", ms);

                if (TxtTheoryTitle != null) TxtTheoryTitle.Text = title;
                if (TxtTheoryMath != null) MathFormulaRenderer.RenderToTextBlock(TxtTheoryMath, mathDescription);

                if (RtbTheoryCodeSnippet != null)
                {
                    string code = !string.IsNullOrWhiteSpace(codeSnippet) ? codeSnippet : GetDefaultPdiSnippet(title);
                    CSharpSyntaxHighlighter.SetCode(RtbTheoryCodeSnippet, code);
                }

                UpdateHistogram();
            }
            catch (Exception ex)
            {
                sw.Stop();
                MessageBox.Show($"Erro ao executar algoritmo: {ex.Message}", "Erro de Processamento", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDefaultPdiSnippet(string title)
        {
            string lower = title.ToLowerInvariant();
            if (lower.Contains("cinza") || lower.Contains("grayscale")) return AlgorithmCodeSnippets.GrayscaleCode;
            if (lower.Contains("invert") || lower.Contains("negativo")) return AlgorithmCodeSnippets.InvertCode;
            if (lower.Contains("sobel") || lower.Contains("scharr") || lower.Contains("borda")) return AlgorithmCodeSnippets.SobelCode;
            if (lower.Contains("gauss")) return AlgorithmCodeSnippets.GaussianCode;
            if (lower.Contains("otsu")) return AlgorithmCodeSnippets.OtsuCode;
            if (lower.Contains("box blur") || lower.Contains("média")) return AlgorithmCodeSnippets.GaussianCode;
            if (lower.Contains("fourier") || lower.Contains("dft")) return AlgorithmCodeSnippets.GrayscaleCode;
            return $@"// Implementação C# do Algoritmo: {title}
public static DirectBitmap Process(DirectBitmap src)
{{
    DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
    src.Lock(); dst.Lock();
    // Execução otimizada em paralelo com acesso direto ao BackBuffer
    src.Unlock(false); dst.Unlock(true);
    return dst;
}}";
        }

        private void Update2DTheory(string title, string math, string code)
        {
            if (TxtTheory2DTitle != null) TxtTheory2DTitle.Text = title;
            if (TxtTheory2DMath != null) MathFormulaRenderer.RenderToTextBlock(TxtTheory2DMath, math);
            if (RtbTheory2DCodeSnippet != null) CSharpSyntaxHighlighter.SetCode(RtbTheory2DCodeSnippet, code);
        }

        private void Update3DTheory(string title, string math, string code)
        {
            if (TxtTheory3DTitle != null) TxtTheory3DTitle.Text = title;
            if (TxtTheory3DMath != null) MathFormulaRenderer.RenderToTextBlock(TxtTheory3DMath, math);
            if (RtbTheory3DCodeSnippet != null) CSharpSyntaxHighlighter.SetCode(RtbTheory3DCodeSnippet, code);
        }

        private void UpdateRayTheory(string title, string math, string code)
        {
            if (TxtTheoryRayTitle != null) TxtTheoryRayTitle.Text = title;
            if (TxtTheoryRayMath != null) MathFormulaRenderer.RenderToTextBlock(TxtTheoryRayMath, math);
            if (RtbTheoryRayCodeSnippet != null) CSharpSyntaxHighlighter.SetCode(RtbTheoryRayCodeSnippet, code);
        }

        private void UpdateStatus(string message, double elapsedMs)
        {
            TxtStatus.Text = message;
            TxtExecutionTime.Text = elapsedMs > 0 ? $"{elapsedMs:F2} ms ({elapsedMs * 1000:F0} µs)" : "0.0 ms";
            if (_currentImage != null)
            {
                TxtResolution.Text = $"{_currentImage.Width}x{_currentImage.Height} ({_currentImage.Width * _currentImage.Height:N0} px)";
            }
        }

        private void LoadSampleImage(DirectBitmap bmp)
        {
            _originalImage?.Dispose();
            _currentImage?.Dispose();

            _originalImage = bmp;
            _currentImage = _originalImage.Clone();
            ImgDisplay.Source = _currentImage.Bitmap;

            UpdateHistogram();
            UpdateStatus("Nova imagem carregada no estúdio de PDI.", 0);
        }

        private InterpolationMode GetSelectedInterpolation()
        {
            return CmbInterpolation.SelectedIndex switch
            {
                0 => InterpolationMode.NearestNeighbor,
                1 => InterpolationMode.Bilinear,
                2 => InterpolationMode.Bicubic,
                _ => InterpolationMode.Bilinear
            };
        }

        #endregion

        #region Renderização do Gráfico de Histograma

        private void UpdateHistogram()
        {
            if (_currentImage == null) return;

            PointAndHistograms.CalculateHistograms(_currentImage, out int[] hR, out int[] hG, out int[] hB, out int[] hLum);

            CanvasHistogram.Children.Clear();
            double width = CanvasHistogram.ActualWidth;
            double height = CanvasHistogram.ActualHeight;

            if (width <= 0) width = 340;
            if (height <= 0) height = 160;

            int maxCount = 1;
            for (int i = 0; i < 256; i++)
            {
                if (hR[i] > maxCount) maxCount = hR[i];
                if (hG[i] > maxCount) maxCount = hG[i];
                if (hB[i] > maxCount) maxCount = hB[i];
                if (hLum[i] > maxCount) maxCount = hLum[i];
            }

            DrawHistogramCurve(hR, Color.FromRgb(255, 80, 80), width, height, maxCount);
            DrawHistogramCurve(hG, Color.FromRgb(80, 255, 80), width, height, maxCount);
            DrawHistogramCurve(hB, Color.FromRgb(80, 140, 255), width, height, maxCount);
            DrawHistogramCurve(hLum, Color.FromArgb(180, 240, 240, 240), width, height, maxCount, true);
        }

        private void DrawHistogramCurve(int[] hist, Color color, double canvasWidth, double canvasHeight, int maxVal, bool isDotted = false)
        {
            Polyline poly = new Polyline
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = isDotted ? 1.5 : 1.0,
                Opacity = 0.85
            };

            for (int i = 0; i < 256; i++)
            {
                double x = (i / 255.0) * canvasWidth;
                double y = canvasHeight - ((double)hist[i] / maxVal * (canvasHeight - 10));
                poly.Points.Add(new System.Windows.Point(x, y));
            }

            CanvasHistogram.Children.Add(poly);
        }

        private void CanvasHistogram_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHistogram();
        }

        #endregion

        #region Eventos da Barra Superior (Presets & Arquivos)

        private void BtnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Imagens Suportadas (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Todos os Arquivos (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    DirectBitmap bmp = DirectBitmap.FromFile(dlg.FileName);
                    LoadSampleImage(bmp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Falha ao abrir imagem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImage == null) return;

            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Arquivo PNG (*.png)|*.png|Arquivo JPEG (*.jpg)|*.jpg|Arquivo BMP (*.bmp)|*.bmp",
                FileName = "PDI_Resultado_Exportado.png"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using (FileStream stream = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        BitmapEncoder encoder = dlg.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            ? new JpegBitmapEncoder()
                            : new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(_currentImage.Bitmap));
                        encoder.Save(stream);
                    }
                    MessageBox.Show("Imagem exportada com sucesso!", "Salvo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar imagem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnResetOriginal_Click(object sender, RoutedEventArgs e)
        {
            if (_originalImage == null) return;
            _currentImage?.Dispose();
            _currentImage = _originalImage.Clone();
            ImgDisplay.Source = _currentImage.Bitmap;
            UpdateHistogram();
            UpdateStatus("Imagem restaurada ao estado original.", 0);
        }

        private void BtnPresetCalibration_Click(object sender, RoutedEventArgs e) => LoadSampleImage(ImageSampleGenerator.GenerateCalibrationScene(512, 512));
        private void BtnPresetColorWheel_Click(object sender, RoutedEventArgs e) => LoadSampleImage(ImageSampleGenerator.GenerateColorWheel(512, 512));
        private void BtnPresetFrequency_Click(object sender, RoutedEventArgs e) => LoadSampleImage(ImageSampleGenerator.GenerateFrequencyPattern(512, 512));
        private void BtnPresetNoise_Click(object sender, RoutedEventArgs e) => LoadSampleImage(ImageSampleGenerator.GenerateNoisyImage(512, 512, 0.15));

        #endregion

        #region Eventos PDI: 1. Espaços de Cores & Canais

        private void BtnGrayscaleBt709_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Escala de Cinza ITU-R BT.709 (Luminância sRGB)",
                "• Fórmula: Y = 0.2126·R + 0.7152·G + 0.0722·B\n" +
                "• Teoria: O olho humano possui densidade muito maior de cones M (sensíveis ao verde, ~535nm). " +
                "Por isso o canal Verde recebe mais de 71% do peso na percepção de luminância moderna.",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                byte lum = ColorSpaces.RgbToGrayscale(sRow[x * 4 + 2], sRow[x * 4 + 1], sRow[x * 4 + 0], ColorSpaces.GrayscaleMethod.LuminanceBt709);
                                dRow[x * 4 + 0] = lum; dRow[x * 4 + 1] = lum; dRow[x * 4 + 2] = lum; dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnGrayscaleBt601_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Escala de Cinza ITU-R BT.601 (NTSC/PAL Clássico)",
                "• Fórmula: Y = 0.299·R + 0.587·G + 0.114·B\n" +
                "• Teoria: Padrão histórico da TV em cores analógica (fósforos CRT originais).",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                byte lum = ColorSpaces.RgbToGrayscale(sRow[x * 4 + 2], sRow[x * 4 + 1], sRow[x * 4 + 0], ColorSpaces.GrayscaleMethod.LuminanceBt601);
                                dRow[x * 4 + 0] = lum; dRow[x * 4 + 1] = lum; dRow[x * 4 + 2] = lum; dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnGrayscaleAverage_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Escala de Cinza por Média Aritmética",
                "• Fórmula: Y = (R + G + B) / 3\n" +
                "• Teoria: Média simples não-ponderada. Ignora as respostas espectrais assimétricas da visão humana.",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                byte lum = ColorSpaces.RgbToGrayscale(sRow[x * 4 + 2], sRow[x * 4 + 1], sRow[x * 4 + 0], ColorSpaces.GrayscaleMethod.Average);
                                dRow[x * 4 + 0] = lum; dRow[x * 4 + 1] = lum; dRow[x * 4 + 2] = lum; dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnGrayscaleLightness_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Escala de Cinza por Claridade (Lightness HSL)",
                "• Fórmula: Y = [max(R,G,B) + min(R,G,B)] / 2\n" +
                "• Teoria: Representa a média dos extremos cromáticos, baseada no canal L do modelo HSL.",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                byte lum = ColorSpaces.RgbToGrayscale(sRow[x * 4 + 2], sRow[x * 4 + 1], sRow[x * 4 + 0], ColorSpaces.GrayscaleMethod.Lightness);
                                dRow[x * 4 + 0] = lum; dRow[x * 4 + 1] = lum; dRow[x * 4 + 2] = lum; dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnChannelRed_Click(object sender, RoutedEventArgs e) => IsolateChannel(2, "Isolamento do Canal Vermelho (R)");
        private void BtnChannelGreen_Click(object sender, RoutedEventArgs e) => IsolateChannel(1, "Isolamento do Canal Verde (G)");
        private void BtnChannelBlue_Click(object sender, RoutedEventArgs e) => IsolateChannel(0, "Isolamento do Canal Azul (B)");

        private void IsolateChannel(int channelIdx, string name)
        {
            ExecuteAlgorithm(
                name,
                $"• Teoria: Zera todos os canais de cor exceto o canal {(channelIdx == 2 ? "R" : channelIdx == 1 ? "G" : "B")}, " +
                "permitindo analisar a decomposição no cubo de cores aditivo RGB.",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                dRow[x * 4 + 0] = channelIdx == 0 ? sRow[x * 4 + 0] : (byte)0;
                                dRow[x * 4 + 1] = channelIdx == 1 ? sRow[x * 4 + 1] : (byte)0;
                                dRow[x * 4 + 2] = channelIdx == 2 ? sRow[x * 4 + 2] : (byte)0;
                                dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnSepia_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Transformação Matricial de Sépia Fotográfica",
                "• Transformação linear via Matriz 3x3:\n" +
                "  R' = 0.393·R + 0.769·G + 0.189·B\n" +
                "  G' = 0.349·R + 0.686·G + 0.168·B\n" +
                "  B' = 0.272·R + 0.534·G + 0.131·B\n" +
                "• Teoria: Simula a química de viragem com sulfeto de prata usada na fotografia histórica do século XIX.",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                Color c = ColorSpaces.ApplySepia(sRow[x * 4 + 2], sRow[x * 4 + 1], sRow[x * 4 + 0]);
                                dRow[x * 4 + 0] = c.B; dRow[x * 4 + 1] = c.G; dRow[x * 4 + 2] = c.R; dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        private void BtnInvert_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Inversão Negativa de Cores",
                "• Fórmula: g(x, y) = 255 - f(x, y)\n" +
                "• Teoria: Complemento aritmético no espaço de cores RGB (Branco vira Preto, Vermelho vira Ciano, etc.).",
                () =>
                {
                    DirectBitmap dst = new DirectBitmap(_originalImage.Width, _originalImage.Height);
                    _originalImage.Lock(); dst.Lock();
                    unsafe
                    {
                        System.Threading.Tasks.Parallel.For(0, _originalImage.Height, y =>
                        {
                            byte* sRow = _originalImage.BackBuffer + (y * _originalImage.Stride);
                            byte* dRow = dst.BackBuffer + (y * dst.Stride);
                            for (int x = 0; x < _originalImage.Width; x++)
                            {
                                dRow[x * 4 + 0] = (byte)(255 - sRow[x * 4 + 0]);
                                dRow[x * 4 + 1] = (byte)(255 - sRow[x * 4 + 1]);
                                dRow[x * 4 + 2] = (byte)(255 - sRow[x * 4 + 2]);
                                dRow[x * 4 + 3] = 255;
                            }
                        });
                    }
                    _originalImage.Unlock(false); dst.Unlock(true);
                    return dst;
                });
        }

        #endregion

        #region Eventos PDI: 2. Operações Pontuais

        private void SliderPointOp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_originalImage == null) return;

            int brightness = (int)SliderBrightness.Value;
            double contrast = SliderContrast.Value;
            double gamma = SliderGamma.Value;

            ExecuteAlgorithm(
                $"Ajustes Pontuais (Brilho: {brightness}, Contraste: {contrast:F1}x, Gamma: {gamma:F2})",
                "• Pipeline de Transformações Pontuais:\n" +
                "  1. Brilho: g1 = f + delta\n" +
                "  2. Contraste: g2 = alpha · (g1 - 128) + 128\n" +
                "  3. Correção Gamma: g3 = 255 · (g2 / 255)^(1/gamma)\n" +
                "• Complexidade: O(256) com Look-Up Table (LUT) pré-computada em vez de O(W·H).",
                () =>
                {
                    using (DirectBitmap b = PointAndHistograms.AdjustBrightness(_originalImage, brightness))
                    using (DirectBitmap c = PointAndHistograms.AdjustContrast(b, contrast))
                    {
                        return PointAndHistograms.AdjustGamma(c, gamma);
                    }
                });
        }

        private void BtnPosterize_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Posterização (Quantização de Níveis Tonais)",
                "• Teoria: Reduz o número de tons contínuos por canal para N níveis discretos.\n" +
                "• Aplicações: Compressão com perda de dados, estilização gráfica de arte pop.",
                () => PointAndHistograms.Posterize(_originalImage, 4));
        }

        private void BtnSolarize_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Solarização (Efeito Sabattier)",
                "• Fórmula: g(v) = (v > T) ? (255 - v) : v\n" +
                "• Teoria: Cria inversão parcial de tons onde áreas muito iluminadas sofrem reversão de contraste.",
                () => PointAndHistograms.Solarize(_originalImage, 128));
        }

        #endregion

        #region Eventos PDI: 3. Processamento de Histograma

        private void BtnEqualizeHsv_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Equalização de Histograma (Luminância)",
                "• Função de Distribuição Acumulada (CDF):\n" +
                "  CDF(i) = \\sum_{j=0}^{i} p(j)\n" +
                "  h_eq(v) = round( (CDF(v) - CDF_min) / (N - CDF_min) · 255 )\n" +
                "• Teoria: Achata o histograma e maximiza o contraste global preservando as proporções cromáticas originais.",
                () => PointAndHistograms.EqualizeHistogram(_originalImage, true));
        }

        private void BtnEqualizeRgb_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Equalização de Histograma RGB Independente",
                "• Teoria: Equaliza os canais R, G e B separadamente. Pode causar desvios de matiz (color cast) em regiões de alto contraste.",
                () => PointAndHistograms.EqualizeHistogram(_originalImage, false));
        }

        private void BtnContrastStretch_Click(object sender, RoutedEventArgs e)
        {
            ExecuteAlgorithm(
                "Expansão de Contraste (Normalização Min-Max)",
                "• Fórmula: g(x, y) = [ (f(x, y) - f_min) / (f_max - f_min) ] · 255\n" +
                "• Teoria: Mapeia linearmente o menor valor existente para 0 e o maior para 255.",
                () => PointAndHistograms.ContrastStretching(_originalImage));
        }

        #endregion

        #region Eventos PDI: 4. Filtros Espaciais (Convolução)

        private void BtnBoxBlur3_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro da Média 3x3 (Box Blur)", "• Kernel 3x3 com pesos uniformes 1/9.", () => SpatialFilters.BoxBlur(_originalImage, 3));

        private void BtnBoxBlur5_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro da Média 5x5 (Box Blur)", "• Kernel 5x5 com pesos uniformes 1/25.", () => SpatialFilters.BoxBlur(_originalImage, 5));

        private void BtnGaussian3_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro Gaussiano 3x3", "• G(x,y) = e^(-(x^2+y^2)/(2·σ^2)) / (2πσ^2) com σ=1.0.", () => SpatialFilters.GaussianBlur(_originalImage, 1.0, 3));

        private void BtnGaussian5_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro Gaussiano 5x5", "• Distribuição normal bidimensional suave com σ=1.4.", () => SpatialFilters.GaussianBlur(_originalImage, 1.4, 5));

        private void BtnSharpen_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro de Nitidez (Sharpen Laplaciano)", "• Kernel Laplaciano com ganho central positivo.", () => SpatialFilters.Sharpen(_originalImage, 1.0));

        private void BtnUnsharpMask_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Máscara de Desfoque (Unsharp Masking)", "• Result = Original + 1.5·(Original - Gaussiano).", () => SpatialFilters.UnsharpMask(_originalImage, 1.5, 1.5));

        private void BtnMedian_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Filtro da Mediana 3x3 (Anti-Ruído)", "• Filtro não-linear que calcula o elemento mediano da janela 3x3. Elimina ruído impulsivo.", () => SpatialFilters.MedianFilter(_originalImage, 1));

        private void BtnEmboss_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Efeito Relevo 3D (Emboss 45°)", "• Simula sombreamento direcionado com bias 128.", () => SpatialFilters.Emboss(_originalImage, 45.0));

        #endregion

        #region Eventos PDI: 5. Detecção de Bordas

        private void BtnSobelMagnitude_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Operador Sobel (Magnitude Total)", "• G = √(Gx^2 + Gy^2) com convoluções ortogonais 3x3.", () => SpatialFilters.Sobel(_originalImage, true, false, false));

        private void BtnSobelGx_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Sobel Gx (Gradiente Horizontal)", "• Destaca bordas verticais com transições horizontais.", () => SpatialFilters.Sobel(_originalImage, false, true, false));

        private void BtnSobelGy_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Sobel Gy (Gradiente Vertical)", "• Destaca bordas horizontais com transições verticais.", () => SpatialFilters.Sobel(_originalImage, false, false, true));

        private void BtnScharr_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Operador Scharr", "• Otimizado numericamente para simetria rotacional perfeita (coeficientes 3, 10, 3).", () => SpatialFilters.Scharr(_originalImage));

        private void BtnLaplacian_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Operador Laplaciano", "• Segunda derivada espacial ∇^2 f = d^2f/dx^2 + d^2f/dy^2.", () => SpatialFilters.Laplacian(_originalImage, true));

        private void BtnLoG_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Laplaciano do Gaussiano (LoG / Mexican Hat)", "• Combina suavização Gaussiana e operador Laplaciano.", () => SpatialFilters.LaplacianOfGaussian(_originalImage));

        private void BtnCanny_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Algoritmo Canny Edge Detector Completo",
                "• Pipeline em 5 Etapas:\n" +
                "  1. Suavização Gaussiana (σ=1.4)\n" +
                "  2. Cálculo do Gradiente e Ângulo (Sobel)\n" +
                "  3. Supressão de Não-Máximos (NMS) nos 4 setores (0°, 45°, 90°, 135°)\n" +
                "  4. Limiarização Dupla (Double Threshold: T_high=65, T_low=25)\n" +
                "  5. Rastreamento de Bordas por Histerese (BFS / Flood nas bordas conexas).",
                () => SpatialFilters.CannyEdgeDetector(_originalImage, 25.0, 65.0));

        #endregion

        #region Eventos PDI: 6. Morfologia & Segmentação

        private void BtnOtsu_Click(object sender, RoutedEventArgs e)
        {
            byte thresh;
            DirectBitmap res = Morphology.OtsuThreshold(_originalImage, out thresh);
            ExecuteAlgorithm(
                $"Limiarização Automática de Otsu (Limiar T* = {thresh})",
                "• Maximiza a variância inter-classes:\n" +
                "  σ_B^2(t) = ω_0(t)·ω_1(t)·[μ_0(t) - μ_1(t)]^2\n" +
                "• Teoria: Separação ótima do fundo e primeiro plano sem parâmetros manuais.",
                () => res);
        }

        private void BtnAdaptiveThreshold_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Limiarização Adaptativa Local", "• Limiar por pixel = Média da vizinhança 15x15 - C.", () => Morphology.AdaptiveThreshold(_originalImage, 15, 5));

        private void BtnErosion_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Erosão Morfológica (A ⊖ B)", "• Mínimo na vizinhança do elemento estruturante. Encolhe objetos claros.", () => Morphology.Erosion(_originalImage, StructuringElementType.Square3x3));

        private void BtnDilation_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Dilatação Morfológica (A ⊕ B)", "• Máximo na vizinhança do elemento estruturante. Expande objetos claros.", () => Morphology.Dilation(_originalImage, StructuringElementType.Square3x3));

        private void BtnOpening_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Abertura Morfológica ((A ⊖ B) ⊕ B)", "• Erosão seguida de Dilatação. Remove ruídos pontuais claros sem alterar tamanho global.", () => Morphology.Opening(_originalImage, StructuringElementType.Square3x3));

        private void BtnClosing_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Fechamento Morfológico ((A ⊕ B) ⊖ B)", "• Dilatação seguida de Erosão. Preenche furos e fendas escuras.", () => Morphology.Closing(_originalImage, StructuringElementType.Square3x3));

        private void BtnMorphGradient_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Gradiente Morfológico (Dilatação - Erosão)", "• Isola os contornos e silhuetas exatas dos objetos.", () => Morphology.MorphologicalGradient(_originalImage, StructuringElementType.Square3x3));

        private void BtnTopHat_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Transformação Top-Hat (Cartola Branca)", "• Original - Abertura. Isola picos de brilho menores que o elemento estruturante.", () => Morphology.TopHat(_originalImage, StructuringElementType.Square3x3));

        #endregion

        #region Eventos PDI: 7. Transformações Geométricas 2D

        private void BtnRotate45_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Rotação 2D +45° (Mapeamento Inverso)", "• (x_src, y_src) = R(-45°) · (x_dst, y_dst) com interpolação.", () => GeometricTransforms.Rotate(_originalImage, 45.0, GetSelectedInterpolation()));

        private void BtnRotate90_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Rotação 2D +90°", "• Rotação ortogonal exata.", () => GeometricTransforms.Rotate(_originalImage, 90.0, GetSelectedInterpolation()));

        private void BtnScaleUp_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Redimensionamento 2D (Zoom In 1.5x)", "• Reamostragem contínua com interpolação selecionada.", () => GeometricTransforms.Scale(_originalImage, 1.5, 1.5, GetSelectedInterpolation()));

        private void BtnScaleDown_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Redimensionamento 2D (Zoom Out 0.7x)", "• Redução de escala.", () => GeometricTransforms.Scale(_originalImage, 0.7, 0.7, GetSelectedInterpolation()));

        private void BtnShear_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Cisalhamento Afim (Shear)", "• x_src = x_dst - 0.2·y_dst.", () => GeometricTransforms.Shear(_originalImage, 0.2, 0.0, GetSelectedInterpolation()));

        private void BtnFlipH_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Espelhamento Horizontal (Flip H)", "• x_src = Width - 1 - x_dst.", () => GeometricTransforms.Flip(_originalImage, true, false));

        private void BtnSwirl_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Distorção Redemoinho (Swirl / Vortex)", "• Ângulo de rotação decresce quadraticamente com a distância ao raio central.", () => GeometricTransforms.Swirl(_originalImage, 220.0, 3.5, GetSelectedInterpolation()));

        private void BtnWave_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Distorção de Ondas (Ripple)", "• x_src = x + A·sin(2π y / λ).", () => GeometricTransforms.Wave(_originalImage, 12.0, 0.05, GetSelectedInterpolation()));

        private void BtnFisheye_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm("Distorção Olho de Peixe (Fisheye / Barrel)", "• Modela a curvatura de lentes fotográficas grande-angulares: r' = r(1 + kr^2).", () => GeometricTransforms.Fisheye(_originalImage, 0.000008, GetSelectedInterpolation()));

        #endregion

        #region Eventos PDI: 8. Frequência, Ruído & Fractais

        private void BtnFourierSpectrum_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Espectro de Magnitude da Transformada de Fourier 2D (2D DFT)",
                "• F(u,v) = \\sum_{x} \\sum_{y} f(x,y) e^{-j 2π (ux/M + vy/N)}\n" +
                "• Magnitude Logarítmica: S(u, v) = c·log(1 + |F(u, v)|)\n" +
                "• Com FFTShift: Frequências baixas (DC) centralizadas no meio; frequências altas nas bordas.",
                () => FrequencyAndProcedural.ComputeFourierMagnitudeSpectrum(_originalImage, 128));

        private void BtnPerlinFbm_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Síntese Procedural de Terreno fBm (Perlin Noise)",
                "• Fractal Brownian Motion combinando 6 oitavas com persistência 0.5 e lacunaridade 2.0.",
                () => FrequencyAndProcedural.GenerateProceduralNoiseImage(512, 512, 6.0, 6));

        private void BtnVoronoi_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Ruído Celular de Voronoi (Worley Noise)",
                "• Diagrama de Voronoi com campos de distância euclidiana mínima.",
                () => FrequencyAndProcedural.GenerateVoronoiNoiseImage(512, 512, 36));

        private void BtnMandelbrot_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Fractal Conjunto de Mandelbrot",
                "• Z_{n+1} = Z_n^2 + C com coloração contínua suave ν = i + 1 - ln(ln|Z|)/ln(2).",
                () => FrequencyAndProcedural.GenerateMandelbrot(512, 512, -0.75, 0.0, 1.0, 100));

        private void BtnJulia_Click(object sender, RoutedEventArgs e) =>
            ExecuteAlgorithm(
                "Fractal Conjunto de Julia (C = -0.7 + 0.27015i)",
                "• Z_{n+1} = Z_n^2 + C_0 no plano complexo.",
                () => FrequencyAndProcedural.GenerateJulia(512, 512, -0.7, 0.27015, 100));

        #endregion

        #region ABA 2: COMPUTAÇÃO GRÁFICA 2D (Rasterização & Matrizes 3x3)

        private void Reset2DPolygon()
        {
            _poly2DVertices = new System.Windows.Point[]
            {
                new System.Windows.Point(200, 150),
                new System.Windows.Point(320, 150),
                new System.Windows.Point(360, 260),
                new System.Windows.Point(260, 340),
                new System.Windows.Point(160, 260)
            };
            _current2DMatrix = Matrix3x3.Identity;
        }

        private void Clear2DCanvas()
        {
            _canvas2D.Lock();
            _canvas2D.Clear(Color.FromRgb(18, 18, 24));
            _canvas2D.Unlock(true);
        }

        private void Btn2D_Clear_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            Reset2DPolygon();
            Update2DTheory("Quadro 2D Limpo", "Selecione uma primitiva ou transformação para desenhar.", "// Quadro 2D pronto para renderização.");
        }

        private void Btn2D_DrawBresenham_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            // Desenha um leque de retas com o algoritmo de Bresenham em todos os octantes
            int cx = 256, cy = 256;
            for (int angle = 0; angle < 360; angle += 15)
            {
                double rad = angle * Math.PI / 180.0;
                int x = (int)(cx + Math.Cos(rad) * 200);
                int y = (int)(cy + Math.Sin(rad) * 200);
                Color col = ColorSpaces.HsvToRgb(angle, 0.85, 0.95);
                Rasterizer2D.DrawLineBresenham(_canvas2D, cx, cy, x, y, col);
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Algoritmo de Linha de Bresenham (1965)",
                "• Aritmética 100% Inteira (sem ponto flutuante ou divisões).\n" +
                "• Variável de Decisão: e = 2·Δy - Δx\n" +
                "• Incrementa x e acumula o erro. Quando e >= 0, incrementa y e subtrai 2·Δx.\n" +
                "• Suporta todos os 8 octantes do plano cartesiano.",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void Btn2D_DrawDDA_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            int cx = 256, cy = 256;
            for (int angle = 0; angle < 360; angle += 20)
            {
                double rad = angle * Math.PI / 180.0;
                int x = (int)(cx + Math.Cos(rad) * 190);
                int y = (int)(cy + Math.Sin(rad) * 190);
                Color col = ColorSpaces.HsvToRgb(angle, 0.9, 1.0);
                Rasterizer2D.DrawLineDDA(_canvas2D, cx, cy, x, y, col);
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Algoritmo DDA (Digital Differential Analyzer)",
                "• Fórmula Incremental:\n" +
                "  dx = (x1 - x0) / steps, dy = (y1 - y0) / steps\n" +
                "• Utiliza ponto flutuante com arredondamento round(x) a cada passo.",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void Btn2D_DrawWu_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            for (int i = 0; i < 12; i++)
            {
                double y0 = 60 + i * 35;
                Rasterizer2D.DrawLineWu(_canvas2D, 50, y0, 460, y0 + (i - 6) * 20, Color.FromRgb(100, 220, 255));
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Algoritmo de Linhas Suavizadas de Xiaolin Wu (Anti-Aliasing)",
                "• Suavização de Serrilhado em Tempo Real:\n" +
                "• Em cada coordenada x, plota os dois pixels adjacentes com transparência alpha proporcional à distância da linha real.",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void Btn2D_DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            int cx = 256, cy = 256;
            for (int r = 30; r <= 220; r += 30)
            {
                Color col = ColorSpaces.HsvToRgb((r * 2.0) % 360, 0.8, 0.95);
                Rasterizer2D.DrawCircleMidpoint(_canvas2D, cx, cy, r, col, false);
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Círculo pelo Algoritmo do Ponto Médio (Bresenham Circle)",
                "• Simetria em 8 Octantes: Calcula 1/8 do perímetro (45°) e espelha em (±x, ±y) e (±y, ±x).\n" +
                "• Variável de decisão: d = 1 - r.",
                AlgorithmCodeSnippets.MidpointCircleCode);
        }

        private void Btn2D_DrawEllipse_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            int cx = 256, cy = 256;
            for (int i = 1; i <= 5; i++)
            {
                int rx = i * 40;
                int ry = i * 20;
                Color col = ColorSpaces.HsvToRgb(i * 50, 0.85, 0.95);
                Rasterizer2D.DrawEllipseMidpoint(_canvas2D, cx, cy, rx, ry, col, false);
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Elipse pelo Algoritmo do Ponto Médio",
                "• Duas Regiões de inclinação (|dy/dx| < 1 e |dy/dx| > 1).\n" +
                "• Simetria em 4 Quadrantes (±x, ±y).",
                AlgorithmCodeSnippets.MidpointCircleCode);
        }

        private void Btn2D_DrawBezierQuad_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            System.Windows.Point p0 = new System.Windows.Point(60, 420);
            System.Windows.Point p1 = new System.Windows.Point(256, 40);
            System.Windows.Point p2 = new System.Windows.Point(450, 420);

            // Desenha polígono de controle
            Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, Color.FromArgb(120, 255, 255, 255));
            Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, Color.FromArgb(120, 255, 255, 255));

            // Curva de Bézier Quadrática
            Rasterizer2D.DrawBezierQuadratic(_canvas2D, p0, p1, p2, Color.FromRgb(255, 100, 180), 80);

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Curva de Bézier Quadrática (3 Pontos de Controle)",
                "• Fórmula Paramétrica: B(t) = (1-t)^2·P0 + 2(1-t)t·P1 + t^2·P2 para t ∈ [0, 1].",
                AlgorithmCodeSnippets.MidpointCircleCode);
        }

        private void Btn2D_DrawBezierCubic_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            System.Windows.Point p0 = new System.Windows.Point(60, 380);
            System.Windows.Point p1 = new System.Windows.Point(120, 60);
            System.Windows.Point p2 = new System.Windows.Point(380, 440);
            System.Windows.Point p3 = new System.Windows.Point(450, 100);

            // Polígono de controle
            Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, Color.FromArgb(100, 200, 200, 200));
            Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, Color.FromArgb(100, 200, 200, 200));
            Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p2.X, (int)p2.Y, (int)p3.X, (int)p3.Y, Color.FromArgb(100, 200, 200, 200));

            // Curva de Bézier Cúbica
            Rasterizer2D.DrawBezierCubic(_canvas2D, p0, p1, p2, p3, Color.FromRgb(80, 220, 255), 100);

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Curva de Bézier Cúbica (4 Pontos de Controle)",
                "• Fórmula Paramétrica: B(t) = (1-t)^3·P0 + 3(1-t)^2 t·P1 + 3(1-t)t^2·P2 + t^3·P3\n" +
                "• Padrão industrial em fontes vetoriais (TrueType / PostScript / SVG).",
                AlgorithmCodeSnippets.MidpointCircleCode);
        }

        private void Btn2D_DrawScanlinePoly_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            // Estrela côncava de 5 pontas
            System.Windows.Point[] star = new System.Windows.Point[10];
            double cx = 256, cy = 256;
            for (int i = 0; i < 10; i++)
            {
                double r = (i % 2 == 0) ? 180 : 80;
                double angle = (i * 36 - 90) * Math.PI / 180.0;
                star[i] = new System.Windows.Point(cx + r * Math.Cos(angle), cy + r * Math.Sin(angle));
            }

            Rasterizer2D.DrawPolygonScanline(_canvas2D, star, Color.FromRgb(255, 180, 50));

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Preenchimento de Polígonos por Varredura (Scanline Fill)",
                "• Constrói a Edge Table (ET) e Active Edge Table (AET).\n" +
                "• Preenche os spans de pixels entre pares ordenados de interseções (Regra Par-Ímpar / Paridade).",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void Btn2D_DrawClipping_Click(object sender, RoutedEventArgs e)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            Rect clipWindow = new Rect(100, 100, 312, 312);

            // Desenha a janela de recorte (Caixa)
            Color boxCol = Color.FromRgb(100, 100, 130);
            Rasterizer2D.DrawLineBresenham(_canvas2D, 100, 100, 412, 100, boxCol);
            Rasterizer2D.DrawLineBresenham(_canvas2D, 412, 100, 412, 412, boxCol);
            Rasterizer2D.DrawLineBresenham(_canvas2D, 412, 412, 100, 412, boxCol);
            Rasterizer2D.DrawLineBresenham(_canvas2D, 100, 412, 100, 100, boxCol);

            // Traça linhas de teste
            Random rnd = new Random(42);
            for (int i = 0; i < 20; i++)
            {
                System.Windows.Point p0 = new System.Windows.Point(rnd.Next(20, 490), rnd.Next(20, 490));
                System.Windows.Point p1 = new System.Windows.Point(rnd.Next(20, 490), rnd.Next(20, 490));

                // Desenha a linha original tênue em vermelho
                Rasterizer2D.DrawLineBresenham(_canvas2D, (int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, Color.FromArgb(70, 255, 80, 80));

                // Recorta com Cohen-Sutherland
                System.Windows.Point cp0 = p0;
                System.Windows.Point cp1 = p1;
                if (Rasterizer2D.ClipLineCohenSutherland(clipWindow, ref cp0, ref cp1))
                {
                    // Desenha o segmento visível em verde neon
                    Rasterizer2D.DrawLineBresenham(_canvas2D, (int)cp0.X, (int)cp0.Y, (int)cp1.X, (int)cp1.Y, Color.FromRgb(80, 255, 120));
                }
            }

            _canvas2D.Unlock(true);
            Update2DTheory(
                "Recorte de Linhas de Cohen-Sutherland",
                "• Outcodes de 4 bits: Top (1000), Bottom (0100), Right (0010), Left (0001).\n" +
                "• Trivial Accept: code0 | code1 == 0\n" +
                "• Trivial Reject: code0 & code1 != 0",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void Btn2D_FloodFill_Click(object sender, RoutedEventArgs e)
        {
            // Preenche o centro da tela
            _canvas2D.Lock();
            Rasterizer2D.FloodFill(_canvas2D, 256, 256, Color.FromRgb(40, 100, 220));
            _canvas2D.Unlock(true);

            Update2DTheory(
                "Preenchimento por Inundação (Flood Fill)",
                "• Algoritmo iterativo baseado em fila (Queue-based 4-way) para evitar Stack Overflow.",
                AlgorithmCodeSnippets.BresenhamLineCode);
        }

        private void ImgDisplay2D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.GetPosition(ImgDisplay2D);
            int x = (int)(pt.X * _canvas2D.Width / ImgDisplay2D.ActualWidth);
            int y = (int)(pt.Y * _canvas2D.Height / ImgDisplay2D.ActualHeight);

            _canvas2D.Lock();
            Rasterizer2D.FloodFill(_canvas2D, x, y, Color.FromRgb(220, 60, 120));
            _canvas2D.Unlock(true);
        }

        private void Btn2D_TransformTranslate_Click(object sender, RoutedEventArgs e) => Apply2DTransform(Matrix3x3.CreateTranslation(30, 20), "Translação 2D (+30px, +20px)");
        private void Btn2D_TransformRotate_Click(object sender, RoutedEventArgs e) => Apply2DTransform(Matrix3x3.CreateRotation(30.0 * Math.PI / 180.0), "Rotação 2D (+30°)");
        private void Btn2D_TransformScale_Click(object sender, RoutedEventArgs e) => Apply2DTransform(Matrix3x3.CreateScale(1.2, 1.2), "Escala 2D (1.2x)");
        private void Btn2D_TransformShear_Click(object sender, RoutedEventArgs e) => Apply2DTransform(Matrix3x3.CreateShear(0.2, 0.0), "Cisalhamento 2D (Shear X)");

        private void Apply2DTransform(Matrix3x3 mat, string name)
        {
            Clear2DCanvas();
            _canvas2D.Lock();

            // Centro do polígono como pivô
            double cx = 0, cy = 0;
            foreach (var v in _poly2DVertices) { cx += v.X; cy += v.Y; }
            cx /= _poly2DVertices.Length;
            cy /= _poly2DVertices.Length;

            Matrix3x3 toOrigin = Matrix3x3.CreateTranslation(-cx, -cy);
            Matrix3x3 fromOrigin = Matrix3x3.CreateTranslation(cx, cy);
            Matrix3x3 composite = fromOrigin * mat * toOrigin;

            for (int i = 0; i < _poly2DVertices.Length; i++)
            {
                _poly2DVertices[i] = composite.TransformPoint(_poly2DVertices[i]);
            }

            // Desenha polígono transformado
            Rasterizer2D.DrawPolygonScanline(_canvas2D, _poly2DVertices, Color.FromRgb(70, 160, 255));

            _canvas2D.Unlock(true);
            Update2DTheory(
                $"Transformação Afim 2D: {name}",
                "• Composição Matricial Homogênea 3x3:\n" +
                "  M = T(pivô) · Transformação · T(-pivô)\n" +
                "• Unifica translação, rotação, escala e cisalhamento na mesma multiplicação matricial.",
                AlgorithmCodeSnippets.Matrix3x3Code);
        }

        #endregion

        #region ABA 3: COMPUTAÇÃO GRÁFICA 3D (WPF VIEWPORT 3D)

        private HierarchicalRobotArm? _robotArm;
        private DispatcherTimer? _timerRobot;
        private double _robotAnimTime = 0;

        private void RbCameraProj_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewport3D == null) return;
            bool isPerspective = RbCameraPerspective?.IsChecked == true;
            _viewport3D.SetCameraProjection(isPerspective);
            UpdateStatus($"Câmera 3D alterada para: {(isPerspective ? "Projeção Perspectiva (com ponto de fuga)" : "Projeção Ortográfica (paralela)")}", 0);
            Update3DTheory(
                isPerspective ? "Projeção Perspectiva 3D" : "Projeção Ortográfica 3D",
                isPerspective 
                    ? "• Projeção Perspectiva: v_{clip} = M_{proj} × M_{view} × M_{model} × v_{local} com divisão perspectiva por W."
                    : "• Projeção Ortográfica: Mapeamento linear direto sem ponto de fuga angular.",
                AlgorithmCodeSnippets.Pipeline3DMVPCode);
        }

        private void Btn3D_LoadRobot_Click(object sender, RoutedEventArgs e)
        {
            if (_viewport3D == null) return;

            _robotArm = new HierarchicalRobotArm();
            _viewport3D.SetHierarchicalScene(_robotArm.RootNode.ModelGroup);
            _viewport3D.SetDistance(7.5); // Garante enquadramento panorâmico perfeito sem cortes

            // Reseta sliders
            if (SliderRobotBase != null) SliderRobotBase.Value = 0;
            if (SliderRobotShoulder != null) SliderRobotShoulder.Value = 25;
            if (SliderRobotElbow != null) SliderRobotElbow.Value = -45;
            if (SliderRobotWrist != null) SliderRobotWrist.Value = 30;

            UpdateRobotJoints();
            UpdateStatus("Modelagem Hierárquica carregada: Robô Articulado de 4 Níveis (Scene Graph / Cinemática Direta).", 0);
            Update3DTheory(
                "Modelagem Hierárquica e Grafo de Cena (Scene Graph)",
                "• Multiplicação cumulativa de transformações locais: M_{total} = M_{base} × M_{ombro} × M_{cotovelo} × M_{pulso}.",
                AlgorithmCodeSnippets.Pipeline3DMVPCode);
        }

        private void SliderRobot_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateRobotJoints();
        }

        private void UpdateRobotJoints()
        {
            if (_robotArm == null) return;

            double b = SliderRobotBase?.Value ?? 0;
            double s = SliderRobotShoulder?.Value ?? 25;
            double el = SliderRobotElbow?.Value ?? -45;
            double w = SliderRobotWrist?.Value ?? 30;

            _robotArm.SetJointAngles(b, s, el, w);
        }

        private void ChkRobotAnim_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_timerRobot == null)
            {
                _timerRobot = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25) };
                _timerRobot.Tick += (s, ev) =>
                {
                    _robotAnimTime += 0.04;
                    if (SliderRobotBase != null) SliderRobotBase.Value = Math.Sin(_robotAnimTime * 0.7) * 90;
                    if (SliderRobotShoulder != null) SliderRobotShoulder.Value = Math.Sin(_robotAnimTime * 1.1) * 40 + 15;
                    if (SliderRobotElbow != null) SliderRobotElbow.Value = Math.Cos(_robotAnimTime * 1.3) * 50 - 20;
                    if (SliderRobotWrist != null) SliderRobotWrist.Value = Math.Sin(_robotAnimTime * 2.0) * 60;
                };
            }

            if (ChkRobotAnim.IsChecked == true)
            {
                if (_robotArm == null) Btn3D_LoadRobot_Click(this, new RoutedEventArgs());
                _timerRobot.Start();
            }
            else
            {
                _timerRobot.Stop();
            }
        }

        private void Cmb3DShapes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewport3D == null) return;
            if (ChkRobotAnim != null) ChkRobotAnim.IsChecked = false;
            string shape = (Cmb3DShapes.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Torus";
            _viewport3D.SetShape(shape);
            Update3DTheory(
                $"Geometria Paramétrica 3D: {shape}",
                "• Transformação MVP: v_{clip} = M_{proj} × M_{view} × M_{model} × v_{local}.\n• Iluminação de Blinn-Phong: I = I_a·k_a + I_d·k_d·(N·L) + I_s·k_s·(N·H)^α.",
                AlgorithmCodeSnippets.Pipeline3DMVPCode);
        }

        private void Btn3D_ColorBlue_Click(object sender, RoutedEventArgs e) => _viewport3D?.UpdateMaterial(Color.FromRgb(60, 150, 250), Slider3DSpecular.Value);
        private void Btn3D_ColorGold_Click(object sender, RoutedEventArgs e) => _viewport3D?.UpdateMaterial(Color.FromRgb(240, 190, 40), Slider3DSpecular.Value);
        private void Btn3D_ColorGreen_Click(object sender, RoutedEventArgs e) => _viewport3D?.UpdateMaterial(Color.FromRgb(40, 210, 120), Slider3DSpecular.Value);
        private void Btn3D_ColorRed_Click(object sender, RoutedEventArgs e) => _viewport3D?.UpdateMaterial(Color.FromRgb(240, 60, 80), Slider3DSpecular.Value);

        private void Slider3DMaterial_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            _viewport3D?.UpdateMaterial(Color.FromRgb(60, 150, 250), Slider3DSpecular.Value);

        private void Slider3DLights_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            _viewport3D?.UpdateLights(Color.FromRgb(240, 240, 255), Color.FromRgb(255, 180, 100), Slider3DAmbient.Value);

        private void Chk3DAutoRotate_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (Chk3DAutoRotate.IsChecked == true) _timer3D.Start();
            else _timer3D.Stop();
        }

        private void Btn3D_ResetCamera_Click(object sender, RoutedEventArgs e) => _viewport3D?.ResetCamera();

        #endregion

        #region ABA 4: SOFTWARE 3D & RAY TRACING

        private Mesh3D _currentSoftMesh = Mesh3D.CreateCube(1.8);

        private void CmbSoftMesh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentSoftMesh = CmbSoftMesh.SelectedIndex switch
            {
                0 => Mesh3D.CreateCube(1.8, Color.FromRgb(80, 160, 240)),
                1 => Mesh3D.CreatePyramid(2.0),
                2 => Mesh3D.CreateTorus(1.2, 0.45, 24, 16),
                3 => Mesh3D.CreateUvSphere(1.2, 20, 20),
                _ => Mesh3D.CreateCube(1.8)
            };
            RenderSoftware3D();
        }

        private void SliderSoft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => RenderSoftware3D();
        private void ChkSoft_Changed(object sender, RoutedEventArgs e) => RenderSoftware3D();

        private void BtnSoftRender_Click(object sender, RoutedEventArgs e) => RenderSoftware3D();

        private void RenderSoftware3D()
        {
            if (ImgDisplay3DSoft == null || _currentSoftMesh == null) return;

            Stopwatch sw = Stopwatch.StartNew();
            DirectBitmap bmp = SoftwareRenderer3D.RenderScene(
                _currentSoftMesh,
                512, 512,
                SliderSoftRotX?.Value ?? 0.4,
                SliderSoftRotY?.Value ?? 0.6,
                0.0,
                SliderSoftCamDist?.Value ?? 3.5,
                ChkSoftWireframe?.IsChecked == true
            );
            sw.Stop();

            ImgDisplay3DSoft.Source = bmp.Bitmap;
            UpdateStatus("Pipeline 3D em Software renderizado.", sw.Elapsed.TotalMilliseconds);
            UpdateRayTheory(
                "Renderizador 3D em Software (100% CPU)",
                "• Pipeline de Rasterização em Software:\n  1. Transformação de Vértices MVP\n  2. Back-face Culling\n  3. Rasterização de Triângulos com Interpolação Baricêntrica e Z-Buffer.",
                AlgorithmCodeSnippets.Pipeline3DMVPCode);
        }

        private void SliderRay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }

        private void BtnRayTrace_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            double camAngle = SliderRayCam.Value;
            int bounces = (int)SliderRayBounces.Value;

            DirectBitmap bmp = Raytracer3D.Render(512, 512, camAngle, bounces);
            sw.Stop();

            ImgDisplay3DSoft.Source = bmp.Bitmap;
            UpdateStatus($"Ray Tracer renderizado com {bounces} reflexões recursivas.", sw.Elapsed.TotalMilliseconds);
            UpdateRayTheory(
                $"Ray Tracer Fotorrealista ({bounces} Bounces)",
                "• Equação Paramétrica do Raio: r(t) = O + t·D, t > 0.\n• Interseção Raio-Esfera: |O + t·D - C|^2 = R^2 ⟹ Equação Quadrática.\n• Reflexão Especular Recursiva: R = D - 2(D·N)N.\n• Modelo de Blinn-Phong com Iluminação Global Direta + Espelhamento.",
                AlgorithmCodeSnippets.RayTracingSphereCode);
        }

        #endregion

        #region Barra de Título Customizada & Menu Contextual

        protected override void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != MainTabControl) return;
            UpdateContextualTopBar();
        }

        private void UpdateContextualTopBar()
        {
            if (PnlContextualTopActions == null) return;
            PnlContextualTopActions.Children.Clear();

            int tabIdx = MainTabControl?.SelectedIndex ?? 0;
            switch (tabIdx)
            {
                case 0: // PDI
                    AddTopButton("Abrir", (s, e) => BtnOpenImage_Click(s, e));
                    AddTopButton("Salvar", (s, e) => BtnSaveImage_Click(s, e));
                    AddTopButton("Resetar", (s, e) => BtnResetOriginal_Click(s, e));
                    AddTopSeparator();
                    AddTopButton("Calibração", (s, e) => BtnPresetCalibration_Click(s, e));
                    AddTopButton("Roda HSV", (s, e) => BtnPresetColorWheel_Click(s, e));
                    AddTopButton("MTF", (s, e) => BtnPresetFrequency_Click(s, e));
                    AddTopButton("Ruído", (s, e) => BtnPresetNoise_Click(s, e));
                    break;

                case 1: // 2D
                    AddTopButton("Limpar Quadro", (s, e) => Btn2D_Clear_Click(s, e));
                    AddTopSeparator();
                    AddTopButton("Bresenham", (s, e) => Btn2D_DrawBresenham_Click(s, e));
                    AddTopButton("Linha Wu", (s, e) => Btn2D_DrawWu_Click(s, e));
                    AddTopButton("Bézier", (s, e) => Btn2D_DrawBezierCubic_Click(s, e));
                    AddTopButton("Scanline", (s, e) => Btn2D_DrawScanlinePoly_Click(s, e));
                    break;

                case 2: // 3D
                    AddTopButton("Resetar Câmera", (s, e) => Btn3D_ResetCamera_Click(s, e));
                    AddTopButton("Carregar Robô", (s, e) => Btn3D_LoadRobot_Click(s, e), true);
                    AddTopSeparator();
                    AddTopButton("Toro", (s, e) => { if (Cmb3DShapes != null) Cmb3DShapes.SelectedIndex = 0; });
                    AddTopButton("Esfera", (s, e) => { if (Cmb3DShapes != null) Cmb3DShapes.SelectedIndex = 1; });
                    AddTopButton("Cubo", (s, e) => { if (Cmb3DShapes != null) Cmb3DShapes.SelectedIndex = 2; });
                    break;

                case 3: // Ray Tracing
                    AddTopButton("Renderizar Software 3D", (s, e) => BtnSoftRender_Click(s, e), true);
                    AddTopButton("Renderizar Ray Tracer", (s, e) => BtnRayTrace_Click(s, e), true);
                    break;

                case 4: // Central de Estudos
                    AddTopButton("▶ Praticar no Laboratório", (s, e) => BtnGoToLaboratorioLesson_Click(s, e), true);
                    break;

                case 5: // Laboratório
                    AddTopButton("Modo Foco", (s, e) => BtnFocusCode_Click(s, e), true);
                    AddTopButton("Restaurar Painéis", (s, e) => BtnResetColumns_Click(s, e));
                    AddTopButton("Janela Separada", (s, e) => BtnOpenPopoutStudio_Click(s, e));
                    break;

                case 6: // Estúdio de Projetos
                    AddTopButton("▶ Executar Código", (s, e) => MainStudioControl?.ExecuteFreeScript(), true);
                    AddTopButton("Janela Cheia", (s, e) => BtnOpenProjectStudio_Click(s, e));
                    break;
            }
        }

        private void AddTopButton(string text, RoutedEventHandler onClick, bool isPrimary = false)
        {
            var btn = new Button
            {
                Content = text,
                Padding = new Thickness(8, 3, 8, 3),
                Margin = new Thickness(0, 0, 3, 0),
                FontSize = 11.5
            };
            if (isPrimary)
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2563EB"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                btn.FontWeight = FontWeights.Bold;
            }
            btn.Click += onClick;
            PnlContextualTopActions.Children.Add(btn);
        }

        private void AddTopSeparator()
        {
            var sep = new Rectangle
            {
                Width = 1,
                Height = 16,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E2E40")),
                Margin = new Thickness(4, 0, 6, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            PnlContextualTopActions.Children.Add(sep);
        }

        #endregion

        #region ABA 5: CENTRAL DE ESTUDOS & DOCUMENTAÇÃO PASSO A PASSO

        private void LstStudyTopics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstStudyTopics?.SelectedItem is StudyTopic topic)
            {
                TxtDocCategory.Text = topic.Category.ToUpper();
                TxtDocTitle.Text = topic.Title;
                TxtDocSummary.Text = topic.Summary;
                MathFormulaRenderer.RenderToTextBlock(TxtDocMath, topic.MathFormulas);
                TxtDocExplanation.Text = topic.CodeExplanation;
                CSharpSyntaxHighlighter.SetCode(RtbDocSnippet, topic.CodeSnippet);
                TxtDocComplexity.Text = topic.ComplexityAndTips;
                TxtDocWhereToTest.Text = topic.WhereToTest;
                if (IcMicrosoftRefs != null)
                {
                    IcMicrosoftRefs.ItemsSource = topic.MicrosoftReferences;
                }

                LoadStudyTopicQuiz(topic);
            }
        }

        private void LoadStudyTopicQuiz(StudyTopic topic)
        {
            if (topic.Quiz == null || string.IsNullOrEmpty(topic.Quiz.Question))
            {
                if (GrpStudyTopicQuiz != null) GrpStudyTopicQuiz.Visibility = Visibility.Collapsed;
                return;
            }

            if (GrpStudyTopicQuiz != null) GrpStudyTopicQuiz.Visibility = Visibility.Visible;
            if (TxtStudyTopicQuizQuestion != null) TxtStudyTopicQuizQuestion.Text = topic.Quiz.Question;
            if (BtnStudyTopicQuizOpt0 != null) BtnStudyTopicQuizOpt0.Content = (topic.Quiz.Options.Count > 0 ? "A) " + topic.Quiz.Options[0] : "");
            if (BtnStudyTopicQuizOpt1 != null) BtnStudyTopicQuizOpt1.Content = (topic.Quiz.Options.Count > 1 ? "B) " + topic.Quiz.Options[1] : "");
            if (BtnStudyTopicQuizOpt2 != null) BtnStudyTopicQuizOpt2.Content = (topic.Quiz.Options.Count > 2 ? "C) " + topic.Quiz.Options[2] : "");

            if (BrdStudyTopicQuizFeedback != null) BrdStudyTopicQuizFeedback.Visibility = Visibility.Collapsed;
        }

        private void BtnStudyTopicQuizOpt_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudyTopics?.SelectedItem is not StudyTopic topic || topic.Quiz == null) return;
            if (sender is not Button btn || !int.TryParse(btn.Tag?.ToString(), out int optIndex)) return;

            bool isCorrect = (optIndex == topic.Quiz.CorrectOptionIndex);
            if (BrdStudyTopicQuizFeedback != null) BrdStudyTopicQuizFeedback.Visibility = Visibility.Visible;

            if (isCorrect)
            {
                if (BrdStudyTopicQuizFeedback != null)
                {
                    BrdStudyTopicQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A24"));
                    BrdStudyTopicQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E"));
                }
                if (TxtStudyTopicQuizFeedback != null)
                {
                    TxtStudyTopicQuizFeedback.Text = $"✓ RESPOSTA CORRETA!\n{topic.Quiz.Explanation}";
                    TxtStudyTopicQuizFeedback.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
                }
            }
            else
            {
                if (BrdStudyTopicQuizFeedback != null)
                {
                    BrdStudyTopicQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A1E1E"));
                    BrdStudyTopicQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                }
                if (TxtStudyTopicQuizFeedback != null)
                {
                    TxtStudyTopicQuizFeedback.Text = $"✕ INCORRETO. Tente novamente!\n{topic.Quiz.Explanation}";
                    TxtStudyTopicQuizFeedback.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCA5A5"));
                }
            }
        }

        private void BtnGoToLaboratorioLesson_Click(object sender, RoutedEventArgs e)
        {
            if (LstStudyTopics?.SelectedItem is StudyTopic topic && topic.TargetLessonNumber > 0)
            {
                MainTabControl.SelectedIndex = 5; // Aba Laboratório
                int targetIdx = topic.TargetLessonNumber - 1;
                if (targetIdx >= 0 && targetIdx < _interactiveLessons.Count && LstInteractiveLessons != null)
                {
                    LstInteractiveLessons.SelectedIndex = targetIdx;
                }
            }
        }

        private void BtnGoToStudyTheory_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is InteractiveLesson lesson)
            {
                MainTabControl.SelectedIndex = 4; // Aba Central de Estudos
                int topicIdx = Math.Clamp(lesson.Number - 1, 0, _allStudyTopics.Count - 1);
                if (LstStudyTopics != null) LstStudyTopics.SelectedIndex = topicIdx;
            }
        }

        private void BtnCopyTheoryCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbTheoryCodeSnippet != null)
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbTheoryCodeSnippet);
                if (!string.IsNullOrEmpty(code))
                {
                    Clipboard.SetText(code);
                    UpdateStatus("Código C# de PDI copiado para a área de transferência.", 0);
                }
            }
        }

        private void BtnCopyTheory2DCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbTheory2DCodeSnippet != null)
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbTheory2DCodeSnippet);
                if (!string.IsNullOrEmpty(code))
                {
                    Clipboard.SetText(code);
                    UpdateStatus("Código C# 2D copiado para a área de transferência.", 0);
                }
            }
        }

        private void BtnCopyTheory3DCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbTheory3DCodeSnippet != null)
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbTheory3DCodeSnippet);
                if (!string.IsNullOrEmpty(code))
                {
                    Clipboard.SetText(code);
                    UpdateStatus("Código C# 3D copiado para a área de transferência.", 0);
                }
            }
        }

        private void BtnCopyTheoryRayCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbTheoryRayCodeSnippet != null)
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbTheoryRayCodeSnippet);
                if (!string.IsNullOrEmpty(code))
                {
                    Clipboard.SetText(code);
                    UpdateStatus("Código C# de Ray Tracing copiado para a área de transferência.", 0);
                }
            }
        }

        private void TxtSearchStudy_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allStudyTopics == null || LstStudyTopics == null) return;

            string query = (TxtSearchStudy?.Text ?? "").Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                LstStudyTopics.ItemsSource = _allStudyTopics;
            }
            else
            {
                List<StudyTopic> filtered = _allStudyTopics.FindAll(t =>
                    t.Title.ToLower().Contains(query) ||
                    t.Category.ToLower().Contains(query) ||
                    t.Summary.ToLower().Contains(query) ||
                    t.MathFormulas.ToLower().Contains(query));
                LstStudyTopics.ItemsSource = filtered;
            }

            if (LstStudyTopics.Items.Count > 0)
            {
                LstStudyTopics.SelectedIndex = 0;
            }
        }

        private void BtnOpenOnlineDocs_Click(object sender, RoutedEventArgs e)
        {
            OpenBrowserUrl("https://gabriel-freitas-s.github.io/CGPDI.StudyLab/");
        }

        private void BtnOpenMsRef_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url)
            {
                OpenBrowserUrl(url);
            }
        }

        private void OpenBrowserUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível abrir o navegador: {ex.Message}", "Erro ao Abrir Link", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region 🎓 ABA 6: LABORATÓRIO INTERATIVO & PLAYGROUND GUIADO

        private void LstInteractiveLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstInteractiveLessons.SelectedItem is InteractiveLesson lesson)
            {
                _currentLessonIndex = _interactiveLessons.IndexOf(lesson);
                LoadLesson(lesson);
            }
        }

        private void BtnPrevLesson_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLessonIndex > 0)
            {
                _currentLessonIndex--;
                LstInteractiveLessons.SelectedIndex = _currentLessonIndex;
            }
        }

        private void BtnNextLesson_Click(object sender, RoutedEventArgs e)
        {
            if (_currentLessonIndex < _interactiveLessons.Count - 1)
            {
                _currentLessonIndex++;
                LstInteractiveLessons.SelectedIndex = _currentLessonIndex;
            }
        }

        private void LoadLesson(InteractiveLesson lesson)
        {
            _simulationStepIndex = 0;

            int currentStep = lesson.Number;
            int totalSteps = _interactiveLessons.Count;
            TxtLessonProgressHeader.Text = $"Trilha de Estudos (Passo {currentStep} de {totalSteps})";
            TxtLessonPercent.Text = $"{(currentStep * 100 / totalSteps)}%";
            PbLessonProgress.Maximum = totalSteps;
            PbLessonProgress.Value = currentStep;

            TxtLessonModule.Text = lesson.Module.ToUpper();
            TxtLessonTitle.Text = lesson.Title;
            TxtLessonSummary.Text = lesson.Summary;

            string sol = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
            if (RtbLabCode != null) CSharpSyntaxHighlighter.SetCode(RtbLabCode, sol);
            TxtLabExplanation.Text = lesson.CodeExplanation;

            // Exibe a aba XAML apenas em lições onde há conteúdo de estudo XAML relevante
            if (lesson.HasXamlContent)
            {
                if (TabItemLabXaml != null)
                {
                    TabItemLabXaml.Visibility = Visibility.Visible;
                    if (RtbLabXamlCode != null && lesson.XamlSnippet != null) XamlSyntaxHighlighter.SetCode(RtbLabXamlCode, lesson.XamlSnippet);
                }
            }
            else
            {
                if (TabItemLabXaml != null)
                {
                    TabItemLabXaml.Visibility = Visibility.Collapsed;
                    if (TabLabEditor != null && TabLabEditor.SelectedItem == TabItemLabXaml)
                    {
                        TabLabEditor.SelectedIndex = 0;
                    }
                }
            }

            // Configuração do Desafio de Código do Aluno
            if (TxtChallengeGoal != null) TxtChallengeGoal.Text = lesson.ChallengeGoal;
            if (RtbLabEditableCode != null) CSharpSyntaxHighlighter.SetCode(RtbLabEditableCode, lesson.StarterTemplate);
            if (TxtCompilerReport != null)
            {
                TxtCompilerReport.Text = "Pronto para testar. Escreva seu código e clique em 'Compilar e Executar C#' ou 'Executar Testes Unitários'.";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
            }

            ConfigureLabSliders(lesson);
            LoadQuiz(lesson);

            UpdateLabSimulation();
        }

        private void ConfigureLabSliders(InteractiveLesson lesson)
        {
            SliderLab1.ValueChanged -= SliderLab_ValueChanged;
            SliderLab2.ValueChanged -= SliderLab_ValueChanged;
            SliderLab3.ValueChanged -= SliderLab_ValueChanged;

            switch (lesson.Type)
            {
                case LessonType.BgraMemoryLayout:
                    TxtLabParam1.Text = "Canal Azul (Blue: 0-255):";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 255; SliderLab1.Value = 255;
                    TxtLabParam2.Text = "Canal Verde (Green: 0-255):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 255; SliderLab2.Value = 120;
                    TxtLabParam3.Text = "Canal Vermelho (Red: 0-255):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 255; SliderLab3.Value = 30;
                    break;

                case LessonType.CSharpPropertiesAndNotify:
                    TxtLabParam1.Text = "Valor da Propriedade (0-255):";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 255; SliderLab1.Value = 140;
                    TxtLabParam2.Text = "(Data Binding Reativo):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 1; SliderLab2.Value = 1;
                    TxtLabParam3.Text = "(Não utilizado):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.PointerStrideOffset:
                    TxtLabParam1.Text = "Coluna X (0 a 7):";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 7; SliderLab1.Value = 3;
                    TxtLabParam2.Text = "Linha Y (0 a 7):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 7; SliderLab2.Value = 2;
                    TxtLabParam3.Text = "(Não utilizado nesta lição):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.WpfXamlAndDependencyProps:
                    TxtLabParam1.Text = "Escala Desejada (%):";
                    SliderLab1.Minimum = 20; SliderLab1.Maximum = 200; SliderLab1.Value = 100;
                    TxtLabParam2.Text = "Espaço Disponível Pai:";
                    SliderLab2.Minimum = 100; SliderLab2.Maximum = 450; SliderLab2.Value = 300;
                    TxtLabParam3.Text = "(Não utilizado):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.WriteableBitmapLifecycle:
                    TxtLabParam1.Text = "Etapa do Ciclo (1 a 4):";
                    SliderLab1.Minimum = 1; SliderLab1.Maximum = 4; SliderLab1.Value = 1;
                    TxtLabParam2.Text = "(Buffer Traseiro GPU):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 1; SliderLab2.Value = 1;
                    TxtLabParam3.Text = "(Não utilizado):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.ConvolutionStepByStep:
                    TxtLabParam1.Text = "Posição X da Máscara 3x3:";
                    SliderLab1.Minimum = 1; SliderLab1.Maximum = 6; SliderLab1.Value = 2;
                    TxtLabParam2.Text = "Posição Y da Máscara 3x3:";
                    SliderLab2.Minimum = 1; SliderLab2.Maximum = 6; SliderLab2.Value = 2;
                    TxtLabParam3.Text = "Divisor de Normalização:";
                    SliderLab3.Minimum = 1; SliderLab3.Maximum = 16; SliderLab3.Value = 9;
                    break;

                case LessonType.OtsuThresholdSearch:
                    TxtLabParam1.Text = "Limiar de Corte T (0-255):";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 255; SliderLab1.Value = 118;
                    TxtLabParam2.Text = "(Automático):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 1; SliderLab2.Value = 0;
                    TxtLabParam3.Text = "(Automático):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.BresenhamStepByStep:
                    TxtLabParam1.Text = "Destino X1 (3 a 15):";
                    SliderLab1.Minimum = 3; SliderLab1.Maximum = 15; SliderLab1.Value = 12;
                    TxtLabParam2.Text = "Destino Y1 (2 a 12):";
                    SliderLab2.Minimum = 2; SliderLab2.Maximum = 12; SliderLab2.Value = 8;
                    TxtLabParam3.Text = "(Automático):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                case LessonType.MatrixTransform2D:
                    TxtLabParam1.Text = "Translação X (Tx: -100 a +100):";
                    SliderLab1.Minimum = -100; SliderLab1.Maximum = 100; SliderLab1.Value = 0;
                    TxtLabParam2.Text = "Rotação Angular (Graus: 0-360°):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 360; SliderLab2.Value = 45;
                    TxtLabParam3.Text = "Escala (0.5x a 2.5x):";
                    SliderLab3.Minimum = 0.5; SliderLab3.Maximum = 2.5; SliderLab3.Value = 1.2;
                    break;

                case LessonType.PipelineMVP3D:
                    TxtLabParam1.Text = "Rotação Y do Modelo (0-360°):";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 360; SliderLab1.Value = 45;
                    TxtLabParam2.Text = "Distância Z da Câmera (Profundidade):";
                    SliderLab2.Minimum = 1.5; SliderLab2.Maximum = 8.0; SliderLab2.Value = 3.5;
                    TxtLabParam3.Text = "Campo de Visão (FOV em Graus):";
                    SliderLab3.Minimum = 30; SliderLab3.Maximum = 120; SliderLab3.Value = 60;
                    break;

                case LessonType.HierarchicalSceneGraph:
                    TxtLabParam1.Text = "Rotação da Base (Eixo Y):";
                    SliderLab1.Minimum = -90; SliderLab1.Maximum = 90; SliderLab1.Value = 15;
                    TxtLabParam2.Text = "Inclinação do Ombro (Eixo Z):";
                    SliderLab2.Minimum = -60; SliderLab2.Maximum = 60; SliderLab2.Value = 35;
                    TxtLabParam3.Text = "Articulação do Cotovelo (Eixo Z):";
                    SliderLab3.Minimum = -90; SliderLab3.Maximum = 90; SliderLab3.Value = -40;
                    break;

                case LessonType.RayTracingIntersection:
                    TxtLabParam1.Text = "Posição Vertical do Raio (Offset Y):";
                    SliderLab1.Minimum = -120; SliderLab1.Maximum = 120; SliderLab1.Value = -20;
                    TxtLabParam2.Text = "(Automático):";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 1; SliderLab2.Value = 0;
                    TxtLabParam3.Text = "(Automático):";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 1; SliderLab3.Value = 0;
                    break;

                default:
                    TxtLabParam1.Text = "Parâmetro 1:";
                    SliderLab1.Minimum = 0; SliderLab1.Maximum = 255; SliderLab1.Value = 128;
                    TxtLabParam2.Text = "Parâmetro 2:";
                    SliderLab2.Minimum = 0; SliderLab2.Maximum = 255; SliderLab2.Value = 128;
                    TxtLabParam3.Text = "Parâmetro 3:";
                    SliderLab3.Minimum = 0; SliderLab3.Maximum = 255; SliderLab3.Value = 128;
                    break;
            }

            SliderLab1.ValueChanged += SliderLab_ValueChanged;
            SliderLab2.ValueChanged += SliderLab_ValueChanged;
            SliderLab3.ValueChanged += SliderLab_ValueChanged;
        }

        private void LoadQuiz(InteractiveLesson lesson)
        {
            TxtQuizQuestion.Text = lesson.QuizQuestion;
            BrdQuizFeedback.Visibility = Visibility.Collapsed;

            ResetQuizButton(BtnQuizOpt0);
            ResetQuizButton(BtnQuizOpt1);
            ResetQuizButton(BtnQuizOpt2);

            if (lesson.QuizOptions.Count > 0)
            {
                BtnQuizOpt0.Content = "A) " + lesson.QuizOptions[0].Text;
                BtnQuizOpt0.Visibility = Visibility.Visible;
            }
            else BtnQuizOpt0.Visibility = Visibility.Collapsed;

            if (lesson.QuizOptions.Count > 1)
            {
                BtnQuizOpt1.Content = "B) " + lesson.QuizOptions[1].Text;
                BtnQuizOpt1.Visibility = Visibility.Visible;
            }
            else BtnQuizOpt1.Visibility = Visibility.Collapsed;

            if (lesson.QuizOptions.Count > 2)
            {
                BtnQuizOpt2.Content = "C) " + lesson.QuizOptions[2].Text;
                BtnQuizOpt2.Visibility = Visibility.Visible;
            }
            else BtnQuizOpt2.Visibility = Visibility.Collapsed;
        }

        private void ResetQuizButton(Button btn)
        {
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E2A"));
            btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323246"));
            btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0EC"));
        }

        private void BtnQuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int optIndex))
            {
                if (LstInteractiveLessons.SelectedItem is InteractiveLesson lesson && optIndex < lesson.QuizOptions.Count)
                {
                    var opt = lesson.QuizOptions[optIndex];
                    BrdQuizFeedback.Visibility = Visibility.Visible;

                    ResetQuizButton(BtnQuizOpt0);
                    ResetQuizButton(BtnQuizOpt1);
                    ResetQuizButton(BtnQuizOpt2);

                    if (opt.IsCorrect)
                    {
                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1B5E20"));
                        btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                        BrdQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2A20"));
                        BrdQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
                        TxtQuizFeedback.Text = "✅ RESPOSTA CORRETA!\n" + opt.Explanation;
                    }
                    else
                    {
                        btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5C1A1A"));
                        btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935"));
                        BrdQuizFeedback.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2A1E1E"));
                        BrdQuizFeedback.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7D2E2E"));
                        TxtQuizFeedback.Text = "❌ RESPOSTA INCORRETA.\n" + opt.Explanation;
                    }
                }
            }
        }

        private void SliderLab_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateLabSimulation();
        }

        private void BtnStepSimulation_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex++;
            UpdateLabSimulation();
        }

        private void BtnRunAllSimulation_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 25;
            UpdateLabSimulation();
        }

        private void BtnResetSimulation_Click(object sender, RoutedEventArgs e)
        {
            _simulationStepIndex = 0;
            UpdateLabSimulation();
        }

        private void UpdateLabSimulation()
        {
            if (_labBitmap == null || LstInteractiveLessons?.SelectedItem is not InteractiveLesson lesson) return;

            // Atualiza badges visuais com valores numéricos
            if (TxtLabVal1 != null) TxtLabVal1.Text = $"[ {SliderLab1.Value:F0} ]";
            if (TxtLabVal2 != null) TxtLabVal2.Text = $"[ {SliderLab2.Value:F0} ]";
            if (TxtLabVal3 != null) TxtLabVal3.Text = $"[ {SliderLab3.Value:F0} ]";

            StringBuilder log = new StringBuilder();
            InteractiveLabManager.RenderSimulation(
                _labBitmap,
                lesson,
                SliderLab1.Value,
                SliderLab2.Value,
                SliderLab3.Value,
                255,
                _simulationStepIndex,
                log);

            TxtLabConsole.Text = log.ToString();
        }

        #region Controles de Zoom e Cópia de Código & Texto

        // --- Zoom & Cópia para Tab 5 (Documentação Integrada) ---
        private void BtnZoomInDocCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbDocSnippet != null && RtbDocSnippet.FontSize < 28) RtbDocSnippet.FontSize += 1.5;
        }

        private void BtnZoomOutDocCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbDocSnippet != null && RtbDocSnippet.FontSize > 10) RtbDocSnippet.FontSize -= 1.5;
        }

        private void BtnResetDocCodeZoom_Click(object sender, RoutedEventArgs e)
        {
            if (RtbDocSnippet != null) RtbDocSnippet.FontSize = 13.0;
        }

        private void BtnCopyDocCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbDocSnippet != null)
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbDocSnippet);
                if (!string.IsNullOrEmpty(code))
                {
                    Clipboard.SetText(code);
                    UpdateStatus("Código C# copiado para a área de transferência!", 0);
                }
            }
        }

        // --- Zoom & Cópia para Tab 6 (Laboratório Interativo) ---
        private void BtnZoomInLab_Click(object sender, RoutedEventArgs e)
        {
            if (TxtLessonSummary.FontSize < 22) TxtLessonSummary.FontSize += 1.5;
        }

        private void BtnZoomOutLab_Click(object sender, RoutedEventArgs e)
        {
            if (TxtLessonSummary.FontSize > 10) TxtLessonSummary.FontSize -= 1.5;
        }

        private void BtnZoomInLabCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbLabCode.FontSize < 28) RtbLabCode.FontSize += 1.5;
            if (TxtLabExplanation.FontSize < 24) TxtLabExplanation.FontSize += 1.0;
        }

        private void BtnZoomOutLabCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbLabCode.FontSize > 10) RtbLabCode.FontSize -= 1.5;
            if (TxtLabExplanation.FontSize > 9) TxtLabExplanation.FontSize -= 1.0;
        }

        private void BtnResetLabCodeZoom_Click(object sender, RoutedEventArgs e)
        {
            RtbLabCode.FontSize = 13.5;
            TxtLabExplanation.FontSize = 13.0;
        }

        private void BtnCopyLabCode_Click(object sender, RoutedEventArgs e)
        {
            string code = CSharpSyntaxHighlighter.GetPlainText(RtbLabCode);
            if (!string.IsNullOrEmpty(code))
            {
                Clipboard.SetText(code);
                UpdateStatus("Código C# da lição copiado para a área de transferência!", 0);
            }
        }

        private void BtnZoomInConsole_Click(object sender, RoutedEventArgs e)
        {
            if (TxtLabConsole.FontSize < 26) TxtLabConsole.FontSize += 1.5;
        }

        private void BtnZoomOutConsole_Click(object sender, RoutedEventArgs e)
        {
            if (TxtLabConsole.FontSize > 10) TxtLabConsole.FontSize -= 1.5;
        }

        private void BtnCopyConsole_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtLabConsole.Text))
            {
                Clipboard.SetText(TxtLabConsole.Text);
                UpdateStatus("Logs do console copiados para a área de transferência!", 0);
            }
        }

        // --- Controles de Compilação Dinâmica & Testes Automatizados (Roslyn) ---

        private async void BtnRunUserCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is not InteractiveLesson lesson) return;

            BtnRunUserCode.IsEnabled = false;
            BtnRunTests.IsEnabled = false;
            TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
            TxtCompilerReport.Text = "Compilando com Roslyn e executando testes em tempo real...";

            string userCode = CSharpSyntaxHighlighter.GetPlainText(RtbLabEditableCode);
            var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(
                lesson,
                userCode,
                _labBitmap,
                SliderLab1.Value,
                SliderLab2.Value,
                SliderLab3.Value);

            BtnRunUserCode.IsEnabled = true;
            BtnRunTests.IsEnabled = true;

            DisplayCompilerReport(report);
        }

        private async void BtnRunTests_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is not InteractiveLesson lesson) return;

            BtnRunUserCode.IsEnabled = false;
            BtnRunTests.IsEnabled = false;
            TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
            TxtCompilerReport.Text = "Executando bateria de testes unitários automatizados...";

            string userCode = CSharpSyntaxHighlighter.GetPlainText(RtbLabEditableCode);
            var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(
                lesson,
                userCode,
                null, // Apenas testes, sem renderizar no canvas
                SliderLab1.Value,
                SliderLab2.Value,
                SliderLab3.Value);

            BtnRunUserCode.IsEnabled = true;
            BtnRunTests.IsEnabled = true;

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

            TxtCompilerReport.Text = sb.ToString();
            TxtCompilerReport.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(report.Success ? "#86EFAC" : (isCustomSuccess ? "#38BDF8" : "#FCA5A5")));

            UpdateStatus(report.Success ? "Código compilado e testado com 100% de sucesso!" : (isCustomSuccess ? "Código personalizado renderizado no Canvas." : "Erros encontrados no código do aluno."), report.ExecutionTimeMs);
        }

        private void BtnBlankCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is InteractiveLesson lesson)
            {
                CSharpSyntaxHighlighter.SetCode(RtbLabEditableCode, lesson.BlankTemplate);
                TxtCompilerReport.Text = "Modo 'Em Branco' ativado. Escreva o algoritmo do zero e clique em 'Compilar e Executar C#'.";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
            }
        }

        private void BtnStarterCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is InteractiveLesson lesson)
            {
                CSharpSyntaxHighlighter.SetCode(RtbLabEditableCode, lesson.StarterTemplate);
                TxtCompilerReport.Text = "Template inicial carregado com comentários orientadores (TODOs).";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1"));
            }
        }

        private void BtnRestoreOfficialCode_Click(object sender, RoutedEventArgs e)
        {
            if (LstInteractiveLessons?.SelectedItem is InteractiveLesson lesson)
            {
                string code = !string.IsNullOrEmpty(lesson.SolutionCode) ? lesson.SolutionCode : lesson.CodeSnippet;
                CSharpSyntaxHighlighter.SetCode(RtbLabEditableCode, code);
                TxtCompilerReport.Text = "Gabarito oficial de referência carregado no editor com destaque sintático.";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
            }
        }

        private void BtnZoomInEditableCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbLabEditableCode.FontSize < 32) RtbLabEditableCode.FontSize += 1.5;
        }

        private void BtnZoomOutEditableCode_Click(object sender, RoutedEventArgs e)
        {
            if (RtbLabEditableCode.FontSize > 10) RtbLabEditableCode.FontSize -= 1.5;
        }

        // --- Controles de Layout Retrátil & Estúdio em Janela Dedicada ---

        private void BtnOpenPopoutStudio_Click(object sender, RoutedEventArgs e)
        {
            int currentLessonNum = (LstInteractiveLessons?.SelectedItem is InteractiveLesson lesson) ? lesson.Number : 1;
            var studio = new CodeStudioWindow(currentLessonNum);
            studio.Show();
            studio.Activate();
        }

        private void BtnToggleLabTrack_Click(object sender, RoutedEventArgs e)
        {
            if (ColLabTrack.Width.Value > 0)
            {
                ColLabTrack.Width = new GridLength(0);
                ColLabSplitter1.Width = new GridLength(0);
                BtnToggleLabTrack.Content = "▶ Expandir Trilha";
            }
            else
            {
                ColLabTrack.Width = new GridLength(390);
                ColLabSplitter1.Width = new GridLength(5);
                BtnToggleLabTrack.Content = "◀ Recolher Trilha";
            }
        }

        private void BtnTogglePlayground_Click(object sender, RoutedEventArgs e)
        {
            if (ColLabPlayground.Width.Value > 0)
            {
                ColLabPlayground.Width = new GridLength(0);
                ColLabSplitter2.Width = new GridLength(0);
                BtnTogglePlayground.Content = "▶ Expandir Canvas";
            }
            else
            {
                ColLabPlayground.Width = new GridLength(380);
                ColLabSplitter2.Width = new GridLength(5);
                BtnTogglePlayground.Content = "◀ Recolher Canvas";
            }
        }

        private void BtnFocusCode_Click(object sender, RoutedEventArgs e)
        {
            ColLabTrack.Width = new GridLength(0);
            ColLabSplitter1.Width = new GridLength(0);
            ColLabPlayground.Width = new GridLength(0);
            ColLabSplitter2.Width = new GridLength(0);
            BtnToggleLabTrack.Content = "▶ Expandir Trilha";
            BtnTogglePlayground.Content = "▶ Expandir Canvas";
            UpdateStatus("Modo Foco ativado: 100% da largura alocada ao editor de código e testes.", 0);
        }

        private void BtnResetColumns_Click(object sender, RoutedEventArgs e)
        {
            ColLabTrack.Width = new GridLength(390);
            ColLabSplitter1.Width = new GridLength(5);
            ColLabPlayground.Width = new GridLength(380);
            ColLabSplitter2.Width = new GridLength(5);
            BtnToggleLabTrack.Content = "◀ Recolher Trilha";
            BtnTogglePlayground.Content = "◀ Recolher Canvas";
            UpdateStatus("Painéis restaurados para a disposição balanceada de 3 colunas.", 0);
        }

        private async void BtnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Verificando novas versões no GitHub Releases...", 0);
            var release = await UpdateManager.CheckForUpdatesAsync();
            if (release != null)
            {
                // Verificação manual: sempre mostra, mesmo que a versão esteja adiada/ignorada
                var dlg = new UpdateDialogWindow(release, UpdateSettingsStore.Load()) { Owner = this };
                dlg.ShowDialog();
            }
            else
            {
                MessageBox.Show($"Você já está usando a versão mais recente (v{UpdateManager.CurrentVersionString}) do CGPDI StudyLab!",
                    "Nenhuma Atualização Pendente", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateStatus($"Aplicação atualizada na versão mais recente (v{UpdateManager.CurrentVersionString}).", 0);
            }
        }

        private void BtnOpenProjectStudio_Click(object sender, RoutedEventArgs e)
        {
            var studioWin = new ProjectStudioWindow { Owner = this };
            studioWin.Show();
            UpdateStatus("Estúdio de Projetos do Zero aberto em janela dedicada.", 0);
        }

        #region Manipuladores WPF / XAML do Laboratório

        private void BtnLabRenderXaml_Click(object sender, RoutedEventArgs e)
        {
            if (RtbLabXamlCode == null) return;
            string xamlCode = XamlSyntaxHighlighter.GetPlainText(RtbLabXamlCode);
            var result = LiveCodeCompiler.EvaluateXaml(xamlCode);

            if (result.Success && result.Element != null)
            {
                PnlLabLiveXamlContainer.Child = result.Element;
                TabLabVisualizer.SelectedItem = TabItemLabLiveXaml;
                TxtCompilerReport.Text = $"[XAML Renderizado com Êxito em {result.ExecutionTimeMs:F1} ms]\n{result.Logs}";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
                UpdateStatus($"Interface WPF instanciada dinamicamente via XamlReader ({result.ExecutionTimeMs:F1} ms).", 0);
            }
            else
            {
                TxtCompilerReport.Text = $"[Erro de Compilação XAML]:\n{result.ErrorMessage}";
                TxtCompilerReport.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F87171"));
                UpdateStatus("Erro de sintaxe na marcação XAML.", 0);
            }
        }

        private void BtnLabXamlTemplateBasic_Click(object sender, RoutedEventArgs e)
        {
            string xaml = @"<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height=""Auto""/>
        <RowDefinition Height=""*""/>
    </Grid.RowDefinitions>
    <Border Grid.Row=""0"" Background=""#1E293B"" CornerRadius=""6"" Padding=""10"" Margin=""0,0,0,10"">
        <StackPanel>
            <TextBlock Text=""Painel WPF Nativo Interativo"" Foreground=""#38BDF8"" FontSize=""15"" FontWeight=""Bold""/>
            <TextBlock Text=""Gerado dinamicamente a partir do seu código XAML via XamlReader."" Foreground=""#94A3B8"" FontSize=""11.5""/>
        </StackPanel>
    </Border>
    <StackPanel Grid.Row=""1"" VerticalAlignment=""Center"" HorizontalAlignment=""Center"">
        <Button Content=""Botão Dinâmico WPF"" Background=""#2563EB"" Foreground=""#FFFFFF"" Padding=""16,8"" FontSize=""13"" FontWeight=""Bold"" Margin=""0,0,0,10""/>
        <Slider Minimum=""0"" Maximum=""100"" Value=""65"" Width=""240"" Height=""24""/>
    </StackPanel>
</Grid>";
            XamlSyntaxHighlighter.SetCode(RtbLabXamlCode, xaml);
            BtnLabRenderXaml_Click(sender, e);
        }

        private void BtnLabXamlTemplateShapes_Click(object sender, RoutedEventArgs e)
        {
            string xaml = @"<Canvas Background=""#0B0F19"">
    <Ellipse Canvas.Left=""40"" Canvas.Top=""30"" Width=""100"" Height=""100"" Fill=""#3B82F6"" Stroke=""#60A5FA"" StrokeThickness=""3""/>
    <Rectangle Canvas.Left=""110"" Canvas.Top=""80"" Width=""120"" Height=""80"" Fill=""#10B981"" RadiusX=""8"" RadiusY=""8"" Opacity=""0.85""/>
    <Polygon Points=""260,30 320,130 200,130"" Fill=""#F59E0B"" Stroke=""#FBBF24"" StrokeThickness=""2""/>
    <TextBlock Canvas.Left=""40"" Canvas.Top=""180"" Text=""Formas Vetoriais GPU"" Foreground=""#E2E8F0"" FontSize=""12"" FontWeight=""Bold""/>
</Canvas>";
            XamlSyntaxHighlighter.SetCode(RtbLabXamlCode, xaml);
            BtnLabRenderXaml_Click(sender, e);
        }

        private void BtnLabXamlTemplateAnim_Click(object sender, RoutedEventArgs e)
        {
            string xaml = @"<Grid>
    <Border Width=""120"" Height=""120"" CornerRadius=""60"" Background=""#EC4899"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"">
        <Border.Triggers>
            <EventTrigger RoutedEvent=""Border.Loaded"">
                <BeginStoryboard>
                    <Storyboard RepeatBehavior=""Forever"" AutoReverse=""True"">
                        <DoubleAnimation Storyboard.TargetProperty=""Opacity"" From=""0.2"" To=""1.0"" Duration=""0:0:1.5""/>
                    </Storyboard>
                </BeginStoryboard>
            </EventTrigger>
        </Border.Triggers>
        <TextBlock Text=""Pulsar WPF"" Foreground=""#FFFFFF"" FontWeight=""Bold"" FontSize=""13"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""/>
    </Border>
</Grid>";
            XamlSyntaxHighlighter.SetCode(RtbLabXamlCode, xaml);
            BtnLabRenderXaml_Click(sender, e);
        }

        private void BtnLabCopyXaml_Click(object sender, RoutedEventArgs e)
        {
            string text = XamlSyntaxHighlighter.GetPlainText(RtbLabXamlCode);
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
                UpdateStatus("Marcação XAML copiada para a área de transferência!", 0);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}