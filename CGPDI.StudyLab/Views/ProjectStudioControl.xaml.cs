using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CGPDI.StudyLab.Core;
using Microsoft.Win32;

namespace CGPDI.StudyLab.Views
{
    public partial class ProjectStudioControl : UserControl
    {
        private DirectBitmap _freeBitmap = null!;
        private List<ProjectTemplate> _templates = new();
        private bool _isCompilingFree = false;
        private bool _isInitializing = true;
        private bool _isHighlighting = false;
        private bool _pendingScriptExecution = false;

        private readonly DispatcherTimer _sliderDebounceTimer;
        private readonly DispatcherTimer _highlightDebounceTimer;

        public event EventHandler? PopoutRequested;

        public ProjectStudioControl()
        {
            InitializeComponent();

            _sliderDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            _sliderDebounceTimer.Tick += SliderDebounceTimer_Tick;

            _highlightDebounceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
            _highlightDebounceTimer.Tick += HighlightDebounceTimer_Tick;

            InitStudio();
            Loaded += ProjectStudioControl_Loaded;
        }

        private void ProjectStudioControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitializing) return;
            _isInitializing = false;

            LoadDefaultXamlSnippet();

            RtbFreeCode.TextChanged += RtbFreeCode_TextChanged;
            RtbFreeXamlCode.TextChanged += RtbFreeCode_TextChanged;

            _ = ExecuteFreeScript();
        }

        private void InitStudio()
        {
            _freeBitmap = new DirectBitmap(512, 512);
            ImgFreeSimulation.Source = _freeBitmap.Bitmap;

            _templates = ProjectTemplatesManager.GetTemplates();
            LstProjectTemplates.ItemsSource = _templates;
            if (_templates.Count > 0)
            {
                LstProjectTemplates.SelectedIndex = 0;
            }
        }

        private void LoadDefaultXamlSnippet()
        {
            string xaml = @"<Viewbox Margin=""10"">
    <Canvas Width=""500"" Height=""350"">
        <!-- Fundo em Degradê Escuro -->
        <Canvas.Resources>
            <LinearGradientBrush x:Key=""GradCard"" StartPoint=""0,0"" EndPoint=""1,1"">
                <GradientStop Color=""#1E293B"" Offset=""0.0""/>
                <GradientStop Color=""#0F172A"" Offset=""1.0""/>
            </LinearGradientBrush>
            <LinearGradientBrush x:Key=""GradNeon"" StartPoint=""0,0"" EndPoint=""1,0"">
                <GradientStop Color=""#38BDF8"" Offset=""0.0""/>
                <GradientStop Color=""#818CF8"" Offset=""1.0""/>
            </LinearGradientBrush>
        </Canvas.Resources>

        <!-- Cartão Base -->
        <Rectangle Canvas.Left=""20"" Canvas.Top=""20"" Width=""460"" Height=""310"" RadiusX=""12"" RadiusY=""12""
                   Fill=""{StaticResource GradCard}"" Stroke=""#334155"" StrokeThickness=""1.5""/>

        <!-- Formas Vetoriais Decorativas -->
        <Ellipse Canvas.Left=""50"" Canvas.Top=""50"" Width=""80"" Height=""80"" Fill=""{StaticResource GradNeon}"" Opacity=""0.85""/>
        <Path Data=""M 200,80 L 260,180 L 140,180 Z"" Fill=""#F43F5E"" Opacity=""0.75""/>

        <!-- Texto Estilizado -->
        <TextBlock Canvas.Left=""50"" Canvas.Top=""220"" Text=""CGPDI StudyLab • WPF Vetorial""
                   Foreground=""#F8FAFC"" FontSize=""18"" FontWeight=""Bold""/>
        <TextBlock Canvas.Left=""50"" Canvas.Top=""250"" Text=""Renderização nativa XAML com Viewbox, Canvas e Brushes.""
                   Foreground=""#94A3B8"" FontSize=""12""/>
    </Canvas>
</Viewbox>";
            _isHighlighting = true;
            XamlSyntaxHighlighter.SetCode(RtbFreeXamlCode, xaml);
            _isHighlighting = false;
        }

        private void LstProjectTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstProjectTemplates.SelectedItem is not ProjectTemplate tpl) return;

            TxtStudioCurrentProject.Text = $"[Projeto] {tpl.Title}";
            TxtTemplateDesc.Text = tpl.Description;

            _isHighlighting = true;
            CSharpSyntaxHighlighter.SetCode(RtbFreeCode, tpl.InitialCode);
            _isHighlighting = false;

            ConfigureSliders(tpl);

            if (!_isInitializing)
            {
                _ = ExecuteFreeScript();
            }
        }

        private void ConfigureSliders(ProjectTemplate tpl)
        {
            SliderFree1.ValueChanged -= SliderFree_ValueChanged;
            SliderFree2.ValueChanged -= SliderFree_ValueChanged;
            SliderFree3.ValueChanged -= SliderFree_ValueChanged;
            SliderFree4.ValueChanged -= SliderFree_ValueChanged;

            TxtFreeParam1.Text = tpl.Param1Name;
            SliderFree1.Minimum = tpl.Param1Min;
            SliderFree1.Maximum = tpl.Param1Max;
            SliderFree1.Value = tpl.Param1Default;

            TxtFreeParam2.Text = tpl.Param2Name;
            SliderFree2.Minimum = tpl.Param2Min;
            SliderFree2.Maximum = tpl.Param2Max;
            SliderFree2.Value = tpl.Param2Default;

            TxtFreeParam3.Text = tpl.Param3Name;
            SliderFree3.Minimum = tpl.Param3Min;
            SliderFree3.Maximum = tpl.Param3Max;
            SliderFree3.Value = tpl.Param3Default;

            TxtFreeParam4.Text = tpl.Param4Name;
            SliderFree4.Minimum = tpl.Param4Min;
            SliderFree4.Maximum = tpl.Param4Max;
            SliderFree4.Value = tpl.Param4Default;

            TxtFreeVal1.Text = $"[ {SliderFree1.Value:F1} ]";
            TxtFreeVal2.Text = $"[ {SliderFree2.Value:F1} ]";
            TxtFreeVal3.Text = $"[ {SliderFree3.Value:F1} ]";
            TxtFreeVal4.Text = $"[ {SliderFree4.Value:F1} ]";

            SliderFree1.ValueChanged += SliderFree_ValueChanged;
            SliderFree2.ValueChanged += SliderFree_ValueChanged;
            SliderFree3.ValueChanged += SliderFree_ValueChanged;
            SliderFree4.ValueChanged += SliderFree_ValueChanged;
        }

        private void SliderFree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isInitializing) return;

            if (TxtFreeVal1 != null) TxtFreeVal1.Text = $"[ {SliderFree1.Value:F1} ]";
            if (TxtFreeVal2 != null) TxtFreeVal2.Text = $"[ {SliderFree2.Value:F1} ]";
            if (TxtFreeVal3 != null) TxtFreeVal3.Text = $"[ {SliderFree3.Value:F1} ]";
            if (TxtFreeVal4 != null) TxtFreeVal4.Text = $"[ {SliderFree4.Value:F1} ]";

            _sliderDebounceTimer.Stop();
            _sliderDebounceTimer.Start();
        }

        private static readonly SolidColorBrush ConsoleCyanBrush = new((Color)ColorConverter.ConvertFromString("#38BDF8"));
        private static readonly SolidColorBrush ConsoleRedBrush = new((Color)ColorConverter.ConvertFromString("#F87171"));

        private void SliderDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _sliderDebounceTimer.Stop();
            _ = ExecuteFreeScript();
        }

        private void RtbFreeCode_TextChanged(object sender, TextChangedEventArgs e)
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
                    int offset = XamlSyntaxHighlighter.GetCaretCharIndex(RtbFreeXamlCode);
                    string xaml = XamlSyntaxHighlighter.GetPlainText(RtbFreeXamlCode);
                    XamlSyntaxHighlighter.Highlight(RtbFreeXamlCode, xaml);
                    XamlSyntaxHighlighter.SetCaretCharIndex(RtbFreeXamlCode, offset);
                }
                else if (TabStudioEditor.SelectedIndex == 0)
                {
                    int offset = CSharpSyntaxHighlighter.GetCaretCharIndex(RtbFreeCode);
                    string code = CSharpSyntaxHighlighter.GetPlainText(RtbFreeCode);
                    CSharpSyntaxHighlighter.Highlight(RtbFreeCode, code);
                    CSharpSyntaxHighlighter.SetCaretCharIndex(RtbFreeCode, offset);
                }
            }
            catch (Exception)
            {
                // Fallback seguro durante digitação rápida
            }
            finally
            {
                _isHighlighting = false;
            }
        }

        private void CbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbResolution.SelectedItem is ComboBoxItem item && int.TryParse(item.Tag?.ToString(), out int res)
                && _freeBitmap != null && (_freeBitmap.Width != res || _freeBitmap.Height != res))
            {
                _freeBitmap = new DirectBitmap(res, res);
                ImgFreeSimulation.Source = _freeBitmap.Bitmap;
                if (!_isInitializing) _ = ExecuteFreeScript();
            }
        }

        public async Task ExecuteFreeScript()
        {
            if (_freeBitmap == null) return;

            if (_isCompilingFree)
            {
                _pendingScriptExecution = true;
                return;
            }

            _isCompilingFree = true;
            _pendingScriptExecution = false;

            try
            {
                string code = CSharpSyntaxHighlighter.GetPlainText(RtbFreeCode);
                double p1 = SliderFree1.Value;
                double p2 = SliderFree2.Value;
                double p3 = SliderFree3.Value;
                double p4 = SliderFree4.Value;

                TxtStudioStatus.Text = "Renderizando DirectBitmap...";

                var result = await LiveCodeCompiler.ExecuteCustomScriptAsync(code, _freeBitmap, null, p1, p2, p3, p4);

                if (result.Success)
                {
                    TxtFreeStats.Text = $"Tempo: {result.ExecutionTimeMs:F1} ms • Resolução: {_freeBitmap.Width}×{_freeBitmap.Height}";
                    TxtFreeConsole.Text = string.IsNullOrEmpty(result.Logs)
                        ? $"[Compilação com Êxito] Script C# executado em {result.ExecutionTimeMs:F1} ms."
                        : $"[Logs de Execução]:\n{result.Logs}\n\nTempo: {result.ExecutionTimeMs:F1} ms.";
                    TxtFreeConsole.Foreground = ConsoleCyanBrush;
                    TxtStudioStatus.Text = $"Pronto. Executado em {result.ExecutionTimeMs:F1} ms.";
                    TabStudioVisualizer.SelectedIndex = 0;
                }
                else
                {
                    TxtFreeStats.Text = $"Erro de Compilação • Resolução: {_freeBitmap.Width}×{_freeBitmap.Height}";
                    TxtFreeConsole.Text = $"[Falha na Execução Roslyn]:\n{result.ErrorMessage}";
                    TxtFreeConsole.Foreground = ConsoleRedBrush;
                    TxtStudioStatus.Text = "Erro detectado no código. Verifique os diagnósticos no console.";
                }
            }
            catch (Exception ex)
            {
                TxtFreeConsole.Text = $"[Erro de Execução]:\n{ex.Message}";
                TxtFreeConsole.Foreground = ConsoleRedBrush;
                TxtStudioStatus.Text = "Erro durante a execução do script.";
            }
            finally
            {
                _isCompilingFree = false;
                if (_pendingScriptExecution)
                {
                    _pendingScriptExecution = false;
                    _ = ExecuteFreeScript();
                }
            }
        }

        public void ExecuteFreeXaml()
        {
            try
            {
                string xamlCode = XamlSyntaxHighlighter.GetPlainText(RtbFreeXamlCode);
                TxtStudioStatus.Text = "Processando marcação XAML com XamlReader...";

                var result = LiveCodeCompiler.EvaluateXaml(xamlCode);

                if (result.Success && result.Element != null)
                {
                    PnlFreeLiveXamlContainer.Child = null;
                    PnlFreeLiveXamlContainer.Child = result.Element;
                    TabStudioVisualizer.SelectedItem = TabItemFreeLiveXaml;
                    TxtFreeConsole.Text = $"[XAML Renderizado com Êxito em {result.ExecutionTimeMs:F1} ms]\n{result.Logs}";
                    TxtFreeConsole.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
                    TxtStudioStatus.Text = $"Elemento WPF instanciado e ativo ({result.ExecutionTimeMs:F1} ms).";
                }
                else
                {
                    TxtFreeConsole.Text = $"[Erro de Compilação XAML]:\n{result.ErrorMessage}";
                    TxtFreeConsole.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F87171"));
                    TxtStudioStatus.Text = "Falha ao analisar a marcação XAML.";
                }
            }
            catch (Exception ex)
            {
                TxtFreeConsole.Text = $"[Erro de Execução XAML]:\n{ex.Message}";
                TxtFreeConsole.Foreground = ConsoleRedBrush;
                TxtStudioStatus.Text = "Erro ao renderizar árvore visual XAML.";
            }
        }

        private void BtnRunCode_Click(object sender, RoutedEventArgs e)
        {
            if (TabStudioEditor.SelectedIndex == 1)
            {
                ExecuteFreeXaml();
            }
            else
            {
                LiveCodeCompiler.ClearCustomScriptCache();
                _ = ExecuteFreeScript();
            }
        }

        private void TabStudioEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != TabStudioEditor) return;

            if (TabStudioEditor.SelectedIndex == 1)
            {
                TabStudioVisualizer.SelectedItem = TabItemFreeLiveXaml;
            }
            else if (TabStudioEditor.SelectedIndex == 0)
            {
                TabStudioVisualizer.SelectedIndex = 0;
            }
        }

        private void BtnClearCode_Click(object sender, RoutedEventArgs e)
        {
            _isHighlighting = true;
            if (TabStudioEditor.SelectedIndex == 1)
            {
                XamlSyntaxHighlighter.SetCode(RtbFreeXamlCode, "<Canvas Width=\"400\" Height=\"300\">\n    \n</Canvas>");
            }
            else
            {
                CSharpSyntaxHighlighter.SetCode(RtbFreeCode, "// Digite seu código C# aqui\nOutput.Clear(0xFF101018);\n");
            }
            _isHighlighting = false;
        }

        private void BtnResetTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (LstProjectTemplates.SelectedItem is ProjectTemplate tpl)
            {
                _isHighlighting = true;
                CSharpSyntaxHighlighter.SetCode(RtbFreeCode, tpl.InitialCode);
                _isHighlighting = false;
                ConfigureSliders(tpl);
                _ = ExecuteFreeScript();
            }
        }

        private void BtnExportImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sfd = new SaveFileDialog
                {
                    Filter = "Imagem PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp",
                    FileName = $"CGPDI_Studio_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                };

                if (sfd.ShowDialog() == true)
                {
                    using var stream = new FileStream(sfd.FileName, FileMode.Create);
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(_freeBitmap.Bitmap));
                    encoder.Save(stream);
                    TxtStudioStatus.Text = $"Imagem exportada com sucesso: {Path.GetFileName(sfd.FileName)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar imagem: {ex.Message}", "Erro de Exportação", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPopoutStudio_Click(object sender, RoutedEventArgs e)
        {
            PopoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
